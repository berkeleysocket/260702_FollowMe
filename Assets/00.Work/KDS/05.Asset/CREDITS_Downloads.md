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

Filter Mode = Point 로 임포트할 것.

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

### T3 공원

| 팩 | 링크 | 풀 위치 |
|---|---|---|
| Free Park Zone | https://free-game-assets.itch.io/free-green-zone-tileset-pixel-art | `City_Park/ParkZone_CraftPix/` |
| Akuarii33 Simple Park | https://akuarii33.itch.io/simple-park-and-modular-building-assests | `City_Park/Akuarii33_Park/` |

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
