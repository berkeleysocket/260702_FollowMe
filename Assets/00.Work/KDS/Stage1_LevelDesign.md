# Stage 1 — 「처음」 레벨 디자인 (Act 1)

> KDS 전용. 위협·장애물 모드 **없음**. 그레이박스 → 타일셋 교체 순.  
> 공통 규칙: `LevelDesign_Guide.md` — **아트=기획서 / 레벨=스마일모식**

## 목표 감각
- **아트(기획서)**: 홍대/강남 번화가, 봄, 밝음, 네온·버스킹·전광판
- **레벨(스마일모)**: 초반 튜토리얼 러닝 — Intro→Teach→Breath→Setpiece→Goal. 위험 비트 없음
- 평탄한 러닝 + **저지대** 좋아요 (닿기 쉬움). 개수는 **첫 도파민** 수준
- S1 독백: *"계정 만드는 데 1분도 안 걸렸네."* → 클리어 *"어? 좋아요… 눌러 준 사람이 있네."*
- Act 체크리스트 「넉넉하게」는 **높이(저지대·쉽게 닿음)** 의미. S1 개수는 과밀 금지 → S2~S3에서 늘림

## 월드 스케일
- 단위: 1u ≈ 1m 감각, 총 길이 **X 0 → 130**
- 지면 Y ≈ **0**, 카메라 orthographic size ≈ 5~6
- 플레이어 스폰: **(-2, 1)**

## 구간 배치

| 구간 | X 범위 | 이름 | 플레이 | 좋아요 |
|---|---|---|---|---|
| A | 0–20 | Intro | 평지, 조작 익히기 | **2** (저지대) |
| B | 20–45 | LikeTaste | 첫 도파민 맛보기. 간격 두고 배치 | **9** (저지대) |
| C | 45–70 | Fork1 | 상단=플랫폼+좋아요 / 하단=일상 | 상단 **4** |
| D | 70–100 | Setpiece | 직선 번화가·네온. 연출 우선 | **4** (저지대) |
| E | 100–120 | Fork2 | 짧은 갈림길 | 상단 **2** |
| F | 120–130 | Goal | 스테이지 종료 | 0 |
| | | | **합계** | **≈21** (일상 5 → 비율 ~4:1) |

## 하이어라키 (씬)

```
Level_S1
  Background          # 패럴랙스 스프라이트 (하늘/원경)
  Grid                # Tilemap
    Tilemap_Ground    # 바닥 + CompositeCollider2D
    Tilemap_Platform  # 갈림길 발판 + CompositeCollider2D
    Tilemap_Midground # 건물 중경 (콜라이더 없음)
    Tilemap_Props     # 데코 오브젝트 타일
  Collectibles        # Likes / Daily (인터랙션 GO)
  PhotoPoints
  Zones / Lighting / PlayerSpawn
```

타일 에셋: `05.Asset/City_Modern/Tiles/{Floor,Building,Decoration}/`

## 사진 포인트 (포토존)
범위 안 + **E 홀드(Interact)** 로 촬영 → 좋아요·팔로우 **대폭** 상승 (1회성).

| ID | 위치(대략) | 보상(기본) | 연출 의도 |
|---|---|---|---|
| Photo_ForkViewpoint | C 갈림길 ~58 | ♡6,000 / Follow 250 | 거리 풍경 인증샷 |
| Photo_NeonBillboard | D 전광판 ~85 | ♡12,000 / Follow 500 | SNS 광고판 앞 인증 |
| Photo_Busking | D 버스킹 ~92 | ♡8,000 / Follow 300 | 버스킹·셀카 문화 |

SO: `08.SO/PhotoPoint/` · 스크립트: `02.Script/Social/`  
테스트용 `PhotoProbePlayer` 포함 (팀 플레이어로 교체 예정).

## Act1 제약
- 언팔로워·장애물 활성 **배치하지 않음**
- 갈림길은 막힘 없이 둘 다 통과 가능 (손해는 좋아요 개수뿐)
- 좋아요는 **낮은 Y** 우선 (Act 스토리: 손만 뻗으면 닿음)
