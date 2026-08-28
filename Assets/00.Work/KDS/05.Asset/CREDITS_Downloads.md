# 에셋 CREDITS · 반입 현황

> 경로: `Assets/00.Work/KDS/05.Asset/`  
> 갱신: 2026-08-25

## 자동 반입 완료 (에이전트가 프로젝트에 넣음)

| 폴더 | 출처 | 라이선스 | 용도 |
|---|---|---|---|
| `City_Cafe/coffeeshopstuff.png` | PixelPossumStudio via [OpenGameArt](https://opengameart.org/content/cafe-and-tea-shop-pixel-art) | 기부 권장 / itch NYOP | T2 카페 파사드·소품 (Aseprite 조합 전제) |
| `City_Neon/WarpedCity_ansimuz/` | ansimuz [Warped City](https://opengameart.org/content/warped-city) | Public domain | T1 네온·야경 BG/타일 보강 |
| `VFX_Fireworks/Stealthix_Fires/` | Stealthix [Animated Fires](https://opengameart.org/content/animated-fires) | **CC0** | VFX (불꽃축제용 **Firework 시트는 itch 수동**) |
| `VFX_Fireworks/Kenney_ParticlePack/` | Kenney via [OGA](https://opengameart.org/content/particle-pack-80-sprites) | CC0 | T3 불꽃/스파크·연기 파티클 |
| `_Downloads/` | zip 원본 보관 | — | 재압축용 |

Filter Mode = Point · PPU 32 — Unity에서 `FollowMe/KDS/Apply Map Asset Import (Cafe S4-S8 + Park S9-S11)` 실행.

### T2 카페 (S4-S8) — **반입 완료 (2026-08-28)**

| 팩 | 풀 위치 | 상태 |
|---|---|---|
| OGA coffeeshopstuff | `City_Cafe/coffeeshopstuff.png` | ✅ PPU32 Point |
| Wish Drinks **free** | `City_Cafe/Wish_Drinks/` | ✅ 28+ PNG (디저트·음료 포토) |
| PixelPossum itch | `City_Cafe/PixelPossum_Cafe/CoffeeShopStuff.png` | ✅ itch는 PNG만 (OGA 동일 팩) |
| zip 원본 | `_Downloads/Wish_Drinks.zip` · `PixelPossum_Cafe.zip` | ✅ |

SIL01·B0z·UI/HUD·플레이어 에셋은 **본 작업 범위外**.

---

## itch.io 수동 다운로드 필요 (로그인 / NYOP / 유료)

에이전트가 itch 계정 없이 zip을 받을 수 없음. 아래 링크에서 **Download** → zip을 받은 뒤  
`_Downloads/`에 넣고, 표의 **풀 경로**로 풀어 주세요.

### P0 필수

| 팩 | 링크 | 풀 위치 |
|---|---|---|
| LuckyLoops Ultimate UI Icons | https://lucky-loops.itch.io/ultimate-ui-pixelart-icons | `UI_Icons/LuckyLoops/` |
| Pixel Explosive UI 25 Icons | https://pixelexplosive.itch.io/pixel-art-ui-icon-pack-25-icons-32x32 | `UI_Icons/PixelExplosive/` |
| Pipoya Popup Emotes | https://pipoya.itch.io/free-popup-emotes-pack | `Emotes/Pipoya/` |
| LuckyLoops Satyr | https://lucky-loops.itch.io/character-satyr | `Characters/Satyr/` |
| B0z Basic Platformer | https://b0z609.itch.io/2d-basic-platformer-32x32-asset-pack | `Props_Checkpoint/B0z/` |
| Coloritmic Neon City **Free** | https://coloritmic.itch.io/neoncityasset | `City_Neon/Coloritmic_Free/` |
| Stealthix **Animated Fireworks** | https://stealthix.itch.io/animated-fireworks | `VFX_Fireworks/Stealthix_Fireworks/` |

### T3 공원 — **반입 완료 (2026-08-28)**

| 팩 | 풀 위치 | 상태 |
|---|---|---|
| Free Park Zone | `City_Park/ParkZone_CraftPix/` | ✅ 193 PNG |
| Akuarii33 Simple Park | `City_Park/Akuarii33_Park/` | ✅ `park assets.png` + aseprite |
| Stealthix Fireworks (itch) | `VFX_Fireworks/Stealthix_Fireworks/` | ✅ 11 explosion sheets |
| zip 원본 | `_Downloads/Fireworks.zip` 등 | ✅ |

Filter Mode = Point · PPU 32 — Unity 재임포트 후 `Apply Fireworks Park Themes (S9-S11)` 필요.

### T4 지하철 (무료 데모 → 유료)

| 팩 | 링크 | 풀 위치 |
|---|---|---|
| SakPix Cyberpunk Side-Scroller **DEMO** | https://sakpix.itch.io/cyberpunk-side-scroller-tileset-32x32 | `City_Subway/SakPix_Demo/` |
| Atomic Realm Industrial **FREE** | https://atomicrealm.itch.io/industrial-tileset | `City_Subway/Atomic_Industrial_Free/` |
| CraftPix Free Industrial | https://free-game-assets.itch.io/free-industrial-zone-tileset-pixel-art | `City_Subway/IndustrialZone_CraftPix/` |
| Cute SCKR Subway (유료 $3.99) | https://comshadow.itch.io/modern-subway-station-tileset-pack | `City_Subway/CuteSCKR_Subway/` |
| SakPix Full (유료 ~$4) | 위와 동일 페이지 | `City_Subway/SakPix_Full/` |

### 선택

| 팩 | 링크 | 풀 위치 |
|---|---|---|
| 49Wolf Smileys (이모티콘 대안 CC0) | https://49wolf.itch.io/smileys-pack-1 | `Emotes/49Wolf/` |
| PixelPossum Cafe (OGA png와 동일 원본) | https://pixelpossumstudio.itch.io/cafe-and-tea-shop-2d | 이미 `City_Cafe/` 있음 |

---

## 빠른 수동 절차

1. itch.io 로그인  
2. 위 링크마다 **No thanks, just take me to the downloads** (NYOP $0)  
3. zip 저장 → `05.Asset/_Downloads/`  
4. 해당 폴더로 압축 해제  
5. Unity에서 임포트 후 Texture Type=Sprite, Filter=Point, Compression=None
