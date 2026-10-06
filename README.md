# Nukki Studio

이미지 속 **객체(사람, 동물, 물건 등)를 AI로 찾거나 직접 선택**해서 **누끼 따기 / 객체 지우기 / 배경 지우기**를 하는 Windows 데스크톱 프로그램입니다. 여러 이미지를 목록에 넣고 클릭해 가며 작업한 뒤 한 번에 저장할 수 있습니다.

## 주요 기능

| 기능 | 설명 |
|------|------|
| 객체 자동 선택 (A) | AI가 이미지 속 객체(80종)를 찾아 "사람 1", "개 1"처럼 목록에 올림 — 버튼을 눌렀을 때만 실행 |
| 수동 선택 | 클릭(좌: 포함 / 우: 제외), 박스 드래그, 브러시로 칠하기 (원형 / 네모, 툴바 + / − 또는 + / − 키로 크기 조절) |
| 누끼 저장 (Ctrl+S) | 선택한 객체만 배경 없이 저장 (객체 크기로 자르기 / 배경색 채우기 옵션) |
| 객체 지우기 (Del) | 선택한 객체를 지우고 주변 배경으로 자연스럽게 채움 — 오른쪽 위에서 MI-GAN(빠름) / LaMa(고품질) 선택 |
| 화면 보기 | 휠 확대 / 축소, 확대 시 아래·오른쪽 스크롤 막대로 이동 (Shift+휠 좌우), 축소 시 고품질 표시 |
| 이미지 크기 조절 (Ctrl+R) | 해상도 변경 (고품질 확대 + 선명화, 객체도 함께 맞춤) |
| AI 모델 다운로드 | 없는 모델을 프로그램에서 진행률과 함께 받기 (용량 표시, 누를 때만 받음) |
| 배경 지우기 (Ctrl+B) | 선택한 객체만 남기고 나머지를 투명하게 |
| 잘라내기 / 복사 / 붙여넣기 | Ctrl+X / Ctrl+C / Ctrl+V — 붙여넣은 객체를 드래그로 이동, Ctrl+휠로 크기 조절 후 Enter로 확정 |
| 이미지 목록 · 일괄 저장 | 파일 / 폴더(하위 포함)를 끌어다 놓아 추가, 이미지별 작업 유지, 전체 일괄 저장 |
| 내보내기 설정 | 형식(PNG / JPG / BMP), JPG 품질, 크기(원본 / 비율 5~400% / 긴 변 최대), 저장 위치(원본 폴더의 output / 지정 폴더) |
| 보정 | 경계 부드럽게, 확장 / 축소, 작은 조각 제거 · 구멍 메우기 |
| 실행 취소 | Ctrl+Z / Ctrl+Y (Ctrl+Shift+Z) — 지우기·배경 지우기·붙여넣기 포함 |
| 도움말 / 프로그램 정보 | F1 사용 설명서, 오픈소스 / AI 모델 라이선스 표시 |

자세한 사용법과 단축키 전체는 [docs/사용설명서.md](docs/사용설명서.md) (프로그램에서 F1과 같은 내용)를 참고하세요.

## 기본 사용 순서

1. 이미지 파일이나 폴더를 창에 **끌어다 놓기** (또는 가운데 안내 영역 클릭 / Ctrl+O)
2. **객체 자동 선택(A)** 또는 객체를 클릭해서 선택
3. 객체 목록에서 고른 뒤 **Ctrl+S**(누끼 저장) / **Del**(지우기) / **Ctrl+B**(배경 지우기)
4. 다음 이미지는 왼쪽 목록에서 클릭 (PageDown), 끝나면 **전체 일괄 저장**
5. 결과는 기본적으로 **원본 파일 폴더 안의 `output` 폴더**에 저장됩니다.

## 기술 스택

| 항목 | 선택 |
|------|------|
| 언어 / 런타임 | C# / .NET 10.0 LTS (`net10.0-windows`) |
| UI | WinForms (다크 테마, 사용자 정의 컨트롤) |
| AI 추론 | Microsoft.ML.OnnxRuntime.DirectML 1.24.4 (GPU 우선, 실패 시 CPU) |
| 객체 윤곽 | MobileSAM ONNX (약 43MB, Apache 2.0) |
| 객체 자동 감지 | D-FINE-N COCO ONNX (14.8MB, Apache 2.0) — 정확도 때문에 항상 CPU |
| 객체 지우기 | MI-GAN 512 ONNX (26.8MB, MIT, GPU) / LaMa ONNX (88.3MB, Apache 2.0, CPU 약 2초) — 모델이 없으면 기본 채우기 알고리즘 |
| 실행 방식 | **Python 없이 C# exe 하나로 동작** (ONNX 모델만 사용) |
| 배포 | Release 빌드 시 자동 Publish — Self-Contained 단일 exe |

