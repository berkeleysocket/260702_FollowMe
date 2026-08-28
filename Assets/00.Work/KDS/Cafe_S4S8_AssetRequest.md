# S4~S8 카페거리 — 에셋 일괄 요청 (한 번에)

> 대상: 김동선 / 아트  
> 목적: Act2 「달콤함」 S4~S8을 **처음부터** 카페거리 맵으로 제작  
> 규격: 32×32 · 1px 아웃라인 · nearest-neighbor · Satyr 톤  
> 반입 경로: `Assets/00.Work/KDS/05.Asset/_Downloads/` → 정리 후 `City_Cafe/`

현재 보유: `City_Cafe/coffeeshopstuff.png` 1장뿐 (파사드·티바·가구·가로등).  
**아래를 한 번에** zip으로 넣어 주시면 맵 퀄이 크게 올라갑니다.

---

## 1. 다운로드 요청 (우선순위 순)

| 우선 | 팩 | 링크 | 쓰는 곳 | 비고 |
|---|---|---|---|---|
| **필수** | PixelPossum **Cafe & Tea Shop 2D** | https://pixelpossumstudio.itch.io/cafe-and-tea-shop-2d | 파사드·어닝·야외좌석·간판 | NYOP. 횡스크롤 midground용 |
| **필수** | Wish **Drinks** (free) | https://wish1.itch.io/wishdrinkspack | 디저트/음료 포토 소품 | free. midground prop만 |
| **강력** | B0z **Basic Platformer 32×32** | https://b0z609.itch.io/2d-basic-platformer-32x32-asset-pack | 카페거리 바닥·플랫폼 | 이미 itch 목록에 있음 |
| **강력** | SIL01 실루엣 (trial OK) | https://pixelartmaterial.itch.io/sil01-100-silhouette-assets | 줄 서는 NPC 2~3포즈 | 축소·단색 처리 |
| **선택** | Coloritmic Neon Free | https://coloritmic.itch.io/neoncityasset | 카페 네온 간판 보강 | 이미 목록에 있음 |

위 zip을 `_Downloads/`에 넣고 알려주시면 `City_Cafe/`로 정리·임포트(PPU32/Point)까지 이어서 합니다.

---

## 2. 없으면 제작/대체 요청 (아트 0.5~1일)

| ID | 에셋 | 사이즈·수량 | 용도 |
|---|---|---|---|
| **CAF-BG** | 여름 카페거리 **원경 BG** 1장 (또는 sky+far 2레이어) | 가로 타일 가능, 높이 ~11u 체감 | City 번화가 BG와 구분 |
| **CAF-GROUND** | 보도블록/카페거리 **바닥 타일** 3~4종 | 32×32 | TempGround 교체 |
| **CAF-ALLEY** | **골목 벽** 타일 2~3 (전선·좁은 벽) | 32×32 or 스프라이트 | Pressure·Setpiece |
| **CAF-PHOTO** | 포토존 **프레임/스탠딩보드** 1 | ~64×64 이하 | Breath 포토 |
| **CAF-DESSERT** | 디저트 조형 1 | ~48×48 | S4 Photo_Dessert |
| **CAF-NPC** | 줄서기 실루엣 2~3 (같은 포즈 반복 OK) | 32×48 전후 | S5 Photo_PoseLine |
| **CAF-SHADE** | (선택) 그늘 오버레이 가이드 — 없어도 Tint로 대체 가능 | — | 경고 구간 |

---

## 3. 지금 맵에서 쓰는 것 (요청 전에도 진행)

- `coffeeshopstuff` → CAFE/TEA 파사드·가로등·테이블·펜스
- City_Modern sky/far → 따뜻한 틴트만 (원경)
- 골목 그늘 = 반투명 오버레이
- 시스템: Zones / MapMode / CP / Photo / Monster 슬롯 (스펙 그대로)

---

## 4. 완료 정의 (S4~S8)

- S4: 밝은 카페거리 + 경고 골목(괴물 미스폰) + 포토 3
- S5: 첫 추격 사이클 + 포토 2
- S6~S8: 같은 템플릿 변형 (길이·괴물·밀도만 스펙 따름)
- Game 뷰에서 **번화가(S1)와 한눈에 구분**

요청물 반입 전이라도 **골격+카페 소품 배치**는 재생성해 둠.
