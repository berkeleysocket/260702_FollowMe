# S9~S11 도시공원 야간축제 — 에셋 일괄 요청 (한 번에)

> 대상: **아트 디렉터** → 김동선 zip 반입  
> 목적: Act3 「불꽃」 S9~S11을 S8(카페) **이후** 이어지는 맵으로 폴리시  
> 규격: 32×32 · 1px 아웃라인 · nearest-neighbor · Satyr 톤  
> 반입 경로: `Assets/00.Work/KDS/05.Asset/_Downloads/` → 정리 후 `City_Park/` · `VFX_Fireworks/`

**판정**: 골격·테마 코드는 완료. Game 뷰에서 **S4(카페)와 S9(공원)이 한눈에 구분 안 됨** → 공원 전용 에셋 필수.

---

## 0. 지금 맵 상태 (대체 중)

| 항목 | 현재 | 문제 |
|---|---|---|
| 지면 | City_Modern Floor + 녹색 틴트 | 잔디·산책로 느낌 약함 |
| 소품 | Decoration 시트를 나무·가로등으로 **임의 매핑** | 공원·축제로 안 읽힘 |
| 배경 | City sky + WarpedCity 스카이라인 | 원경은 OK, midground 공원 부재 |
| 불꽃 | Kenney star/magic + Stealthix **Fires**(OGA) | **정식 Fireworks 시트(itch) 미반입** |
| NPC | 검은 Quad 실루엣 | 폰 보는 포즈 없음 |
| S11 공허 | 반투명 오버레이만 | 연기·조명 꺼짐 연출 부족 |

---

## 1. 다운로드 요청 (우선순위 순) — **거의 무료, 한 번에**

| 우선 | 팩 | 링크 | 쓰는 곳 | 비고 |
|---|---|---|---|---|
| **필수** | Free **Park Zone** (CraftPix) | https://free-game-assets.itch.io/free-green-zone-tileset-pixel-art | 잔디·보도·벤치·분수·울타리·나무·원경 BG | NYOP. itch 목록에 있음 |
| **필수** | Akuarii33 **Simple Park** | https://akuarii33.itch.io/simple-park-and-modular-building-assests | **가을 나무**·덤불·벤치·가로등·축제 소품 | NYOP. **fall** 변형 우선 |
| **필수** | Stealthix **Animated Fireworks** | https://stealthix.itch.io/animated-fireworks | Teach/Setpiece 불꽃 VFX | CC0. OGA Fires와 별도 |
| **강력** | SIL01 실루엣 (trial OK) | https://pixelartmaterial.itch.io/sil01-100-silhouette-assets | 벤치에 앉아 폰 보는 NPC 2~3 | T2·T3 공용 |
| **선택** | Taxmanium Basic Grass 32×32 | https://taxmaniumgames.itch.io/basic-grass-tileset-32x32 | 잔디·슬로프·꽃 ($2) | Park Zone 보강 |
| **선택** | RagnaPixel Particle FX | https://ragnapixel.itch.io/particle-fx | S11 공허 연기 (~$2.79) | T3-05 |
| **선택** | MasTho FX V3 | https://mastho0128.itch.io/animations-and-particles-fx-v3 | 불꽃 고퀄 ($5) | Stealthix 대체/보강 |

zip을 `_Downloads/`에 넣고 알려주시면 → `City_Park/` · `VFX_Fireworks/` 정리 · PPU32/Point 임포트 · `StageVisualThemeApplier` 연결까지 KDS가 이어서 함.

---

## 2. 없으면 제작/대체 요청 (아트 0.5~1일)

| ID | 에셋 | 사이즈·수량 | 용도 | 스테이지 |
|---|---|---|---|---|
| **T3-01** | **가을·밤 팔레트** hex + 레이어별 틴트 1장 | 문서 | 잔디 채도↓·불꽃 주황 액센트 | S9~S11 |
| **T3-02** | 공원 **midground** 세트 | 32×32 타일 + ~64px 소품 | 벤치·가로등·분수·울타리·**가을 나무** 2종 | Intro·Breath·Goal |
| **T3-02b** | 원경 **도시 스카이라인** 1레이어 (선택) | 가로 타일 | 공원 너머 도시 | Background far |
| **T3-03** | 불꽃 스프라이트 **터짐 2종** (애니 시트 OK) | ~64×64 | ♡ 윈도우 Teach·Setpiece | S9 Photo_FireworksPeak ~48 |
| **T3-04** | **폰 보는 NPC** 실루엣 2~3 (벤치·서기) | 32×48 | 축제 인파 서사 | Breath·Setpiece |
| **T3-05** | 축제 **끝** 연출: 조명↓·연기 1종 | 오버레이 or 파티클 | Pressure 공허 | **S11** FestivalHollow 대체 |
| **T3-PHOTO** | 포토존 연출 3종 | ~64px | Photo_FireworksPeak / RiverBridge→**분수·다리** / PhoneCrowd | S9 기준 |

