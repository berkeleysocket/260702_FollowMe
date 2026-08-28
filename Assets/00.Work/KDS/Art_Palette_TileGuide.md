# Follow Me — Act 팔레트 · 타일 가이드 (아트)

> 프로듀서 2주 지시: **이모티콘 6종 + Act 팔레트·타일 가이드**. 픽셀 재작업 최소.  
> 원칙: 같은 타일셋도 **팔레트·소품·채도**로 Act 구분 (`Story_FollowMe` / `LevelDesign_Guide`)

---

## 1. 공통 규격

| 항목 | 값 |
|---|---|
| 그리드 | **32×32** |
| 아웃라인 | 1px |
| 색 수 | 스프라이트당 16색 이하 권장 |
| 필터 | nearest-neighbor only (AA 금지) |
| 레퍼런스 | LuckyLoops Satyr 톤 |

하루 고정색: 피부 `#F5D0C5` / 머리 `#3D3D3D` / 후드 `#A8D8EA` / 폰 `#1A1A2E`

---

## 2. Act별 팔레트 (타일 Tint / Global Volume)

| Act | 계절·장소 | 톤 | Global 채도 | 스카이/조명 | 타일 사용 |
|---|---|---|---|---|---|
| **1** | 봄 · 번화가 | 밝음·네온 | 100% | 하늘 `#7EC8E3`→`#E8F4FC` (기존 sky OK) | **T1** City_Modern 그대로 |
| **2** | 여름 · 카페거리 | 선명·따뜻 | 110~120% | 노란 하이라이트, 골목만 그늘↓ | **T1 재사용** + Shop 타일 강조 + 따뜻한 Tint |
| **3** | 가을 · **도시공원 야간축제** | 따뜻·밤 | 90% + 주황(불꽃) | 밤하늘·잔디채도↓, 불꽃 구간만 채도↑ | **T3 Park** + Fireworks VFX |

#### T3-01 확정 hex (S9~S11 · `Park_S9S11_AssetRequest` 연동)

**Global**: Act3 base saturation **90%**. Teach·Setpiece 불꽃 구간만 local saturation **110%** (불꽃 스프라이트 자체 tint는 아래 액센트).

| 레이어 | S9 Intro (낮→저녁) | S9~S10 Teach/Setpiece (밤) | S11 Pressure (공허) |
|---|---|---|---|
| 카메라 BG | `#1A2035` | `#141828` | `#0A0C14` |
| 하늘 tint | RGB × `(0.22, 0.26, 0.42)` | 동일 | × `(0.14, 0.16, 0.22)` |
| 원경 far | `#5A5070` α0.9 | 동일 | `#3A3848` α0.75 |
| 잔디 (지면) | `#3D6B4F` sat **90%** | `#2E4A38` sat **75%** | `#1E2E24` sat **55%** |
| 산책로 | `#5A4E42` | `#4A4038` | `#3A342E` |
| 가로등 warm | `#FFE8A8` | 동일 | `#6A7080` (dim, α0.7) |
| 불꽃 액센트 | — | `#FFB84D` · `#FF6B35` | **OFF** (스프라이트 0·연기만) |
| FestivalHollow overlay | — | — | RGB `(0.04, 0.06, 0.12)` α **0.48** |

**Unity `StageVisualThemeApplier` 현재값**: CamBg `#141828`, TileTint S11 `(0.42, 0.48, 0.38)`, grass prop `(0.32, 0.38, 0.28)` — 위 표와 정합. zip 반입 후 Park 타일은 **리컬러 없이** 팩 원색 + 위 tint만.
| **4** | 겨울 · 지하철 | 어둡·차가움 | 55~65% | 암전·열차 역광 | **T1 암화** + Floor(석재) 터널화 |
| **5** | 다시 봄 | Act1 복제 | **채도 −25%** | Act1과 동일 구도, 빛 얇게 | T1 복제 + Saturation 다운 |

### Unity에서 최소 구현 (픽셀 수정 없이)

1. Tilemap / Background에 `Color` tint만 Act별로 다르게  
2. Act5: URP Volume 또는 Sprite Material saturation −25%  
3. Act4: 전역 어두운 tint + 포인트 라이트(열차 예고)만

---

## 3. 맵 템플릿 ↔ 보유 에셋

| 템플릿 | 2주 | 보유 | 판정 |
|---|---|---|---|
| **T1 번화가** | ✅ | `KDS/05.Asset/City_Modern/` (GandalfHardcore City 32x32) | **충분** — Building·Shop·Decoration·BG 3레이어 |
| **T2 카페거리** | ✅ | 동일 City + Shop 타일 | **신규 팩 불필요** — 따뜻한 Tint + 소품 배치로 카페 거리화 |
| **T3 공원축제** | 권장/컷 | Park Zone + 가을소품 + Fireworks | 한강 폐기 → **도시공원** (`Art_Asset_Findings`) |
| **T4 지하철** | ✅ | City Floor(석재·어두운 타일) | **신규 팩 불필요** — 암화 Tint + 좁은 통로. 유료 top-down 지하철 팩은 스타일·시점 불일치 |
| Greybox | 프로토 | `KDS/05.Asset/Greybox/` | 레벨 스케치용만 |

