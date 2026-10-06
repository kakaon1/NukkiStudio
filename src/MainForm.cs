using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using NukkiStudio.App.Controls;
using NukkiStudio.App.Detection;
using NukkiStudio.App.Editing;
using NukkiStudio.App.Export;
using NukkiStudio.App.Imaging;
using NukkiStudio.App.Inpainting;
using NukkiStudio.App.Models;
using NukkiStudio.App.Segmentation;
using NukkiStudio.App.UI;

namespace NukkiStudio.App;

public partial class MainForm : Form
{
    private enum ViewMode
    {
        Overlay,
        Cutout,
        Mask,
    }

    private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tif", ".tiff" };

    private string _modelsDirectory = "";
    private IReadOnlyList<ModelInfo> _models = Array.Empty<ModelInfo>();
    private ModelInfo? _modelInfo;
    private ISegmentationModel? _model;
    private DevicePreference _device = DevicePreference.Auto;
    private bool _modelLoading;

    // 이미지 목록 / 현재 문서
    private ImageItem? _currentItem;
    private int _pasteCounter;
    private EditorDocument? _doc;
    private Bitmap? _displayImage;   // 원본 (표시용 PArgb)
    private Bitmap? _overlay;        // 오버레이
    private Bitmap? _viewImage;      // 누끼 결과 / 마스크 보기용
    private ViewMode _view = ViewMode.Overlay;
    private bool _imageHasAlpha;

    // 인코딩 / 예측 상태
    private ImageBuffer? _encodedImage;
    private bool _encodeRunning;
    private bool _encodePending;
    private int _predictVersion;

    // 객체 지우기 / 배경 지우기 / 일괄 저장
    private IInpainter? _inpainter;
    private ObjectDetector? _detector;
    private bool _editBusy;

    private Color _backgroundColor = Color.White;
    private ExportSettings _exportSettings = new();
    private AppPreferences _prefs = new();
    private bool _syncingInpaintCombo;
    private bool _startupDone;
    private readonly ToolTip _tips = new();
    private string? _lastOutputFolder;
    private readonly CancellationTokenSource _closing = new();

    /// <summary>시작할 때 열 이미지 파일 / 폴더 (명령줄 인수).</summary>
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public string[] InitialPaths { get; init; } = Array.Empty<string>();