### 스테이지별 차이 (에셋 밀도만)

| St | 연출 키워드 | 에셋 요청 포인트 |
|---|---|---|
| **S9** | 축제 피크·♡ 최대급 | 불꽃 밀집·밝은 가로등·NPC 다수 |
| **S10** | 알림 공포·♡ 42 | 불꽃 유지·NPC 밀도↑ |
| **S11** | **공허**·Breath 끊김 | 같은 공원 + 불꽃 OFF·조명↓·나무/NPC↓·연기 |

---

## 3. 팔레트 가이드 (T3-01 — 문서 즉시 가능)

`Art_Palette_TileGuide.md` §Act3 기준:

| 레이어 | 낮(Intro) | 밤 Teach/Setpiece | S11 Pressure |
|---|---|---|---|
| 하늘 | `#1A2035` | `#141828` | `#0A0C14` |
| 잔디 | `#3D6B4F` sat 90% | `#2E4A38` sat 75% | `#1E2E24` sat 55% |
| 산책로 | `#5A4E42` | `#4A4038` | `#3A342E` |
| 불꽃 액센트 | — | `#FFB84D` · `#FF6B35` | **OFF** |
| 가로등 | `#FFE8A8` warm | 동일 | `#6A7080` dim |

Global: Act3 base sat **90%**. 불꽃 구간만 local sat **110%**.

---

## 4. 납품·임포트 규칙

1. PNG / 스프라이트시트, **PPU = 32**, Filter = Point  
2. 폴더: `05.Asset/City_Park/{ParkZone_CraftPix,Akuarii33_Park}/` · `05.Asset/VFX_Fireworks/Stealthix_Fireworks/`  
3. AI 생성물 → Aseprite 32×32 재작업 전제  
4. 크레딧: `CREDITS_Downloads.md` 갱신 (Akuarii33 크레딧 문구 확인)

---

## 5. 완료 정의 (S9~S11)

- Game 뷰에서 **S4 카페거리와 S9 공원이 3초 안에 구분**됨  
- Teach 구간: 하늘 불꽃이 **좋아요 윈도우** 연출로 읽힘 (정적 star 대체 탈출)  
- S11 Pressure: 축제 끝난 **텅 빈 공원** (불꽃 0·조명 약함·연기 optional)  
- 포토 3곳(S9): 불꽃 피크 / 분수·전망 / 폰 인파 — 각각 전용 소품 1개 이상

---

## 6. 일정 제안 (프로듀서)

| 시점 | 아트 | 레벨 영향 |
|---|---|---|
| **즉시** | T3-01 팔레트 hex 확정 (문서) | 틴트만으로 S11 공허 미세조정 가능 |
| **D+0** (zip 수령) | Park Zone + Akuarii33 + Stealthix 반입 | S9 대표맵 폴리시 시작 |
| **D+1** | T3-04 NPC 실루엣 · T3-05 연기 | S10~S11 밀도·공허 |
| **T4 전** | 선택 유료(Taxmanium/MasTho) | S9~S11 마감 후 S12 착수 |

**예산 1순위**: T3 무료 3종 ≈ **$0** → T4 SakPix($4)보다 **먼저** (S8 직후 플레이 연속성).

---

## 7. 아트 디렉터 판정 (2026-08-28)

### 7-1. 반입 경로 · 임포트 규칙 (필수 3종)

| zip ( `_Downloads/` ) | 풀 경로 | Applier·용도 |
|---|---|---|
| Park Zone (CraftPix NYOP) | `05.Asset/City_Park/ParkZone_CraftPix/` | 잔디·보도·벤치·분수·울타리·원경 BG |
| Akuarii33 Simple Park (**fall** 변형 우선) | `05.Asset/City_Park/Akuarii33_Park/` | 가을 나무 2종·덤불·가로등·축제 소품 |
| Stealthix **Animated Fireworks** (itch, OGA Fires와 별도) | `05.Asset/VFX_Fireworks/Stealthix_Fireworks/` | Teach/Setpiece `FireworkBurstSprites` 교체 |

