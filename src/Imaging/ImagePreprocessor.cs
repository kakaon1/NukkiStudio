namespace NukkiStudio.App.Imaging;

/// <summary>모델 입력용 전처리: 리사이즈 + RGB float(0~255) HWC 텐서 변환.</summary>
public static class ImagePreprocessor
{
    /// <summary>
    /// image를 outW × outH로 리사이즈해 HWC RGB float 배열로 만든다.
    /// tensorW × tensorH가 더 크면 오른쪽 / 아래를 0으로 채운다.
    /// 축소는 영역 평균, 확대는 바이리니어로 처리한다.
    /// </summary>
    public static float[] ToRgbHwc(ImageBuffer image, int outW, int outH, int tensorW, int tensorH)
    {
        var dst = new float[tensorW * tensorH * 3];
        var src = image.Pixels;
        int sw = image.Width, sh = image.Height;
        float scaleX = (float)sw / outW;
        float scaleY = (float)sh / outH;
        bool downscale = scaleX > 1f || scaleY > 1f;

        Parallel.For(0, outH, y =>
        {
            for (int x = 0; x < outW; x++)
            {
                float r, g, b;
                if (downscale)
                {
                    // 대상 픽셀이 덮는 원본 영역 평균
                    int x0 = (int)(x * scaleX), x1 = Math.Max(x0 + 1, Math.Min(sw, (int)((x + 1) * scaleX)));
                    int y0 = (int)(y * scaleY), y1 = Math.Max(y0 + 1, Math.Min(sh, (int)((y + 1) * scaleY)));
                    long sr = 0, sg = 0, sb = 0;
                    for (int yy = y0; yy < y1; yy++)
                    {
                        int row = yy * sw * 4;
                        for (int xx = x0; xx < x1; xx++)
                        {
                            int i = row + xx * 4;
                            sb += src[i];
                            sg += src[i + 1];
                            sr += src[i + 2];
                        }
                    }
                    int n = (x1 - x0) * (y1 - y0);
                    r = (float)sr / n;
                    g = (float)sg / n;
                    b = (float)sb / n;
                }
                else
                {
                    float fx = Math.Clamp((x + 0.5f) * scaleX - 0.5f, 0, sw - 1);
                    float fy = Math.Clamp((y + 0.5f) * scaleY - 0.5f, 0, sh - 1);
                    int ix = (int)fx, iy = (int)fy;
                    int ix1 = Math.Min(ix + 1, sw - 1), iy1 = Math.Min(iy + 1, sh - 1);
                    float ax = fx - ix, ay = fy - iy;
                    float Sample(int c)
                    {
                        float p00 = src[(iy * sw + ix) * 4 + c], p10 = src[(iy * sw + ix1) * 4 + c];
                        float p01 = src[(iy1 * sw + ix) * 4 + c], p11 = src[(iy1 * sw + ix1) * 4 + c];
                        float top = p00 + (p10 - p00) * ax;
                        float bottom = p01 + (p11 - p01) * ax;
                        return top + (bottom - top) * ay;
                    }
                    b = Sample(0);
                    g = Sample(1);
                    r = Sample(2);
                }

                // 투명한 부분(배경 지우기 결과)은 중간 회색 위에 합성해서 모델에 넘긴다
                float alpha = src[(Math.Min((int)(y * scaleY), sh - 1) * sw + Math.Min((int)(x * scaleX), sw - 1)) * 4 + 3] / 255f;
                if (alpha < 1f)
                {
                    r = r * alpha + 128f * (1f - alpha);
                    g = g * alpha + 128f * (1f - alpha);
                    b = b * alpha + 128f * (1f - alpha);
                }

                int o = (y * tensorW + x) * 3;
                dst[o] = r;
                dst[o + 1] = g;
                dst[o + 2] = b;
            }
        });
        return dst;
    }
}
