# 제3자 구성요소 및 AI 모델 라이선스

Nukki Studio의 소스 코드는 [MIT License](LICENSE)로 공개됩니다.
아래 구성요소는 각 저작권자의 라이선스를 따르며, 라이선스 전문은 [`src/Resources/Licenses/`](src/Resources/Licenses/)와 프로그램의 **도움말 → 프로그램 정보**에 들어 있습니다.

## exe에 포함되는 구성요소

| 구성요소 | 저작권자 | 라이선스 | 전문 |
|------|------|------|------|
| .NET Runtime / Windows Forms | .NET Foundation | MIT | [06_DotNet.txt](src/Resources/Licenses/06_DotNet.txt) |
| ONNX Runtime (DirectML 패키지) | Microsoft | MIT | [04_ONNXRuntime.txt](src/Resources/Licenses/04_ONNXRuntime.txt) |
| DirectML | Microsoft | Microsoft 소프트웨어 사용 조건 (앱과 함께 재배포 허용) | [05_DirectML.txt](src/Resources/Licenses/05_DirectML.txt) |

## AI 모델 (exe에 포함하지 않음 — 첫 실행 때 사용자가 원래 출처에서 직접 받음)

| 모델 | 용도 | 저작권자 | 라이선스 | 받는 곳 |
|------|------|------|------|------|
| MobileSAM | 객체 선택 | Chaoning Zhang 외 | Apache 2.0 | [nrl-ai/anylearning-labeling-models](https://huggingface.co/nrl-ai/anylearning-labeling-models) |
| Segment Anything 디코더 | 객체 선택 | Meta Platforms | Apache 2.0 | 위와 같음 |
| D-FINE-N (COCO) | 객체 자동 감지 | Peterande | Apache 2.0 | 위와 같음 |
| MI-GAN | 객체 지우기 (빠름) | Picsart AI Research | MIT | [FreeHugsForRobots/ps-inpaint-migan](https://huggingface.co/FreeHugsForRobots/ps-inpaint-migan) |
| LaMa | 객체 지우기 (고품질) | Samsung AI Center Moscow / OpenCV Zoo 변환 | Apache 2.0 | [opencv/inpainting_lama](https://huggingface.co/opencv/inpainting_lama) |

## 참고

- MI-GAN과 LaMa의 공개 가중치는 Places2 데이터셋으로 학습되었으며, 이 데이터셋에는 연구 / 비상업 용도 조건이 있습니다. 이 프로그램은 무료로 공개되며 판매하지 않습니다. 상업적으로 쓰려면 각 모델과 데이터셋 조건을 따로 확인하세요.
- 모델 파일은 이 저장소와 배포 파일에 포함되어 있지 않습니다. 프로그램이 위 주소에서 받으며, 각 모델의 라이선스는 받는 곳의 조건을 따릅니다.