**임포트 (전 PNG/시트 공통)**:

1. Texture Type = **Sprite (2D and UI)** · Multiple if 시트  
2. **PPU = 32** · Filter Mode = **Point (no filter)** · Compression = **None**  
3. AA 금지 · 32×32 그리드 외 소품은 **Aseprite 재작업** 후 반입  
4. 반입 후 `CREDITS_Downloads.md` + Akuarii33 크레딧 문구 확인  
5. 메뉴 `FollowMe/KDS/Apply Fireworks Park Themes (S9-S11)` 재실행

**현재 `_Downloads/`**: Park·Akuarii33·Fireworks zip **미수령** — `itch_manual_urls.txt` L8~L9·L7.

### 7-2. T3-01 팔레트 — **확정** (`Art_Palette_TileGuide.md` §2 T3-01)

§3 표와 동일. 문서 즉시 납품 완료.

### 7-3. SIL01 vs RagnaPixel

| 팩 | 판정 | 일정 | 사유 |
|---|---|---|---|
| **SIL01 trial** | **채택 (강력)** | **D+0** (park zip과 동시) 또는 zip 없으면 **D+1** | T3-04 폰 NPC · T2 줄서기 공용. 검정 Quad 대체 탈출 |
| **RagnaPixel Particle FX** | **보류** | D+1 **플레이테스트 후** | S11 연기: **D+0 Kenney** `smoke_08~10` + FestivalHollow로 최소 충족. 공허가 약하면 RagnaPixel ($2.79) |
| MasTho FX V3 ($5) | **컷** | — | Stealthix Fireworks + Kenney로 2주 스코프 충분 |
| Taxmanium Grass ($2) | **선택** | T4 전 여유 시 | Park Zone 잔디 보강만 |

### 7-4. 레벨 납품 경로 · Applier 연결 (KDS 개발)

**레벨이 쓰는 폴더** (반입 완료 후):

```
05.Asset/City_Park/ParkZone_CraftPix/     — 지면 타일·공원 midground
05.Asset/City_Park/Akuarii33_Park/        — 가을 나무·벤치·가로등 (fall 우선)
05.Asset/VFX_Fireworks/Stealthix_Fireworks/ — 불꽃 burst (itch)
05.Asset/VFX_Fireworks/Kenney_ParticlePack/PNG/ — 연기·스파크 (S11, 기존)
05.Asset/City_Neon/WarpedCity_ansimuz/.../skyline-a.png — 원경 far (유지)
05.Asset/Characters/SIL01/ (trial)        — NPC 실루엣 (D+0~1)
```

**`StageVisualThemeApplier.cs` 수정 포인트** (zip 후 KDS):

| 항목 | 현재 (그레이박스) | zip 후 |
|---|---|---|
| `PlaceParkGroundVisual` | `FloorSheet` 타일 + 녹색 tint | `ParkZone_CraftPix` 잔디/보도 시트 |
| `PlaceParkSceneryInRange` | `DecorationSheet` 인덱스 임의 매핑 | Akuarii33 가로등·**fall tree**·벤치 (+ Park Zone 분수 등) |
| `FireworkBurstSprites` | Kenney star/magic + `Stealthix_Fires` | **Stealthix_Fireworks** itch 시트 우선 |
| `PlacePhoneNpcSilhouettes` | 흰 Quad | SIL01 `phone/sitting` 실루엣 2~3 |
| `PlaceFestivalHollow` | 단색 overlay | 유지 + optional Kenney smoke / RagnaPixel |
| `EnsurePixelImportSettings` | Cafe·Neon·Floor 등 | 위 Park·Fireworks·SIL01 경로 추가 |

**레벨 작업** (Applier 이후):

- 메뉴 `Apply Fireworks Park Themes (S9-S11)` → Game 뷰 **S4 카페 vs S9 공원 3초 구분** 확인  
- S9 Photo 3곳: 불꽃 피크 / 분수·전망 / 폰 인파 — `StageMapDatabase` 포토 X에 맞춰 장애물·좋아요 배치  
- S11: Teach~Pressure 구간 불꽃 0·가로등 dim·NPC↓ — 코드는 `spec.Stage >= 11` 분기 이미 있음

**원경**: WarpedCity `skyline-a` 유지. Park Zone BG 레이어 있으면 far 뒤에 1줄 추가 가능 (T3-02b 선택).