### City_Modern 소품 활용 메모

- **번화가**: Pink/Blue glass + 신호등·가로등·표지판  
- **카페거리**: Shop 내부 선반 타일 + 따뜻한 Brick + Decoration를 골목 입구에 몰아 “그늘”  
- **지하철**: Floor/석재 프레임 + Decoration 최소화 + 조명↓  

---

## 4. 컷씬 이모티콘 6종 (필수)

가이드 §8 매핑 — 표정 ID와 1:1:

| ID | 감정 | 가이드 예시 | 용도 |
|---|---|---|---|
| `Excited` | 설렘 | 🙂 | Act1~2 컷/반응 |
| `Surprised` | 놀람 | 😮 | 첫 괴물·이벤트 |
| `Anxious` | 불안 | 😰 | 경고·추격 |
| `Tired` | 허탈 | 😔 | Act5·정산 |
| `Empty` | 공허 | 😶 | S15 |
| `Bound` | 굴레 | 🖤 | **S16 엔딩** |

파일 규격 제안: `Emote_{ID}.png` 32×32, 투명 BG, 말풍선 프레임은 개발/UI가 씌워도 됨.

### 추천 다운로드 (직접 제작 대신 선별)

| 우선 | 팩 | 링크 | 라이선스 | 비고 |
|---|---|---|---|---|
| **1순위** | Pipoya FREE Popup Emotes | https://pipoya.itch.io/free-popup-emotes-pack | 상업 OK, 재배포 금지 | 32×32, 게임 팝업용. 6종만 골라 리네이밍 |
| **대안** | 49Wolf Smileys Pack 1 | https://49wolf.itch.io/smileys-pack-1 | **CC0** | 121종 중 6개 선별. 톤이 둥글면 1px 아웃라인만 맞춤 |
| 보류 | BitBub (JustHellygar) | https://justhellygar.itch.io/bitbub | 상업 OK | 애니 있음 — 공수↑면 스킵 |
| 비추천(유료) | domechaos Speech Emote | — | 유료 | 2주 스코프 밖 |

작가 이모티콘 큐시트 오면 → 위 팩에서 **6장만 추출** → `KDS/05.Asset/Emotes/` 배치.

---

## 5. 캐릭터·적 (아직 프로젝트에 없음)

| 용도 | 팩 | 링크 | 상태 |
|---|---|---|---|
| 언팔로워(적) | LuckyLoops **Satyr** | https://lucky-loops.itch.io/character-satyr | 보드 확정, **미반입** |
| 하루 프로토 | LuckyLoops **Character Template** | https://lucky-loops.itch.io/animated-character-template | 추천, **미반입** — 폰·민트후드는 이후 리컬러 |
| 플레이어 본구현 | KSY 폴더 | — | 아트는 훅·가이드만 |

2주: Satyr 반입 + Template 프로토 가능. Act별 의상·자세 변화는 프로듀서 컷.

---

## 6. 찾지 말 것 / 사지 말 것 (2주)

- Top-down 전용 지하철·시티 팩 (시점 불일치)  
- 중세 House 유료 팩 (카페 현대감과 충돌)  
- 표정 스프라이트 6종 전부 신규 제작 (프로듀서: 버리기)  
- Act별 풀 리페인트

---

## 7. 반입 체크리스트 (김동선/레벨)

- [ ] `Emotes/` 6종 PNG (Pipoya 또는 49Wolf에서 선별)  
- [ ] Satyr zip → `KDS/05.Asset/Characters/Satyr/`  
- [ ] (선택) Character Template → 프로토 플레이어  
- [ ] Stage1에 Act1 tint 확인 / Act5용 Saturation 프리셋 메모  
- [ ] Credits: GandalfHardcore, LuckyLoops, Pipoya(또는 49Wolf)

---

## 8. 다음 역할 전달

| 역할 | 요청 |
|---|---|
| **작가** | S16 이모티콘 큐시트 (시간순 ID 나열) |
| **레벨** | T1 tint만으로 S2/S3 차별 / S4는 Shop+따뜻 Tint / S12는 암화 |
| **개발** | 컷씬 이모티콘 재생 + Act별 Global tint/saturation 훅 |
| **프로듀서** | 유료 지하철 팩 구매 불필요 — 팔레트 전략으로 확정 요청 |
