# Stage 1 — 「처음」 레벨 디자인 (Act 1)

> KDS 전용. **수집 튜토리얼만.** 포토존·적 **없음** (S2=포토, S3=적).  
> 공통 규칙: `LevelDesign_Guide.md`

## 이 스테이지가 가르치는 것

> **달리면서 아이템을 먹으면, 팔로우와 (좋아요) 게이지가 오른다.**

| 아이템 | 프리팹 (YHW — 수정 금지, 참조만) | 효과 |
|---|---|---|
| **하트** | `Assets/00.Work/YHW/YHW/Prefabs/Items/Pickup_heart.prefab` | **팔로우** ↑ |
| **이모지** | `…/Pickup_emoji_*.prefab` (smile/love/cool/…) | **좋아요** ↑ (게이지) |
| 포토존 | — | **배치하지 않음** (S2) |
| 적 | — | **배치하지 않음** (S3) |

점수 연동: KDS `SocialItemScoreBridge`가 YHW `PlayerItemCollector` 이벤트를 구독.  
CLI: `unity command stage1-collectibles` → YHW 프리팹을 `Level_S1/Collectibles`에 인스턴스.

## 목표 감각
- **아트**: 홍대/강남 번화가, 봄, 밝음
- **레벨**: Intro→Teach→Breath→Setpiece→Goal. 위험 없음
- 저지대 수집 — 손만 뻗으면 닿음
- 독백: *"계정 만드는 데 1분도 안 걸렸네."* → *"어? 좋아요… 눌러 준 사람이 있네."*

## 월드 스케일
- 총 길이 **X 0 → 130**, 지면 Y≈0, 스폰 **(-2, 1)**

## 구간 배치

| 구간 | X | 이름 | 플레이 | Follow | Like |
|---|---|---|---|---|---|
| A | 0–20 | Intro | 조작 + 첫 하트 | 소량 | 소량 |
| B | 20–45 | Teach | 하트·이모지 맛보기 | 다수 | 다수 |
| C | 45–70 | Fork1 | 상단=이모지 / 하단=하트·일상 예고 | 소량 | 상단 |
| D | 70–100 | Setpiece | 직선 번화가 | 중간 | 중간 |
| E | 100–120 | Fork2 | 짧은 갈림 | 소량 | 상단 |
| F | 120–130 | Goal | 종료 | 0 | 0 |
| | | | **합계(스펙)** | **14** | **12** |

CLI: `unity command stage1-collectibles`

## 하이어라키

```
Level_S1
  Background
  Grid / Tilemap_Ground
  ThemeProps          # 건물 스프라이트
  Collectibles        # Follow_* / Like_* (YHW prefab)
  Zone_Goal           # x≈128, StageGoal 트리거 (클리어)
  Zones / Lighting / PlayerSpawn
```

## Act1 제약 (S1)
- 언팔로워·장애물·포토존 **없음**
- 갈림길은 막지 않음 (손해는 수집량뿐)
- 수집은 **낮은 Y** 우선