측정값 (개발 PC, RTX 4050 Laptop): 이미지 분석 0.05~0.25초, 클릭당 선택 0.05~0.1초, 객체 감지 약 0.05초(CPU), 객체 지우기 0.08~0.15초(GPU).

## 빌드 및 실행

1. Visual Studio 2026에서 `NukkiStudio.sln`을 엽니다.
2. 구성을 **Release**로 선택하고 빌드합니다 (Debug 빌드는 사용하지 않습니다). 빌드가 끝나면 자동으로 게시됩니다.
3. 결과물: `src\bin\Release\Publish\`
   - `NukkiStudio.exe` — 단일 실행 파일 (약 65MB, .NET 런타임 포함, 별도 설치 불필요)
   - `models\` — AI 모델 폴더 (exe와 함께 배포)
4. `NukkiStudio.exe`를 실행하거나, 이미지 파일 / 폴더를 exe 위에 끌어다 놓아 바로 엽니다.

※ 실행 중인 프로그램이 있으면 게시 파일이 잠겨 빌드가 실패하므로 먼저 종료하세요.

## 모델 준비

ONNX 파일은 용량 때문에 git에서 제외됩니다. 프로그램을 실행하면 없는 모델을 **AI 모델 다운로드** 창에서 받을 수 있습니다 (설정 → AI 모델 다운로드 / 관리). 받는 주소:

| 폴더 | 파일 | 출처 |
|------|------|------|
| `models\lama\` | `inpainting_lama_2025jan.onnx` (88.3MB) | https://huggingface.co/opencv/inpainting_lama |
| `models\mobile_sam\` | `mobile_sam_20230629.zip` (35MB) 압축 해제 | https://huggingface.co/nrl-ai/anylearning-labeling-models |
| `models\dfine\` | `dfine_n_coco_956d170.zip` (13.3MB)의 `dfine_n_coco.onnx`, `LICENSE` | 같은 저장소 |
| `models\migan\` | `migan-512-generator.onnx` (26.8MB) | https://huggingface.co/FreeHugsForRobots/ps-inpaint-migan |

`models\mobile_sam`만 있어도 수동 선택 / 누끼 / 배경 지우기는 동작합니다. `dfine`이 없으면 객체 자동 선택, `migan`이 없으면 AI 지우기 대신 기본 채우기가 쓰입니다.

## 폴더 구조

```
Nukki_Studio/
├─ CLAUDE.md                 에이전트 문서 (작업 지침 + 프로젝트 정보)
├─ README.md                 이 파일
├─ CODEMAP.md                오류 / 결함 이력
├─ docs/
│  ├─ 설계서.md              상세 설계 및 진행 현황
│  └─ 사용설명서.md          사용 설명서 (도움말 F1)
├─ NukkiStudio.sln
├─ models/                   mobile_sam / dfine / migan / lama(프로그램에서 받기)
└─ src/                      WinForms 프로그램 (NukkiStudio.csproj)
   ├─ MainForm / ExportOptionsForm / HelpForm / AboutForm / ModelDownloadForm / ImageResizeForm / BusyForm  (각 .cs / .Designer.cs / .resx)
   ├─ Controls/  UI/  Imaging/  Segmentation/  Detection/  Inpainting/  Models/  Export/  Editing/
   └─ Resources/             app.ico, Licenses/
```

## 라이선스

이 프로그램은 다음 오픈소스 구성요소와 AI 모델을 각 라이선스 조건에 따라 포함합니다. 저작권 고지와 라이선스 전문은 프로그램의 **도움말 → 프로그램 정보**와 `src/Resources/Licenses/`에 있습니다.

| 구성요소 | 라이선스 |
|----------|----------|
| MobileSAM (ChaoningZhang) | Apache License 2.0 |
| Segment Anything 디코더 (Meta Platforms) | Apache License 2.0 |
| D-FINE-N (Peterande) | Apache License 2.0 |
| MI-GAN (Picsart AI Research) | MIT License |
| LaMa (Samsung AI / OpenCV Zoo 변환본) | Apache License 2.0 |
| ONNX Runtime (Microsoft) | MIT License |
| DirectML (Microsoft) | Microsoft 소프트웨어 사용 조건 |
| .NET Runtime / Windows Forms (.NET Foundation) | MIT License |

## 문서

| 문서 | 내용 |
|------|------|
| [CLAUDE.md](CLAUDE.md) | 에이전트 작업 지침, 개발 환경, 빌드 방법, UI / 모델 규칙, 변경 이력 |
| [docs/사용설명서.md](docs/사용설명서.md) | 사용 방법과 단축키 전체 |
| [docs/설계서.md](docs/설계서.md) | 용어, 모델 선정, 처리 흐름, 구조, 진행 현황 |
| [CODEMAP.md](CODEMAP.md) | 오류 / 결함 원인과 조치 이력 |
"# NukkiStudio" 