    public MainForm()
    {
        InitializeComponent();
        if (DesignMode) return;

        InitializePaths();
        LoadSettings();
        InitializeEvents();
        ApplyTheme();
        btnBackgroundColor.Enabled = chkBackground.Checked;
        UpdateRefineLabels();
        UpdateUi();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!DesignMode) Theme.ApplyDarkTitleBar(Handle);
    }

    /// <summary>다크 렌더러, 도구 모음 아이콘, 강조 버튼 비활성 색 처리.</summary>
    private void ApplyTheme()
    {
        var renderer = new DarkToolStripRenderer();
        menuStrip.Renderer = renderer;
        toolStrip.Renderer = renderer;
        statusStrip.Renderer = renderer;

        int iconSize = (int)Math.Round(18 * DeviceDpi / 96.0);
        void SetIcon(ToolStripButton button, AppIcon icon) => button.Image = IconFactory.Create(icon, iconSize, Theme.Text);
        SetIcon(btnOpen, AppIcon.Open);
        SetIcon(btnSave, AppIcon.Save);
        SetIcon(btnAutoDetect, AppIcon.AutoDetect);
        SetIcon(btnToolPoint, AppIcon.Point);
        SetIcon(btnToolBox, AppIcon.Box);
        SetIcon(btnToolBrushAdd, AppIcon.Brush);
        UpdateBrushShapeIcon();
        SetIcon(btnEraseObject, AppIcon.Eraser);
        SetIcon(btnRemoveBackground, AppIcon.RemoveBackground);
        SetIcon(btnUndo, AppIcon.Undo);
        SetIcon(btnRedo, AppIcon.Redo);
        SetIcon(btnHelp, AppIcon.Info);
        SetIcon(btnFit, AppIcon.Fit);
        SetIcon(btnActual, AppIcon.Actual);

        // 강조 버튼은 비활성일 때 일반 버튼 색으로
        foreach (var button in new[] { btnCommit, btnSavePng, btnSaveBatch })
        {
            button.EnabledChanged += (_, _) => button.BackColor = button.Enabled ? Theme.Accent : Theme.Raised;
        }
    }

    private void LoadSettings()
    {
        _exportSettings = ExportSettings.Load();
        UpdateExportSummary();
        _prefs = AppPreferences.Load();
        trkBrush.Value = Math.Clamp(_prefs.BrushSize, trkBrush.Minimum, trkBrush.Maximum);
        canvas.BrushRadius = trkBrush.Value / 2f;
        canvas.BrushSquare = _prefs.BrushSquare;
    }

    private void InitializePaths()
    {
        _modelsDirectory = Path.Combine(AppContext.BaseDirectory, "models");
    }

    private void InitializeEvents()
    {
        Shown += MainForm_Shown;
        FormClosing += MainForm_FormClosing;
        FormClosed += MainForm_FormClosed;
        canvas.PointPrompt += canvas_PointPrompt;
        canvas.BoxPrompt += canvas_BoxPrompt;
        canvas.BrushStroke += canvas_BrushStroke;
        canvas.ZoomChanged += (_, _) => lblZoom.Text = $"{canvas.Zoom * 100:0}%";
        canvas.EmptyAreaClick += mnuFileOpen_Click;
        canvas.FloatingCommit += (_, _) => CommitFloating();
        btnBrushSize.Stepper.Increase += (_, _) => ChangeBrushSize(+1);
        btnBrushSize.Stepper.Decrease += (_, _) => ChangeBrushSize(-1);

        // 창 어디에 끌어다 놓아도 열리도록 모든 하위 컨트롤을 드롭 대상으로 등록
        DragOver += MainForm_DragEnter;
        EnableDropTargets(this);
    }

    private void EnableDropTargets(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            child.AllowDrop = true;
            child.DragEnter += MainForm_DragEnter;
            child.DragOver += MainForm_DragEnter;
            child.DragDrop += MainForm_DragDrop;
            EnableDropTargets(child);
        }
    }

    private async void MainForm_Shown(object? sender, EventArgs e)
    {
        RefreshInpaintCombo();
        if (InitialPaths.Length > 0) AddImages(InitialPaths);

        // 없는 모델이 있으면 받을지 먼저 묻는다 (용량 표시, 사용자가 [다운로드]를 눌러야 받음)
        // 한 번 넘긴 선택 모델은 다시 묻지 않는다 (필수 모델이 없으면 항상 묻는다)
        var missing = ModelDownloadForm.Missing(_modelsDirectory);
        if (missing.Any(m => m.Required || !_prefs.SkippedModels.Contains(m.Id)))
        {
            ShowModelDownload(missing.Select(m => m.Id).ToArray(),
                missing.Any(m => m.Required)
                    ? "객체 선택에 꼭 필요한 AI 모델이 없습니다. 받을 모델을 고른 뒤 [다운로드]를 누르세요.\r\n데이터 요금(핫스팟 등)에 주의하세요. 받은 모델은 models 폴더에 저장되어 다시 받지 않습니다."
                    : "아직 받지 않은 AI 모델이 있습니다. 필요한 모델만 골라 [다운로드]를 누르세요.\r\n[나중에]를 누르면 다음부터 시작할 때 묻지 않습니다 (설정 → AI 모델 다운로드 / 관리에서 언제든 받기).");
            var stillMissing = ModelDownloadForm.Missing(_modelsDirectory).Where(m => !m.Required).Select(m => m.Id);
            _prefs.SkippedModels = _prefs.SkippedModels.Union(stillMissing).ToList();
            _prefs.Save();
        }

        _models = ModelCatalog.Scan(_modelsDirectory);
        BuildModelMenu();
        _startupDone = true;

        var first = _models.FirstOrDefault(m => m.IsSupported);
        if (first is null)
        {
            SetModelStatus("모델 없음", Theme.Danger);
            SetStatus($"models 폴더에 모델이 없습니다: {_modelsDirectory}");
            return;
        }
        await LoadModelAsync(first);
    }

    /// <summary>저장하지 않은 작업이 있으면 종료 전에 확인한다. (저장 여부는 추적하지 않으므로 작업이 있으면 묻는다)</summary>
    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_editBusy)
        {
            e.Cancel = true;
            SetStatus("작업이 끝난 뒤 종료하세요.");
            return;
        }
        int pending = lstImages.Items.Cast<ImageItem>().Count(i => i.HasWork);
        if (pending == 0) return;
        var answer = MessageBox.Show(this, $"작업한 이미지가 {pending}개 있습니다.\n저장하지 않은 작업은 사라집니다. 종료하시겠습니까?",
            Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (answer != DialogResult.Yes) e.Cancel = true;
    }

    private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        _closing.Cancel();
        _prefs.BrushSize = trkBrush.Value;
        _prefs.BrushSquare = canvas.BrushSquare;
        _prefs.Save();
        _model?.Dispose();
        _inpainter?.Dispose();
        _detector?.Dispose();
        _displayImage?.Dispose();
        _overlay?.Dispose();
        _viewImage?.Dispose();
    }

    // ======================= 모델 =======================

    private void BuildModelMenu()
    {
        mnuSettingsModel.DropDownItems.Clear();
        if (_models.Count == 0)
        {
            mnuSettingsModel.DropDownItems.Add(new ToolStripMenuItem("(models 폴더에 모델 없음)") { Enabled = false });
            return;
        }
        foreach (var info in _models)
        {
            var item = new ToolStripMenuItem(info.IsSupported ? info.DisplayName : $"{info.DisplayName} (미지원 형식)")
            {
                Tag = info,
                Enabled = info.IsSupported,
                Checked = info == _modelInfo,
            };
            item.Click += async (_, _) => await LoadModelAsync(info);
            mnuSettingsModel.DropDownItems.Add(item);
        }
    }

    private async Task LoadModelAsync(ModelInfo info)
    {
        if (_modelLoading || _encodeRunning) return;
        _modelLoading = true;
        UpdateUi();
        SetStatus($"AI 모델 불러오는 중: {info.DisplayName} ...");
        SetModelStatus("모델 로딩 중", Theme.Warning);

        var old = _model;
        _model = null;
        _encodedImage = null;
        old?.Dispose();

        try
        {
            var device = _device;
            var sw = Stopwatch.StartNew();
            _model = await Task.Run(() => ModelCatalog.Load(info, device));
            _modelInfo = info;
            SetModelStatus($"{ShortName(info)} · {_model.DeviceDescription}", Theme.Success);
            SetStatus($"모델 준비 완료 ({sw.Elapsed.TotalSeconds:0.0}초). " + (_doc is null ? "이미지를 열거나 끌어다 놓으세요." : ""));
        }
        catch (Exception ex)
        {
            _modelInfo = null;
            SetModelStatus("모델 로드 실패", Theme.Danger);
            SetStatus("모델 로드 실패: " + ex.Message);
            MessageBox.Show(this, "AI 모델을 불러오지 못했습니다.\n\n" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _modelLoading = false;
            BuildModelMenu();
            UpdateUi();
        }

        if (_model is not null && _doc is not null) StartEncode();
    }

    private static string ShortName(ModelInfo info)
    {
        // "Segment Anything (MobileSAM)" → "MobileSAM"
        int open = info.DisplayName.LastIndexOf('(');
        int close = info.DisplayName.LastIndexOf(')');
        return open >= 0 && close > open ? info.DisplayName[(open + 1)..close] : info.DisplayName;
    }

    private async void mnuDeviceAuto_Click(object? sender, EventArgs e) => await ChangeDeviceAsync(DevicePreference.Auto);

    private async void mnuDeviceCpu_Click(object? sender, EventArgs e) => await ChangeDeviceAsync(DevicePreference.CpuOnly);

    private async Task ChangeDeviceAsync(DevicePreference device)
    {
        if (_device == device || _modelLoading || _encodeRunning || _editBusy) return;
        _device = device;
        mnuDeviceAuto.Checked = device == DevicePreference.Auto;
        mnuDeviceCpu.Checked = device == DevicePreference.CpuOnly;
        // 지우기 모델도 다음 사용 때 새 장치로 다시 만든다
        _inpainter?.Dispose();
        _inpainter = null;
        _detector?.Dispose();
        _detector = null;
        if (_modelInfo is not null) await LoadModelAsync(_modelInfo);
    }

    // ======================= 이미지 목록 =======================

    private void mnuFileOpen_Click(object? sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog { Filter = BitmapConverter.OpenFilter, Title = "이미지 열기 (여러 개 선택 가능)", Multiselect = true };
        if (dlg.ShowDialog(this) == DialogResult.OK) AddImages(dlg.FileNames);
    }

    private void mnuFileAddFolder_Click(object? sender, EventArgs e)
    {
        using var dlg = new FolderBrowserDialog { Description = "이미지가 들어 있는 폴더를 선택하세요 (하위 폴더 포함)", UseDescriptionForTitle = true };
        if (dlg.ShowDialog(this) == DialogResult.OK) AddImages(new[] { dlg.SelectedPath });
    }

    private void mnuFilePaste_Click(object? sender, EventArgs e)
    {
        if (_editBusy)
        {
            SetStatus("작업이 끝난 뒤 붙여넣으세요.");
            return;
        }
        try
        {
            if (Clipboard.ContainsFileDropList())
            {
                AddImages(Clipboard.GetFileDropList().Cast<string>());
                return;
            }
            using var bmp = GetClipboardImage();
            if (bmp is not null)
            {
                AddBitmapItem(bmp, "붙여넣은 이미지");
                return;
            }
            SetStatus("클립보드에 이미지가 없습니다.");
        }
        catch (Exception ex)
        {
            SetStatus("붙여넣기 실패: " + ex.Message);
        }
    }

    /// <summary>파일 / 폴더, 또는 이미지 데이터(브라우저·메신저에서 끌어온 그림)를 받는다.</summary>
    private void MainForm_DragEnter(object? sender, DragEventArgs e)
    {
        var data = e.Data;
        bool ok = data is not null &&
                  (data.GetDataPresent(DataFormats.FileDrop) || data.GetDataPresent("PNG") || data.GetDataPresent(DataFormats.Bitmap) || data.GetDataPresent(DataFormats.Dib));
        e.Effect = ok && !_editBusy ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void MainForm_DragDrop(object? sender, DragEventArgs e)
    {
        var data = e.Data;
        if (data is null) return;
        if (data.GetData(DataFormats.FileDrop) is string[] { Length: > 0 } paths)
        {
            AddImages(paths);
            return;
        }
        using var bmp = ReadImageData(data);
        if (bmp is not null) AddBitmapItem(bmp, "끌어온 이미지");
        else SetStatus("끌어다 놓은 항목에서 이미지를 찾지 못했습니다.");
    }

    /// <summary>클립보드 / 드롭 데이터에서 이미지를 읽는다. PNG(투명 유지) → 비트맵 순서.</summary>
    private static Bitmap? ReadImageData(IDataObject data)
    {
        try
        {
            if (data.GetDataPresent("PNG") && data.GetData("PNG") is MemoryStream png)
            {
                png.Position = 0;
                using var img = Image.FromStream(png);
                return BitmapConverter.ToArgbBitmap(img);
            }
            if (data.GetDataPresent(DataFormats.Bitmap) && data.GetData(DataFormats.Bitmap) is Image bitmap)
            {
                return BitmapConverter.ToArgbBitmap(bitmap);
            }
        }
        catch (Exception)
        {
            // 형식이 맞지 않는 데이터
        }
        return null;
    }

    /// <summary>파일이 없는 이미지(붙여넣기 / 끌어온 그림)를 목록에 새 항목으로 추가하고 연다.</summary>
    private void AddBitmapItem(Bitmap bmp, string name)
    {
        if (_editBusy) return;
        _pasteCounter++;
        var item = new ImageItem(null, $"{name} {_pasteCounter}")
        {
            Document = new EditorDocument(BitmapConverter.ToBuffer(bmp), null),
            Thumbnail = MakeThumbnail(bmp),
        };
        lstImages.Items.Add(item);
        UpdateImagesHeader();
        lstImages.SelectedItem = item;
    }

    /// <summary>파일 / 폴더(하위 포함)를 이미지 목록에 추가하고, 새로 추가된 첫 이미지를 연다.</summary>
    private void AddImages(IEnumerable<string> paths)
    {
        if (_editBusy)
        {
            SetStatus("작업이 끝난 뒤 이미지를 추가하세요.");
            return;
        }
        var files = new List<string>();
        foreach (var path in paths)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    files.AddRange(Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                        .Where(IsImageFile).OrderBy(f => f, StringComparer.CurrentCultureIgnoreCase));
                }
                else if (File.Exists(path) && IsImageFile(path))
                {
                    files.Add(path);
                }
            }
            catch (Exception ex)
            {
                SetStatus($"추가 실패: {path} ({ex.Message})");
            }
        }

        var existing = new HashSet<string>(lstImages.Items.Cast<ImageItem>().Where(i => i.Path is not null).Select(i => i.Path!), StringComparer.OrdinalIgnoreCase);
        var added = new List<ImageItem>();
        lstImages.BeginUpdate();
        foreach (var file in files)
        {
            if (!existing.Add(file)) continue;
            var item = new ImageItem(file, Path.GetFileName(file));
            lstImages.Items.Add(item);
            added.Add(item);
        }
        lstImages.EndUpdate();
        UpdateImagesHeader();

        if (added.Count == 0)
        {
            SetStatus(files.Count == 0 ? "추가할 이미지가 없습니다 (지원 형식: PNG, JPG, BMP, GIF, TIFF)." : "이미 목록에 있는 이미지입니다.");
            return;
        }

        SetStatus($"이미지 {added.Count}개를 목록에 추가했습니다.");
        _ = GenerateThumbnailsAsync(added);
        lstImages.SelectedItem = added[0];
    }

    private static bool IsImageFile(string path) =>
        ImageExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);

    /// <summary>목록 미리보기를 백그라운드에서 하나씩 만든다.</summary>
    private async Task GenerateThumbnailsAsync(IReadOnlyList<ImageItem> items)
    {
        foreach (var item in items)
        {
            if (_closing.IsCancellationRequested) return;
            if (item.Thumbnail is not null || item.Path is null) continue;
            var path = item.Path;
            var thumb = await Task.Run(() =>
            {
                try
                {
                    using var stream = new MemoryStream(File.ReadAllBytes(path));
                    using var img = Image.FromStream(stream, false, false);
                    return MakeThumbnail(img);
                }
                catch (Exception)
                {
                    return null;
                }
            });
            if (_closing.IsCancellationRequested) { thumb?.Dispose(); return; }
            item.Thumbnail = thumb;
            if (thumb is null) item.Error ??= "열 수 없는 파일";
            lstImages.Invalidate();
        }
    }

    private static Bitmap MakeThumbnail(Image image)
    {
        const int size = 48;
        var bmp = new Bitmap(size, size);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.Clear(Color.FromArgb(44, 44, 49));
        // 가운데를 정사각형으로 잘라 채우기
        float scale = Math.Max((float)size / image.Width, (float)size / image.Height);
        float w = image.Width * scale, h = image.Height * scale;
        g.DrawImage(image, (size - w) / 2, (size - h) / 2, w, h);
        return bmp;
    }

    private void lstImages_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lstImages.SelectedItem is ImageItem item) SwitchToItem(item);
        UpdateUi();
    }

    /// <summary>목록의 이미지로 전환. 작업이 없는 이전 문서는 메모리에서 내린다.</summary>
    private void SwitchToItem(ImageItem item)
    {
        if (item == _currentItem || _editBusy) return;

        var previous = _currentItem;
        if (previous is { Document: not null, Path: not null } && !previous.HasWork && !previous.Document.HasSelection)
        {
            previous.Document = null;
        }

        _currentItem = item;
        if (item.Document is null)
        {
            try
            {
                using var bmp = BitmapConverter.LoadImageFile(item.Path!);
                item.Document = new EditorDocument(BitmapConverter.ToBuffer(bmp), item.Path);
                item.Error = null;
            }
            catch (Exception ex)
            {
                item.Error = "열 수 없는 파일";
                lstImages.Invalidate();
                ShowDocument(null);
                SetStatus($"이미지를 열 수 없습니다: {item.DisplayName} ({ex.Message})");
                return;
            }
        }
        ShowDocument(item.Document);
    }

    private void ShowDocument(EditorDocument? doc)
    {
        CancelFloating();
        _doc = doc;
        _predictVersion++;
        canvas.OverlayImage = null;
        canvas.BaseImage = null;
        _overlay?.Dispose();
        _overlay = null;

        if (doc is null)
        {
            _displayImage?.Dispose();
            _displayImage = null;
            Text = "Nukki Studio";
            lblImageInfo.Text = "-";
            RefreshAll();
            return;
        }

        _overlay = OverlayRenderer.Create(doc.Image.Width, doc.Image.Height);
        UpdateDisplayImage();
        Text = $"Nukki Studio - {_currentItem?.DisplayName}";
        lblImageInfo.Text = $"{doc.Image.Width} × {doc.Image.Height}";

        canvas.BaseImage = null;
        RefreshAll();
        canvas.FitToWindow();

        if (_model is not null) StartEncode();
        else SetStatus(_modelLoading ? "모델 로딩이 끝나면 이미지 분석을 시작합니다." : "AI 모델이 없어 브러시로만 선택할 수 있습니다.");
    }

    /// <summary>현재 문서 이미지로 표시용 비트맵을 다시 만든다 (열기 / 지우기 / 실행 취소 후).</summary>
    private void UpdateDisplayImage()
    {
        if (_doc is null) return;
        // 새 비트맵을 캔버스에 먼저 넘긴 뒤 이전 것을 해제한다 (해제된 비트맵을 그리는 일이 없도록)
        var old = _displayImage;
        _displayImage = BitmapConverter.ToDisplayBitmap(_doc.Image);
        if (old is not null && ReferenceEquals(canvas.BaseImage, old)) canvas.BaseImage = _displayImage;
        old?.Dispose();
        _imageHasAlpha = _doc.HasTransparency();
    }

    /// <summary>객체 지우기 / 배경 지우기 / 실행 취소로 이미지가 바뀐 뒤 처리.</summary>
    private void OnImageChanged()
    {
        // 크기 조절(또는 그 되돌리기)로 크기가 바뀌었으면 오버레이를 새로 만들고 화면에 맞춘다
        bool resized = _doc is not null && _overlay is not null && (_overlay.Width != _doc.Image.Width || _overlay.Height != _doc.Image.Height);
        if (resized)
        {
            canvas.OverlayImage = null;
            _overlay!.Dispose();
            _overlay = OverlayRenderer.Create(_doc!.Image.Width, _doc.Image.Height);
            lblImageInfo.Text = $"{_doc.Image.Width} × {_doc.Image.Height}";
        }
        UpdateDisplayImage();
        RefreshAll();
        if (resized) canvas.FitToWindow();
        if (_model is not null) StartEncode();
    }

    private void btnRemoveImage_Click(object? sender, EventArgs e)
    {
        if (lstImages.SelectedItem is not ImageItem item || _editBusy) return;
        if (item.HasWork &&
            MessageBox.Show(this, $"'{item.DisplayName}'에 저장하지 않은 작업이 있습니다.\n목록에서 빼시겠습니까?", Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        int index = lstImages.SelectedIndex;
        lstImages.Items.RemoveAt(index);
        item.Thumbnail?.Dispose();
        if (item == _currentItem)
        {
            _currentItem = null;
            if (lstImages.Items.Count > 0) lstImages.SelectedIndex = Math.Min(index, lstImages.Items.Count - 1);
            else ShowDocument(null);
        }
        UpdateImagesHeader();
        UpdateUi();
    }

    private void SelectAdjacentImage(int delta)
    {
        if (lstImages.Items.Count == 0) return;
        int index = lstImages.SelectedIndex < 0 ? 0 : Math.Clamp(lstImages.SelectedIndex + delta, 0, lstImages.Items.Count - 1);
        lstImages.SelectedIndex = index;
    }

    private void UpdateImagesHeader()
    {
        int count = lstImages.Items.Count;
        cardImages.Title = count == 0 ? "이미지" : $"이미지 ({count})";
        lblImagesEmpty.Visible = count == 0;
    }

    /// <summary>이미지 목록 한 줄: 미리보기 + 파일 이름 + 작업 상태.</summary>
    private void lstImages_DrawItem(object? sender, DrawItemEventArgs e)
    {
        var g = e.Graphics;
        using (var back = new SolidBrush(lstImages.BackColor)) g.FillRectangle(back, e.Bounds);
        if (e.Index < 0 || e.Index >= lstImages.Items.Count || lstImages.Items[e.Index] is not ImageItem item) return;

        g.SmoothingMode = SmoothingMode.AntiAlias;
        bool selected = (e.State & DrawItemState.Selected) != 0;
        var row = new RectangleF(e.Bounds.X, e.Bounds.Y + 2, e.Bounds.Width - 1, e.Bounds.Height - 4);
        if (selected)
        {
            using var path = Theme.RoundRect(row, 6);
            using var fill = new SolidBrush(Theme.Raised);
            g.FillPath(fill, path);
            using var bar = new SolidBrush(Theme.Accent);
            g.FillRectangle(bar, row.X, row.Y + 10, 3, row.Height - 20);
        }

        // 미리보기
        var thumbRect = new RectangleF(row.X + 10, row.Y + (row.Height - 40) / 2, 40, 40);
        using (var clip = Theme.RoundRect(thumbRect, 5))
        {
            if (item.Thumbnail is not null)
            {
                var state = g.Save();
                g.SetClip(clip);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(item.Thumbnail, thumbRect);
                g.Restore(state);
            }
            else
            {
                using var ph = new SolidBrush(Theme.Card);
                g.FillPath(ph, clip);
            }
        }

        // 이름 + 상태
        int textX = (int)thumbRect.Right + 10;
        var nameRect = new Rectangle(textX, e.Bounds.Y + 9, e.Bounds.Right - textX - 6, 20);
        TextRenderer.DrawText(g, item.DisplayName, lstImages.Font, nameRect, Theme.Text, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

        string status;
        Color statusColor;
        if (item.Error is not null) { status = item.Error; statusColor = Theme.Danger; }
        else if (item.Document is { } d && (d.Objects.Count > 0 || d.IsImageEdited))
        {
            var parts = new List<string>();
            if (d.Objects.Count > 0) parts.Add($"객체 {d.Objects.Count}");
            if (d.IsImageEdited) parts.Add("편집됨");
            status = "● " + string.Join(" · ", parts);
            statusColor = Theme.Success;
        }
        else { status = item == _currentItem ? "작업 중" : "작업 없음"; statusColor = Theme.TextFaint; }
        var statusRect = new Rectangle(textX, e.Bounds.Y + 29, e.Bounds.Right - textX - 6, 18);
        TextRenderer.DrawText(g, status, lstImages.Font, statusRect, statusColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
    }

    // ======================= 인코딩 / 예측 =======================

    /// <summary>현재 이미지를 인코딩한다. 실행 중이면 끝난 뒤 최신 이미지로 다시 실행한다.</summary>
    private async void StartEncode()
    {
        if (_encodeRunning) { _encodePending = true; return; }
        _encodeRunning = true;
        UpdateUi();
        try
        {
            while (true)
            {
                _encodePending = false;
                var doc = _doc;
                var model = _model;
                if (doc is null || model is null || _encodedImage == doc.Image) break;

                var image = doc.Image;
                SetStatus("이미지 분석 중... (AI 인코딩)");
                var sw = Stopwatch.StartNew();
                try
                {
                    await model.SetImageAsync(image);
                }
                catch (Exception ex)
                {
                    SetStatus("이미지 분석 실패: " + ex.Message);
                    break;
                }

                if (model != _model) continue;
                _encodedImage = image;
                SetModelStatus($"{(_modelInfo is null ? "" : ShortName(_modelInfo))} · {model.DeviceDescription}", Theme.Success);
                if (_doc == doc && doc.Image == image && !_encodePending)
                {
                    SetStatus($"분석 완료 ({sw.Elapsed.TotalSeconds:0.00}초) — 객체를 클릭하세요. 좌클릭: 포함 / 우클릭: 제외 / Enter: 확정 / Del: 지우기");
                    break;
                }
            }
        }
        finally
        {
            _encodeRunning = false;
            UpdateUi();
        }

        // 분석 중에 찍은 점이 있으면 지금 반영
        if (_doc is not null && !_doc.Prompts.IsEmpty && _encodedImage == _doc.Image) await RunPredictionAsync();
    }

    private async Task RunPredictionAsync()
    {
        var doc = _doc;
        var model = _model;
        if (doc is null) return;

        var prompts = doc.Prompts;
        if (prompts.IsEmpty)
        {
            doc.ApplyPrediction(null);
            RefreshSelection();
            return;
        }

        if (model is null || _encodedImage != doc.Image)
        {
            RefreshSelection();
            SetStatus(model is null ? "AI 모델이 없습니다." : "이미지 분석이 끝나면 선택이 반영됩니다...");
            return;
        }

        int version = ++_predictVersion;
        var previous = doc.LowResLogits;
        try
        {
            var sw = Stopwatch.StartNew();
            bool cleanup = chkCleanup.Checked;
            var result = await Task.Run(() => model.Predict(prompts, previous));
            if (version != _predictVersion || doc != _doc) return;

            doc.ApplyPrediction(result);
            // 정리(조각 제거)도 백그라운드에서 미리 계산
            await Task.Run(() => doc.GetCleanAiMask(cleanup));
            if (version != _predictVersion || doc != _doc) return;

            RefreshSelection();
            SetStatus($"선택 갱신 ({sw.ElapsedMilliseconds} ms, 예측 점수 {Math.Clamp(result.Score, 0f, 1f):0.00}) — Enter: 확정 / Del: 지우기 / Ctrl+B: 배경 지우기");
        }
        catch (Exception ex)
        {
            SetStatus("선택 계산 실패: " + ex.Message);
        }
    }

    // ======================= 캔버스 입력 =======================

    private async void canvas_PointPrompt(object? sender, PointPromptEventArgs e)
    {
        if (_doc is null || _editBusy) return;
        _doc.AddPoint(new PromptPoint(e.ImagePoint.X, e.ImagePoint.Y, e.Positive));
        UpdatePromptDisplay();
        await RunPredictionAsync();
        UpdateUi();
    }

    private async void canvas_BoxPrompt(object? sender, BoxPromptEventArgs e)
    {
        if (_doc is null || _editBusy) return;
        _doc.SetBox(e.ImageBox);
        UpdatePromptDisplay();
        await RunPredictionAsync();
        UpdateUi();
    }

    private void canvas_BrushStroke(object? sender, BrushStrokeEventArgs e)
    {
        if (_doc is null || _overlay is null || _editBusy) return;

        if (e.Phase == BrushPhase.Begin)
        {
            _predictVersion++; // 진행 중인 예측 결과가 브러시 위에 덮이지 않도록
            _doc.BeginBrushStroke();
        }

        var dirty = _doc.PaintBrush(e.From, e.To, canvas.BrushRadius, e.Add, canvas.BrushSquare);

        if (e.Phase == BrushPhase.End)
        {
            RefreshSelection();
            UpdateUi();
            return;
        }

        // 획 진행 중: 칠한 영역만 오버레이 갱신
        if (_view == ViewMode.Overlay)
        {
            var region = Rectangle.Inflate(dirty, 2, 2);
            OverlayRenderer.Render(_overlay, region, _doc.Objects, CurrentSelectionSource());
            canvas.Invalidate();
        }
    }

    private OverlayRenderer.SelectionSource CurrentSelectionSource()
    {
        if (_doc is null) return default;
        return new OverlayRenderer.SelectionSource(_doc.GetCleanAiMask(chkCleanup.Checked), _doc.Brush);
    }

    // ======================= 화면 갱신 =======================

    private void RefreshAll()
    {
        RefreshObjectList();
        RefreshSelection();
        UpdateUi();
    }

    /// <summary>선택 / 객체 변경 후 오버레이와 보기 이미지를 다시 만든다.</summary>
    private void RefreshSelection()
    {
        UpdatePromptDisplay();
        if (_doc is null || _overlay is null || _displayImage is null)
        {
            canvas.BaseImage = null;
            canvas.OverlayImage = null;
            UpdateUi();
            return;
        }

        if (_view == ViewMode.Overlay)
        {
            OverlayRenderer.Render(_overlay, new Rectangle(0, 0, _overlay.Width, _overlay.Height), _doc.Objects, CurrentSelectionSource());
            canvas.ShowCheckerboard = _imageHasAlpha;
            canvas.BaseImage = _displayImage;
            canvas.OverlayImage = _overlay;
        }
        else
        {
            var combined = BuildCombinedMask();
            var old = _viewImage;
            if (_view == ViewMode.Cutout)
            {
                var buffer = Compositor.Cutout(_doc.Image, combined);
                _viewImage = BitmapConverter.ToDisplayBitmap(buffer);
                canvas.ShowCheckerboard = true;
            }
            else
            {
                _viewImage = OverlayRenderer.MaskToBitmap(combined);
                canvas.ShowCheckerboard = false;
            }
            canvas.OverlayImage = null;
            canvas.BaseImage = _viewImage;
            old?.Dispose();
        }
        UpdateUi();
    }

    /// <summary>보이는 객체 + 현재 선택(보정 적용)의 합집합.</summary>
    private Mask BuildCombinedMask()
    {
        var doc = _doc!;
        var masks = doc.Objects.Where(o => o.Visible).Select(o => o.Mask).ToList();
        var current = doc.BuildFinalMask(CurrentRefine());
        if (current is not null) masks.Add(current);
        return MaskOps.Union(doc.Image.Width, doc.Image.Height, masks);
    }

    private void UpdatePromptDisplay()
    {
        canvas.Points = _doc?.Points ?? Array.Empty<PromptPoint>();
        canvas.Box = _doc?.Box;
        canvas.Invalidate();
    }

    private void RefreshObjectList()
    {
        lstObjects.BeginUpdate();
        try
        {
            lstObjects.Items.Clear();
            if (_doc is null) return;
            foreach (var obj in _doc.Objects) lstObjects.Items.Add(obj);
        }
        finally
        {
            lstObjects.EndUpdate();
            lblObjectsEmpty.Visible = lstObjects.Items.Count == 0;
        }
    }

    private RefineSettings CurrentRefine() => new(chkCleanup.Checked, trkGrow.Value, trkFeather.Value);

    private void UpdateUi()
    {
        bool hasDoc = _doc is not null;
        bool hasSelection = _doc?.HasSelection == true;
        bool hasObjects = _doc?.Objects.Count > 0;
        bool busy = _modelLoading || _encodeRunning;
        bool edit = !_editBusy;

        btnUndo.Enabled = mnuEditUndo.Enabled = edit && _doc?.CanUndo == true;
        btnRedo.Enabled = mnuEditRedo.Enabled = edit && _doc?.CanRedo == true;
        btnCommit.Enabled = mnuEditCommit.Enabled = edit && hasSelection;
        btnClearSelection.Enabled = mnuEditClear.Enabled = edit && (hasSelection || (_doc?.Points.Count > 0) || _doc?.Box is not null);
        btnDeleteObject.Enabled = edit && lstObjects.SelectedItem is not null;
        bool canExport = hasSelection || hasObjects;
        btnSavePng.Enabled = btnSave.Enabled = mnuFileSave.Enabled = canExport;
        btnCopy.Enabled = mnuFileCopy.Enabled = canExport;
        btnSaveAll.Enabled = mnuFileSaveAll.Enabled = hasObjects;
        btnSaveImage.Enabled = mnuFileSaveImage.Enabled = hasDoc;
        btnAutoDetect.Enabled = mnuEditAutoDetect.Enabled = edit && hasDoc && !busy && _model is not null;
        btnEraseObject.Enabled = mnuEditErase.Enabled = mnuEditCut.Enabled = edit && (hasSelection || lstObjects.SelectedItem is not null || _doc?.Objects.Count == 1);
        mnuEditPaste.Enabled = edit;
        mnuEditResize.Enabled = edit && hasDoc;
        cboInpaintModel.Enabled = edit;
        btnRemoveBackground.Enabled = mnuEditRemoveBg.Enabled = edit && canExport;
        btnFit.Enabled = btnActual.Enabled = mnuViewFit.Enabled = mnuViewActual.Enabled = hasDoc;
        mnuSettingsModel.Enabled = mnuSettingsDevice.Enabled = !busy && edit;
        btnRemoveImage.Enabled = edit && lstImages.SelectedItem is not null;
        btnSaveBatch.Enabled = mnuFileSaveBatch.Enabled = edit && lstImages.Items.Cast<ImageItem>().Any(i => i.HasWork);
        lstImages.Enabled = edit;

        btnToolPoint.Checked = canvas.Tool == CanvasTool.Point;
        btnToolBox.Checked = canvas.Tool == CanvasTool.Box;
        btnToolBrushAdd.Checked = canvas.Tool is CanvasTool.BrushAdd or CanvasTool.BrushErase;

        mnuViewOverlay.Checked = _view == ViewMode.Overlay;
        mnuViewCutout.Checked = _view == ViewMode.Cutout;
        mnuViewMask.Checked = _view == ViewMode.Mask;

        lstImages.Invalidate(); // 작업 상태 표시 갱신
    }

    private void SetStatus(string text) => lblStatus.Text = text;

    private void SetModelStatus(string text, Color color)
    {
        lblModel.Text = "● " + text;
        lblModel.ForeColor = color;
    }

    // ======================= 편집 명령 =======================

    private void btnCommit_Click(object? sender, EventArgs e)
    {
        if (_doc is null || _editBusy) return;
        var obj = _doc.CommitSelection(CurrentRefine());
        if (obj is null)
        {
            SetStatus("확정할 선택이 없습니다.");
            return;
        }
        _predictVersion++;
        RefreshObjectList();
        RefreshSelection();
        SetStatus($"'{obj.Name}' 확정. 다음 객체를 클릭하거나 저장하세요.");
    }

    private void btnClearSelection_Click(object? sender, EventArgs e)
    {
        if (_doc is null || _editBusy) return;
        _predictVersion++;
        _doc.ClearSelection();
        RefreshSelection();
    }

    private void btnDeleteObject_Click(object? sender, EventArgs e)
    {
        if (_doc is null || _editBusy || lstObjects.SelectedItem is not SegmentedObject obj) return;
        _doc.RemoveObject(obj);
        RefreshObjectList();
        RefreshSelection();
    }

    private void btnUndo_Click(object? sender, EventArgs e)
    {
        if (_doc is null || _editBusy) return;
        var before = _doc.Image;
        if (!_doc.Undo()) return;
        AfterHistoryChange(before);
    }

    private void btnRedo_Click(object? sender, EventArgs e)
    {
        if (_doc is null || _editBusy) return;
        var before = _doc.Image;
        if (!_doc.Redo()) return;
        AfterHistoryChange(before);
    }

    private void AfterHistoryChange(ImageBuffer imageBefore)
    {
        _predictVersion++;
        if (!ReferenceEquals(imageBefore, _doc!.Image))
        {
            OnImageChanged();
            SetStatus("이미지 편집을 되돌렸습니다.");
        }
        else
        {
            RefreshObjectList();
            RefreshSelection();
        }
    }

    // ---------------- 객체 자동 선택 ----------------

    /// <summary>
    /// 객체 자동 선택: D-FINE으로 객체 상자를 찾고, 상자마다 SAM 박스 프롬프트로 정확한 윤곽을 만들어 객체 목록에 추가한다.
    /// 버튼(또는 A 키)을 눌렀을 때만 실행한다.
    /// </summary>
    private async void btnAutoDetect_Click(object? sender, EventArgs e)
    {
        if (_doc is null || _model is null || _editBusy || _encodeRunning) return;
        var detectorPath = Path.Combine(_modelsDirectory, "dfine", ObjectDetector.ModelFileName);
        if (!File.Exists(detectorPath))
        {
            SetStatus($"객체 감지 모델이 없습니다: models\\dfine\\{ObjectDetector.ModelFileName}");
            return;
        }

        var doc = _doc;
        var model = _model;
        var image = doc.Image;
        var refine = CurrentRefine();
        SetEditBusy(true, "객체를 찾는 중... (AI 객체 감지)");
        try
        {
            var sw = Stopwatch.StartNew();
            _detector ??= await Task.Run(() => new ObjectDetector(detectorPath));
            var detector = _detector;
            var detections = await Task.Run(() => detector.Detect(image));
            if (detections.Count == 0)
            {
                SetStatus("자동으로 찾은 객체가 없습니다 — 객체를 직접 클릭해서 선택하세요.");
                return;
            }

            // 인코딩이 이 이미지 기준인지 확인 (아니면 기다렸다가 진행)
            if (_encodedImage != image) await model.SetImageAsync(image);
            _encodedImage = image;

            SetStatus($"객체 {detections.Count}개 발견 — 윤곽을 다듬는 중...");
            var found = await BusyForm.RunAsync(this, $"객체 {detections.Count}개 윤곽을 다듬는 중...", "찾은 객체마다 정확한 윤곽을 계산하고 있습니다.", () => Task.Run(() =>
            {
                var list = new List<(string, Mask)>();
                foreach (var d in detections)
                {
                    var result = model.Predict(new PromptSet(Array.Empty<PromptPoint>(), d.Box), null);
                    // 상자 밖으로 새어 나간 조각은 버린다
                    var mask = MaskOps.KeepPromptedRegions(result.Mask, Array.Empty<PointF>(), d.Box);
                    if (refine.Cleanup) mask = MaskOps.Cleanup(mask);
                    mask = MaskOps.Feather(MaskOps.Grow(mask, refine.Grow), refine.Feather);
                    list.Add((d.Label, mask));
                }
                return list;
            }));

            var added = doc.AddDetectedObjects(found);
            if (doc == _doc)
            {
                _predictVersion++;
                RefreshObjectList();
                RefreshSelection();
            }
            var names = string.Join(", ", added.Select(o => o.Name).Take(6)) + (added.Count > 6 ? " ..." : "");
            SetStatus($"객체 {added.Count}개를 찾았습니다 ({sw.Elapsed.TotalSeconds:0.0}초): {names} — 목록에서 골라 저장 / Del 지우기 / Ctrl+B 배경 지우기");
        }
        catch (Exception ex)
        {
            SetStatus("객체 자동 선택 실패: " + ex.Message);
        }
        finally
        {
            SetEditBusy(false, null);
        }
    }

    // ---------------- 객체 지우기 / 배경 지우기 ----------------

    /// <summary>지우기 대상: 목록에서 선택한 객체 → 현재 선택 → 객체가 하나뿐이면 그 객체.</summary>
    private (Mask Mask, SegmentedObject? Object, string Name)? GetEditTarget()
    {
        if (_doc is null) return null;
        if (lstObjects.SelectedItem is SegmentedObject selected) return (selected.Mask, selected, selected.Name);
        var current = _doc.BuildFinalMask(CurrentRefine());
        if (current is not null && !current.IsEmpty()) return (current, null, "선택 영역");
        if (_doc.Objects.Count == 1) return (_doc.Objects[0].Mask, _doc.Objects[0], _doc.Objects[0].Name);
        return null;
    }

    /// <summary>선택한 지우기 모델 (LaMa / MI-GAN). 없거나 실패하면 다른 모델 → 기본 채우기 순서로 대신 쓴다.</summary>
    private async Task<IInpainter> GetInpainterAsync()
    {
        if (_inpainter is not null) return _inpainter;
        var lamaPath = LamaInpainter.FindModel(Path.Combine(_modelsDirectory, ModelPackages.Lama.Folder));
        var miganPath = Path.Combine(_modelsDirectory, ModelPackages.MiGan.Folder, MiGanInpainter.ModelFileName);
        bool preferLama = _prefs.InpaintModel == ModelPackages.LamaId;
        var device = _device;
        _inpainter = await Task.Run<IInpainter>(() =>
        {
            var order = preferLama ? new[] { "lama", "migan" } : new[] { "migan", "lama" };
            foreach (var id in order)
            {
                try
                {
                    if (id == "lama" && lamaPath is not null) return new LamaInpainter(lamaPath, device);
                    if (id == "migan" && File.Exists(miganPath)) return new MiGanInpainter(miganPath, device);
                }
                catch (Exception)
                {
                    // 다음 후보로
                }
            }
            return new DiffusionInpainter();
        });
        return _inpainter;
    }

    // ---------------- 지우기 모델 선택 / 모델 다운로드 ----------------

    /// <summary>오른쪽 위 지우기 모델 목록: 설치 여부 표시 + 현재 선택.</summary>
    private void RefreshInpaintCombo()
    {
        _syncingInpaintCombo = true;
        cboInpaintModel.Items.Clear();
        foreach (var p in new[] { ModelPackages.MiGan, ModelPackages.Lama })
        {
            string label = p == ModelPackages.MiGan ? "MI-GAN (빠름)" : "LaMa (고품질)";
            cboInpaintModel.Items.Add(p.IsInstalled(_modelsDirectory) ? label : $"{label} · 받기");
        }
        cboInpaintModel.SelectedIndex = _prefs.InpaintModel == ModelPackages.LamaId ? 1 : 0;
        _syncingInpaintCombo = false;
    }

    private void cboInpaintModel_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_syncingInpaintCombo || cboInpaintModel.SelectedIndex < 0) return;
        var package = cboInpaintModel.SelectedIndex == 1 ? ModelPackages.Lama : ModelPackages.MiGan;
        _prefs.InpaintModel = package.Id;
        _prefs.Save();
        _inpainter?.Dispose();
        _inpainter = null;

        if (!package.IsInstalled(_modelsDirectory))
        {
            ShowModelDownload(new[] { package.Id },
                $"{package.Title} 모델이 아직 없습니다 ({package.SizeText}). [다운로드]를 누르면 받아서 바로 사용합니다.\r\n받지 않으면 설치된 다른 지우기 모델(없으면 기본 채우기)을 씁니다.");
        }
        SetStatus($"지우기 모델: {package.Title}" + (package.IsInstalled(_modelsDirectory) ? "" : " (미설치 — 다른 방법으로 대신 지웁니다)"));
        canvas.Focus();
    }

    private void mnuSettingsDownload_Click(object? sender, EventArgs e) => ShowModelDownload(Array.Empty<string>(), null);

    /// <summary>모델 다운로드 창을 띄우고, 새로 설치되면 모델을 다시 불러온다.</summary>
    private void ShowModelDownload(string[] preselect, string? intro)
    {
        if (_editBusy) return;
        using var form = new ModelDownloadForm { ModelsDirectory = _modelsDirectory, PreselectIds = preselect, IntroText = intro };
        form.ShowDialog(this);
        if (!form.Installed) return;

        _inpainter?.Dispose();
        _inpainter = null;
        _detector?.Dispose();
        _detector = null;
        RefreshInpaintCombo();

        _models = ModelCatalog.Scan(_modelsDirectory);
        BuildModelMenu();
        // 실행 중에 처음으로 선택 모델이 생겼으면 바로 불러온다 (시작할 때는 Shown에서 불러옴)
        if (_startupDone && _model is null && !_modelLoading && _models.FirstOrDefault(m => m.IsSupported) is { } first)
        {
            _ = LoadModelAsync(first);
        }
        SetStatus("AI 모델을 설치했습니다.");
    }

    // ---------------- 이미지 크기 조절 ----------------

    private async void mnuEditResize_Click(object? sender, EventArgs e)
    {
        if (_doc is null || _editBusy) return;
        if (canvas.FloatingImage is not null) CancelFloating();
        var doc = _doc;
        using var form = new ImageResizeForm { OriginalSize = new Size(doc.Image.Width, doc.Image.Height) };
        if (form.ShowDialog(this) != DialogResult.OK) return;
        var size = form.ResultSize;
        if (size.Width == doc.Image.Width && size.Height == doc.Image.Height) return;

        SetEditBusy(true, $"이미지 크기 바꾸는 중... ({size.Width} × {size.Height})");
        try
        {
            // 계산은 백그라운드, 문서 변경은 UI 스레드에서
            var prepared = await BusyForm.RunAsync(this, "이미지 크기 바꾸는 중...", $"{doc.Image.Width} × {doc.Image.Height} → {size.Width} × {size.Height}",
                () => Task.Run(() => doc.PrepareResize(size.Width, size.Height)));
            doc.ApplyResize(prepared);
            if (doc == _doc) OnImageChanged();
            SetStatus($"이미지 크기를 {size.Width} × {size.Height}(으)로 바꿨습니다 — 되돌리기: Ctrl+Z");
        }
        catch (Exception ex)
        {
            SetStatus("크기 조절 실패: " + ex.Message);
        }
        finally
        {
            SetEditBusy(false, null);
        }
    }

    private async void btnEraseObject_Click(object? sender, EventArgs e) => await EraseTargetAsync();

    /// <summary>지우기 대상(선택 객체 / 현재 선택)을 지우고 배경으로 채운다. 지웠으면 true.</summary>
    private async Task<bool> EraseTargetAsync()
    {
        if (_doc is null || _editBusy) return false;
        var target = GetEditTarget();
        if (target is null)
        {
            SetStatus("지울 객체를 클릭해서 선택하거나 객체 목록에서 고르세요.");
            return false;
        }

        var doc = _doc;
        var image = doc.Image;
        var (mask, obj, name) = target.Value;
        SetEditBusy(true, $"'{name}' 지우는 중... (주변 배경으로 채우기)");
        try
        {
            var sw = Stopwatch.StartNew();
            string modelName = "";
            string detail = _prefs.InpaintModel == ModelPackages.LamaId
                ? "고품질 지우기 (LaMa) — 주변 배경으로 자연스럽게 채우는 중입니다. 보통 2~5초 걸립니다."
                : "주변 배경으로 채우는 중입니다.";
            // 0.3초 넘게 걸리면 "지우는 중" 창을 띄우고, 끝나면 자동으로 닫는다
            var result = await BusyForm.RunAsync(this, $"'{name}' 지우는 중...", detail, async () =>
            {
                var inpainter = await GetInpainterAsync();
                modelName = inpainter.Name;
                return await Task.Run(() => inpainter.Inpaint(image, mask));
            });
            doc.ReplaceImage(result, obj);
            if (doc == _doc) OnImageChanged();
            SetStatus($"'{name}' 지움 ({sw.ElapsedMilliseconds} ms, {modelName}) — 마음에 들지 않으면 Ctrl+Z");
            return true;
        }
        catch (Exception ex)
        {
            SetStatus("객체 지우기 실패: " + ex.Message);
            return false;
        }
        finally
        {
            SetEditBusy(false, null);
        }
    }

    // ---------------- 잘라내기 / 붙여넣기 ----------------

    /// <summary>Ctrl+X: 선택 객체를 클립보드에 복사하고 이미지에서 지운다 (배경으로 채움).</summary>
    private async void mnuEditCut_Click(object? sender, EventArgs e)
    {
        if (_doc is null || _editBusy) return;
        if (GetExportTarget() is null)
        {
            SetStatus("잘라낼 객체를 클릭해서 선택하거나 객체 목록에서 고르세요.");
            return;
        }
        btnCopy_Click(sender, e);
        if (await EraseTargetAsync()) SetStatus("잘라냈습니다 — 다른 위치나 다른 이미지에서 Ctrl+V로 붙여넣을 수 있습니다.");
    }

    /// <summary>Ctrl+V: 현재 이미지 위에 클립보드 이미지를 떠 있는 상태로 붙여넣는다. 이미지가 없으면 새 이미지로 연다.</summary>
    private void mnuEditPaste_Click(object? sender, EventArgs e)
    {
        if (_editBusy) return;
        if (_doc is null)
        {
            mnuFilePaste_Click(sender, e);
            return;
        }
        if (Clipboard.ContainsFileDropList())
        {
            AddImages(Clipboard.GetFileDropList().Cast<string>());
            return;
        }

        var bmp = GetClipboardImage();
        if (bmp is null)
        {
            SetStatus("클립보드에 이미지가 없습니다. 객체를 Ctrl+C / Ctrl+X로 복사한 뒤 붙여넣으세요.");
            return;
        }

        // 이미지보다 크면 90% 안에 들어가게 줄이고 가운데에 놓는다
        float scale = Math.Min(1f, Math.Min(_doc.Image.Width * 0.9f / bmp.Width, _doc.Image.Height * 0.9f / bmp.Height));
        float w = bmp.Width * scale, h = bmp.Height * scale;
        canvas.FloatingImage?.Dispose();
        canvas.FloatingRect = new RectangleF((_doc.Image.Width - w) / 2, (_doc.Image.Height - h) / 2, w, h);
        canvas.FloatingImage = bmp;
        canvas.Focus();
        SetStatus("붙여넣기 위치를 정하세요 — 드래그: 이동 / Ctrl+휠: 크기 / Enter·더블클릭: 확정 / Esc: 취소");
    }

    private static Bitmap? GetClipboardImage()
    {
        try
        {
            var data = Clipboard.GetDataObject();
            return data is null ? null : ReadImageData(data);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>떠 있는 붙여넣기 이미지를 현재 이미지에 합친다 (실행 취소 가능).</summary>
    private void CommitFloating()
    {
        if (_doc is null || canvas.FloatingImage is not { } floating) return;
        try
        {
            using var baseBmp = BitmapConverter.ToBitmap(_doc.Image);
            using (var g = Graphics.FromImage(baseBmp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(floating, canvas.FloatingRect);
            }
            _doc.ReplaceImage(BitmapConverter.ToBuffer(baseBmp));
            CancelFloating();
            OnImageChanged();
            SetStatus("붙여넣었습니다 — 되돌리기: Ctrl+Z. 저장: '편집한 이미지 저장'(Ctrl+Alt+S)");
        }
        catch (Exception ex)
        {
            SetStatus("붙여넣기 실패: " + ex.Message);
        }
    }

    private void CancelFloating()
    {
        var old = canvas.FloatingImage;
        canvas.FloatingImage = null;
        old?.Dispose();
    }

    private void btnRemoveBackground_Click(object? sender, EventArgs e)
    {
        if (_doc is null || _editBusy) return;
        // 목록에서 객체를 골랐으면 그 객체만, 아니면 보이는 객체 전체 + 현재 선택을 남긴다
        string kept;
        Mask keep;
        if (lstObjects.SelectedItem is SegmentedObject selected)
        {
            keep = selected.Mask;
            kept = $"'{selected.Name}'만";
        }
        else
        {
            keep = BuildCombinedMask();
            kept = "선택한 객체만";
        }
        if (keep.IsEmpty())
        {
            SetStatus("남길 객체를 먼저 선택하세요 ('객체 자동 선택'(A) 또는 클릭). 선택한 객체만 남고 나머지가 투명해집니다.");
            return;
        }
        _doc.ReplaceImage(EditorDocument.RemoveBackground(_doc.Image, keep));
        OnImageChanged();
        SetStatus($"배경을 지웠습니다 ({kept} 남김) — '편집한 이미지 저장'(Ctrl+Alt+S)으로 저장하세요. 되돌리기: Ctrl+Z");
    }

    private void SetEditBusy(bool busy, string? status)
    {
        _editBusy = busy;
        UseWaitCursor = busy;
        if (status is not null) SetStatus(status);
        UpdateUi();
    }

    // ---------------- 객체 목록 ----------------

    private const int EyeAreaWidth = 36;

    /// <summary>객체 목록 한 줄: 색 견본 + 이름 + 눈 아이콘(표시 / 숨김).</summary>
    private void lstObjects_DrawItem(object? sender, DrawItemEventArgs e)
    {
        var g = e.Graphics;
        using (var back = new SolidBrush(lstObjects.BackColor)) g.FillRectangle(back, e.Bounds);
        if (e.Index < 0 || e.Index >= lstObjects.Items.Count || lstObjects.Items[e.Index] is not SegmentedObject obj) return;

        g.SmoothingMode = SmoothingMode.AntiAlias;
        bool selected = (e.State & DrawItemState.Selected) != 0;
        var row = new RectangleF(e.Bounds.X, e.Bounds.Y + 2, e.Bounds.Width - 1, e.Bounds.Height - 4);
        if (selected)
        {
            using var path = Theme.RoundRect(row, 6);
            using var fill = new SolidBrush(Theme.Raised);
            g.FillPath(fill, path);
            using var bar = new SolidBrush(Theme.Accent);
            g.FillRectangle(bar, row.X, row.Y + 7, 3, row.Height - 14);
        }

        // 색 견본
        float cy = row.Y + row.Height / 2;
        var swatch = new RectangleF(row.X + 12, cy - 7, 14, 14);
        using (var path = Theme.RoundRect(swatch, 4))
        using (var fill = new SolidBrush(obj.Visible ? obj.Color : Color.FromArgb(90, obj.Color)))
            g.FillPath(fill, path);

        var textRect = new Rectangle((int)swatch.Right + 10, e.Bounds.Y, e.Bounds.Width - (int)swatch.Right - 10 - EyeAreaWidth, e.Bounds.Height);
        TextRenderer.DrawText(g, obj.Name, lstObjects.Font, textRect, obj.Visible ? Theme.Text : Theme.TextFaint,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

        // 눈 아이콘
        var state = g.Save();
        g.TranslateTransform(e.Bounds.Right - EyeAreaWidth + 9, cy - 9);
        g.ScaleTransform(18 / 24f, 18 / 24f);
        IconFactory.Draw(g, obj.Visible ? AppIcon.Eye : AppIcon.EyeOff, obj.Visible ? Theme.TextDim : Theme.TextFaint);
        g.Restore(state);
    }

    private void lstObjects_MouseDown(object? sender, MouseEventArgs e)
    {
        int index = lstObjects.IndexFromPoint(e.Location);
        if (index < 0 || e.X < lstObjects.ClientSize.Width - EyeAreaWidth) return;
        ToggleObjectVisibility(index);
    }

    private void ToggleObjectVisibility(int index)
    {
        if (lstObjects.Items[index] is not SegmentedObject obj) return;
        obj.Visible = !obj.Visible;
        lstObjects.Invalidate(lstObjects.GetItemRectangle(index));
        RefreshSelection();
    }

    private void lstObjects_SelectedIndexChanged(object? sender, EventArgs e) => UpdateUi();

    private void lstObjects_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space && lstObjects.SelectedIndex >= 0)
        {
            ToggleObjectVisibility(lstObjects.SelectedIndex);
            e.Handled = true;
        }
    }

    // ======================= 도구 / 보기 =======================

    private void SetTool(CanvasTool tool)
    {
        canvas.Tool = tool;
        canvas.Invalidate();
        UpdateUi();
    }

    private void btnToolPoint_Click(object? sender, EventArgs e) => SetTool(CanvasTool.Point);

    private void btnToolBox_Click(object? sender, EventArgs e) => SetTool(CanvasTool.Box);

    /// <summary>브러시: 칠한 영역이 선택이 된다 (오른쪽 버튼 드래그는 칠한 것 지우기). 칠한 뒤 원하는 기능을 누른다.</summary>
    private void btnToolBrushAdd_Click(object? sender, EventArgs e)
    {
        SetTool(CanvasTool.BrushAdd);
        SetStatus("브러시: 원하는 영역을 칠하세요 (오른쪽 버튼 드래그 = 칠한 부분 지우기) → Enter 확정 / Del 지우기 / Ctrl+B 배경 지우기 / Ctrl+S 저장");
    }

    private void SetView(ViewMode view)
    {
        _view = view;
        RefreshSelection();
    }

    private void mnuViewOverlay_Click(object? sender, EventArgs e) => SetView(ViewMode.Overlay);

    private void mnuViewCutout_Click(object? sender, EventArgs e) => SetView(ViewMode.Cutout);

    private void mnuViewMask_Click(object? sender, EventArgs e) => SetView(ViewMode.Mask);

    private void btnFit_Click(object? sender, EventArgs e) => canvas.FitToWindow();

    private void btnActual_Click(object? sender, EventArgs e) => canvas.ActualSize();

    /// <summary>브러시 크기를 한 단계 키우거나 줄인다 (+ / − 버튼, + / − / [ / ] 키). 크기에 비례한 간격.</summary>
    private void ChangeBrushSize(int direction)
    {
        int step = Math.Max(2, (int)Math.Round(trkBrush.Value * 0.1));
        trkBrush.Value = Math.Clamp(trkBrush.Value + direction * step, trkBrush.Minimum, trkBrush.Maximum);
        SetStatus($"브러시 크기: {trkBrush.Value} px ({(canvas.BrushSquare ? "네모" : "원형")})");
    }

    private void btnBrushShape_Click(object? sender, EventArgs e)
    {
        canvas.BrushSquare = !canvas.BrushSquare;
        UpdateBrushShapeIcon();
        canvas.Invalidate();
        SetStatus($"브러시 모양: {(canvas.BrushSquare ? "네모" : "원형")} — 크기 {trkBrush.Value} px");
    }

    private void UpdateBrushShapeIcon()
    {
        int iconSize = (int)Math.Round(18 * DeviceDpi / 96.0);
        var old = btnBrushShape.Image;
        btnBrushShape.Image = IconFactory.Create(canvas.BrushSquare ? AppIcon.ShapeSquare : AppIcon.ShapeCircle, iconSize, Theme.Text);
        old?.Dispose();
        btnBrushShape.ToolTipText = $"브러시 모양: {(canvas.BrushSquare ? "네모" : "원형")} (클릭하면 {(canvas.BrushSquare ? "원형" : "네모")}으로)";
    }

    private void trkBrush_ValueChanged(object? sender, EventArgs e)
    {
        canvas.BrushRadius = trkBrush.Value / 2f;
        UpdateRefineLabels();
        canvas.Invalidate();
    }

    private void trkRefine_ValueChanged(object? sender, EventArgs e)
    {
        UpdateRefineLabels();
        if (_view != ViewMode.Overlay) RefreshSelection();
    }

    private void chkCleanup_CheckedChanged(object? sender, EventArgs e) => RefreshSelection();

    private void UpdateRefineLabels()
    {
        lblBrushValue.Text = $"{trkBrush.Value} px";
        // 도구 모음 안의 컨트롤에는 ToolStrip 툴팁이 뜨지 않으므로 직접 지정
        _tips.SetToolTip(btnBrushSize.Stepper, $"브러시 크기 {trkBrush.Value} px — 위(+) 키우기 / 아래(−) 줄이기 (단축키 + / −, 누르고 있으면 계속)");
        lblFeatherValue.Text = trkFeather.Value == 0 ? "끔" : $"{trkFeather.Value} px";
        lblGrowValue.Text = trkGrow.Value == 0 ? "0" : $"{(trkGrow.Value > 0 ? "+" : "−")}{Math.Abs(trkGrow.Value)} px";
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        // Ctrl+Shift+Z: 다시 실행 (Ctrl+Y와 같음)
        if (e.Control && e.Shift && e.KeyCode == Keys.Z)
        {
            btnRedo_Click(sender, e);
            e.Handled = e.SuppressKeyPress = true;
            return;
        }
        if (e.Control || e.Alt) return;
        if (ActiveControl is TextBoxBase) return;

        // 붙여넣기 위치 정하는 중: Enter 확정 / Esc 취소
        if (canvas.FloatingImage is not null && e.KeyCode is Keys.Enter or Keys.Escape)
        {
            if (e.KeyCode == Keys.Enter) CommitFloating();
            else { CancelFloating(); SetStatus("붙여넣기를 취소했습니다."); }
            e.Handled = e.SuppressKeyPress = true;
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.Enter: btnCommit_Click(sender, e); break;
            case Keys.Escape: btnClearSelection_Click(sender, e); break;
            case Keys.A: btnAutoDetect_Click(sender, e); break;
            case Keys.Q: SetTool(CanvasTool.Point); break;
            case Keys.W: SetTool(CanvasTool.Box); break;
            case Keys.E: btnToolBrushAdd_Click(sender, e); break;
            case Keys.OemOpenBrackets or Keys.OemMinus or Keys.Subtract: ChangeBrushSize(-1); break;
            case Keys.OemCloseBrackets or Keys.Oemplus or Keys.Add: ChangeBrushSize(+1); break;
            case Keys.PageDown: SelectAdjacentImage(+1); break;
            case Keys.PageUp: SelectAdjacentImage(-1); break;
            default: return;
        }
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    // ======================= 도움말 =======================

    private void mnuHelpManual_Click(object? sender, EventArgs e)
    {
        using var help = new HelpForm();
        help.ShowDialog(this);
    }

    private void mnuHelpShortcuts_Click(object? sender, EventArgs e)
    {
        using var help = new HelpForm();
        help.SelectSection("단축키");
        help.ShowDialog(this);
    }

    private void mnuHelpAbout_Click(object? sender, EventArgs e)
    {
        using var about = new AboutForm();
        about.ShowDialog(this);
    }

    // ======================= 내보내기 =======================

    /// <summary>저장 / 복사 대상: 목록에서 선택한 객체 → 현재 선택 → 객체가 하나뿐이면 그 객체.</summary>
    private (Mask Mask, string Name)? GetExportTarget()
    {
        if (_doc is null) return null;
        if (lstObjects.SelectedItem is SegmentedObject selected) return (selected.Mask, selected.Name);
        var current = _doc.BuildFinalMask(CurrentRefine());
        if (current is not null && !current.IsEmpty()) return (current, "선택");
        if (_doc.Objects.Count == 1) return (_doc.Objects[0].Mask, _doc.Objects[0].Name);
        return null;
    }

    private readonly record struct CutoutOptions(bool Crop, Color? Background);

    private CutoutOptions CurrentCutoutOptions() => new(chkCrop.Checked, chkBackground.Checked ? _backgroundColor : null);

    private static Bitmap RenderCutout(EditorDocument doc, Mask mask, CutoutOptions options)
    {
        Rectangle? crop = null;
        if (options.Crop)
        {
            var bounds = mask.GetBounds();
            if (!bounds.IsEmpty) crop = Compositor.Inflate(bounds, 2, doc.Image.Width, doc.Image.Height);
        }
        return BitmapConverter.ToBitmap(Compositor.Cutout(doc.Image, mask, crop, options.Background));
    }

    /// <summary>편집한 이미지 전체. 배경색 옵션이 켜져 있으면 투명 부분을 그 색으로 채운다.</summary>
    private static Bitmap RenderEditedImage(EditorDocument doc, CutoutOptions options)
    {
        if (options.Background is null) return BitmapConverter.ToBitmap(doc.Image);
        var full = new Mask(doc.Image.Width, doc.Image.Height);
        Array.Fill(full.Data, (byte)255);
        return BitmapConverter.ToBitmap(Compositor.Cutout(doc.Image, full, null, options.Background));
    }

    private string BaseFileName() => _currentItem?.BaseName ?? "nukki";

    private void UpdateExportSummary() => lblExportSummary.Text = _exportSettings.Summary;

    /// <summary>내보내기 설정을 적용해 저장하고 결과 경로를 돌려준다.</summary>
    private string ExportBitmap(Bitmap bitmap, string? sourcePath, string baseName)
    {
        var folder = ImageExporter.ResolveFolder(_exportSettings, sourcePath);
        _lastOutputFolder = folder;
        return ImageExporter.Save(bitmap, folder, baseName, _exportSettings, chkBackground.Checked ? _backgroundColor : null);
    }

    private void btnSavePng_Click(object? sender, EventArgs e)
    {
        var target = GetExportTarget();
        if (_doc is null || target is null)
        {
            SetStatus("저장할 객체를 목록에서 선택하세요 ('객체 자동 선택'(A) 또는 클릭).");
            return;
        }

        try
        {
            using var bmp = RenderCutout(_doc, target.Value.Mask, CurrentCutoutOptions());
            var path = ExportBitmap(bmp, _currentItem?.Path, $"{BaseFileName()}_{target.Value.Name}");
            SetStatus($"저장 완료: {path}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "저장 실패: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnSaveAll_Click(object? sender, EventArgs e)
    {
        if (_doc is null || _doc.Objects.Count == 0)
        {
            SetStatus("확정된 객체가 없습니다. '객체 자동 선택'(A)을 누르거나 선택 후 Enter로 확정하세요.");
            return;
        }

        try
        {
            var options = CurrentCutoutOptions();
            foreach (var obj in _doc.Objects)
            {
                using var bmp = RenderCutout(_doc, obj.Mask, options);
                ExportBitmap(bmp, _currentItem?.Path, $"{BaseFileName()}_{obj.Name}");
            }
            SetStatus($"{_doc.Objects.Count}개 객체 저장 완료: {_lastOutputFolder}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "저장 실패: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnSaveImage_Click(object? sender, EventArgs e)
    {
        if (_doc is null) return;
        try
        {
            using var bmp = RenderEditedImage(_doc, CurrentCutoutOptions());
            var path = ExportBitmap(bmp, _currentItem?.Path, $"{BaseFileName()}_편집");
            SetStatus($"편집한 이미지 저장 완료: {path}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "저장 실패: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 목록의 모든 이미지 중 작업이 있는 것을 내보내기 설정대로 일괄 저장한다.
    /// 확정한 객체는 각각 누끼 이미지로, 지우기 편집이 있으면 편집한 이미지 전체도 저장한다.
    /// 저장 위치가 "원본 위치"이면 이미지마다 자기 폴더의 output 폴더에 저장된다.
    /// </summary>
    private async void btnSaveBatch_Click(object? sender, EventArgs e)
    {
        if (_editBusy) return;
        var items = lstImages.Items.Cast<ImageItem>().Where(i => i.HasWork).ToList();
        if (items.Count == 0)
        {
            SetStatus("저장할 작업이 없습니다. 객체를 확정하거나 지우기를 한 뒤 저장하세요.");
            return;
        }

        var settings = _exportSettings.Clone();
        var options = CurrentCutoutOptions();
        Color? opaque = chkBackground.Checked ? _backgroundColor : null;
        SetEditBusy(true, "일괄 저장 중...");
        int files = 0, done = 0;
        var folders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var errors = new List<string>();
        try
        {
            foreach (var item in items)
            {
                var doc = item.Document!;
                SetStatus($"일괄 저장 중... ({done + 1}/{items.Count}) {item.DisplayName}");
                try
                {
                    var folder = ImageExporter.ResolveFolder(settings, item.Path);
                    folders.Add(folder);
                    files += await Task.Run(() =>
                    {
                        int n = 0;
                        foreach (var obj in doc.Objects)
                        {
                            using var bmp = RenderCutout(doc, obj.Mask, options);
                            ImageExporter.Save(bmp, folder, $"{item.BaseName}_{obj.Name}", settings, opaque);
                            n++;
                        }
                        if (doc.IsImageEdited)
                        {
                            using var bmp = RenderEditedImage(doc, options);
                            ImageExporter.Save(bmp, folder, $"{item.BaseName}_편집", settings, opaque);
                            n++;
                        }
                        return n;
                    });
                }
                catch (Exception ex)
                {
                    errors.Add($"{item.DisplayName}: {ex.Message}");
                }
                done++;
            }
        }
        finally
        {
            SetEditBusy(false, null);
        }

        _lastOutputFolder = folders.Count == 1 ? folders.First() : _lastOutputFolder ?? folders.FirstOrDefault();
        string where = folders.Count == 1 ? folders.First() : $"{folders.Count}개 폴더 (각 이미지 폴더의 output)";
        SetStatus($"일괄 저장 완료: 이미지 {done - errors.Count}개, 파일 {files}개 → {where}");
        if (errors.Count > 0)
        {
            MessageBox.Show(this, "일부 이미지를 저장하지 못했습니다.\n\n" + string.Join("\n", errors.Take(10)), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void btnCopy_Click(object? sender, EventArgs e)
    {
        var target = GetExportTarget();
        if (_doc is null || target is null)
        {
            SetStatus("복사할 객체를 목록에서 선택하세요.");
            return;
        }

        try
        {
            using var bmp = RenderCutout(_doc, target.Value.Mask, CurrentCutoutOptions());
            // 투명도를 지원하는 프로그램용 PNG + 일반 비트맵
            var png = new MemoryStream();
            bmp.Save(png, ImageFormat.Png);
            var data = new DataObject();
            data.SetData("PNG", false, png);
            data.SetImage(bmp);
            Clipboard.SetDataObject(data, true);
            SetStatus($"클립보드에 복사했습니다 ({bmp.Width} × {bmp.Height}).");
        }
        catch (Exception ex)
        {
            SetStatus("복사 실패: " + ex.Message);
        }
    }

    private void btnExportSettings_Click(object? sender, EventArgs e)
    {
        using var dlg = new ExportOptionsForm(_exportSettings, _currentItem?.Path);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        _exportSettings = dlg.Settings;
        _exportSettings.Save();
        UpdateExportSummary();
        SetStatus("내보내기 설정 저장: " + _exportSettings.Summary);
    }

    private void mnuFileOpenOutput_Click(object? sender, EventArgs e)
    {
        var folder = _lastOutputFolder;
        if (folder is null || !Directory.Exists(folder))
        {
            try
            {
                folder = ImageExporter.ResolveFolder(_exportSettings, _currentItem?.Path);
            }
            catch (Exception ex)
            {
                SetStatus("저장 폴더를 열 수 없습니다: " + ex.Message);
                return;
            }
        }
        Process.Start(new ProcessStartInfo("explorer.exe", $"\"{folder}\"") { UseShellExecute = true });
    }
    private void chkBackground_CheckedChanged(object? sender, EventArgs e) => btnBackgroundColor.Enabled = chkBackground.Checked;

    private void btnBackgroundColor_Click(object? sender, EventArgs e)
    {
        using var dlg = new ColorDialog { Color = _backgroundColor, FullOpen = true };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        _backgroundColor = dlg.Color;
        btnBackgroundColor.BackColor = dlg.Color;
    }

    private void mnuFileExit_Click(object? sender, EventArgs e) => Close();
}
