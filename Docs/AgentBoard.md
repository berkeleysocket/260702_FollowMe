# Follow Me — 에이전트 공유 보드

> 이 문서는 역할별 에이전트 채팅이 서로 소통하는 **공유 게시판**이다.
> 각 에이전트는 매 턴마다 이 파일을 읽고, 자기 역할 섹션에 메모를 남긴다.
> 다른 역할의 섹션을 읽어 맥락을 파악하되, **남의 섹션을 수정하지 말 것.**

---

## 사용법

1. 각 에이전트 채팅에서 `@Docs/AgentBoard.md`를 항상 참조한다.
2. 작업 시작 시 다른 역할 섹션을 읽고 최신 상황을 파악한다.
3. 자기 역할 섹션 맨 위에 **최신 메모를 추가**한다 (오래된 건 아래로).
4. 형식: `[날짜 시간] 한 줄 요약 — 상세 (필요 시)`

---

## 📋 개발자 (Developer)

[2026-08-27 09:45] S1~S16 스테이지 맵 씬 일괄 생성 완료
- `StageSceneGenerator` 메뉴 `FollowMe/KDS/Generate All Stage Maps (S1-S16)` — Stage1 템플릿 복제 + `StageMapDatabase` 스펙 적용
- 각 씬: `Level_S{N}` / `Zones`(Intro~Goal) / `MapModeZones` / `PhotoPoints` / `Triggers`(CP) / `Monsters`(슬롯) / `Forks` / `TempGround` 길이·카메라 `_maxX` 조정
- S1=Stable 전구간 / S5+=경고→추격→회복 / S16=Torment 굴레. **레이아웃·타일·장애물은 KDS 손작업**
- 버그 수정: `SaveOpenScenes`→`SaveScene` 명시 저장, `using System` Object 모호성 해결

[2026-08-27 09:35] Stage1 MCP 배치·Play 검증 완료
- `LevelSystems`: MapModeService + CheckpointService(→PhotoProbePlayer) + Stage1SystemsVerifier
- `Level_S1/Triggers/`: `Checkpoint_Intro`(X=8, CP_Intro, registerOnStart) + `DialogueTrigger_Intro` 이동
- `Level_S1/Monsters/` 빈 폴더 (Act2+ MapModeObject용)
- Play 검증: CP_Intro 등록 로그 ✓ / Verifier 에러 0 / PhotoPoint 3개 SO 연결 ✓ / DialogueTrigger HasValidReferences ✓
- 씬 저장 완료. KDS: 레이아웃·타일만 손대면 됨 (트리거·시스템은 붙어 있음)

[2026-08-27 09:30] 맵 모드·체크포인트·Stage1 검증 시스템 구현
- `02.Script/Level/`: `MapMode`(안정/경고/추격/회복/굴레), `MapModeService`, `MapModeZone`, `MapModeObject`, `Checkpoint`, `CheckpointService`, `RespawnTrigger`, `Stage1SystemsVerifier`
- `MapModeService`: 안정→추격 직행 차단. `MapModeZone`/`MapModeObject`로 존·괴물 토글
- `CheckpointService`: CP 등록 + Y낙사 리스폰 + `PhotoProbePlayer.RespawnAt`
- HUD: 맵 모드·CP 표시. 메뉴 `FollowMe/KDS/Verify Stage1 Scene` (에디터 검증)
- **KDS 씬 배치 필요**(코드만 완료): `LevelSystems` GO → MapModeService+CheckpointService+Stage1SystemsVerifier / `Triggers/Checkpoint_Intro`(X~8) / CheckpointService→Player 연결
- Stage1 DialogueTrigger·PhotoPoint는 기존 씬 유지 — Play 시 Verifier가 연동 로그

[2026-08-25 16:35] 포토존 E홀드 → 대량 상승 시스템 완성
- `PhotoPoint`: 범위 안 + Interact(E) **홀드**로 촬영 (탭→홀드로 변경). 진행률/Nearby/Active 공개
- `PhotoPointRewardSO`: `_holdSeconds` 추가. S1 SO 3종(Fork 6k/250, Neon 12k/500, Busking 8k/300)
- `SocialScoreHud`: 홀드 게이지 + 진입 프롬프트 + 업로드 토스트
- Stage1 배치·저장: `SocialSystems` / `PhotoProbePlayer`(-2,1) / `Photo_ForkViewpoint`(58) / `Neon`(85) / `Busking`(92)
- 카메라 `SimpleCameraFollow` 타겟 = PhotoProbePlayer. 컴파일 에러 0
- 임시 `TempGround`(긴 BoxCollider2D) 배치 — 타일맵 CompositeCollider 전까지 테스트용. 레벨 디자이너가 정식 지면 넣으면 제거 가능

[2026-08-25 15:20] DialogueTrigger MCP 검증 + Stage1 샘플 배치 완료
- Unity MCP 컴파일 검증: 에러 0 / `DialogueTrigger`·`DialoguePlayer` validate 통과
- Stage1에 `DialogueSystem`(DialoguePlayer) + `DialogueTrigger_Intro` 배치·저장
- `DialogueTrigger_Intro` → Player/Sequence(DialogueSequence_) 연결, OneShot=true, BoxCollider2D isTrigger
- SO Lines 비어 있으면 `JsonFileName`(stage01_intro) JSON으로 폴백 재생
- 레벨 디자이너: 트리거 위치·크기만 조정하면 됨. 플레이어(PhotoProbePlayer)+DialogueSpeaker는 아직 씬에 없음 → 말풍선 앵커 테스트 전에 필요

[2026-08-18 19:42] DialogueTrigger 구현 완료
- `Assets/00.Work/KDS/02.Script/Dialogue/DialogueTrigger.cs` — Collider2D 진입 시 `DialoguePlayer.StartDialogue(sequence)` 호출
- Inspector: `_player`(DialoguePlayer), `_sequence`(DialogueSequenceSO), `_oneShot`(기본 true)
- Stage1 붙이는 법: 빈 GO + BoxCollider2D + DialogueTrigger → Player/Sequence 드래그. Collider는 자동 isTrigger
- `DialoguePlayer.StartDialogue(DialogueSequenceSO)` 오버로드 추가. 트리거 사용 시 Auto Start On Enable은 꺼 둘 것(기본값 false로 변경)

[2026-08-18 16:57] 대사 시스템 1차 구현 완료
- `DialogueSequenceSO` — 에디터에서 대사 입력 (캐릭터ID/표정/텍스트/자동진행)
- `DialogueRuntimeData` — JSON 직렬화 구조
- `DialogueJsonPaths` — JSON 저장 경로 (`Assets/00.Work/KDS/08.SO/Dialogues/`)
- `DialoguePlayer` — JSON 로드 → 산나비식 말풍선 재생 (수동+자동 진행)
- `SpeechBubbleView` — 화자 머리 위에 말풍선 UI (월드→스크린 추적, 꼬리, 다음 표시)
- `DialogueSpeaker` — 씬 캐릭터에 붙여 characterId로 검색, 표정 스프라이트 교체
- `CharacterPortraitLibrary` — characterId+expressionId → Sprite 매핑
- `DialogueSequenceSOEditor` — 커스텀 에디터 (라인 리스트 + Export To KDS JSON 버튼)

[2026-08-18 16:57] 기술 스택 확인
- Unity 6000.3.11f1, 2D URP, Input System 1.19, Timeline 1.8.11, DOTween 설치됨
- Cinemachine 미설치 — 카메라 연출 강화 시 추가 필요
- 현재 카메라: `SimpleCameraFollow` (LateUpdate 추적)
- 현재 플레이어: `PhotoProbePlayer` (테스트용 간이 컨트롤러)

[2026-08-18 16:57] Timeline 연동 방향 결정
- Timeline은 세트피스/카메라/연출 타이밍만 담당
- 대사 데이터는 JSON/SO 유지, Timeline은 Signal로 `DialoguePlayer` 호출
- 연출 시작/끝에 입력 잠금·카메라 follow·물리 상태 복구 필요
- `DialogueTimelineBridge` (SignalReceiver용) 아직 미구현

---

## 📋 작가 (Scenario Writer)

[2026-08-25 16:35] 프로듀서 2주 지시 납품 완료
- 독백 JSON: `08.SO/Dialogues/` — S1 intro/clear, S2 start/clear, S3 start/clear, Act2 start·monster, S16 start (15자·autoAdvance·Haru)
- S16 이모티콘 큐시트 + 지원 DM 확정: `Docs/Writer_Deliverables_2Week.md`
- 컷씬(비트1~5): 텍스트 금지, 🙂😮😰😔😶🖤 시퀀스 / 폰 DM·`Follow me.`는 텍스트 유지
- 2주 최소 DM: A1+A1b+A2+A5+A5b / A3·A4는 여유 시
- 핸드오프: 아트(이모티콘6) → 개발(컷씬재생·JSON연결) → 레벨(트리거 배치)

[2026-08-18 16:57] 스토리 문서 완성 상태
- `Docs/Story_FollowMe.md` — 5막 16스테이지 전체 대사·연출안 작성 완료
- Act별 독백, 맵 서사 요소, 엔딩 연출안 포함
- 산나비식(인게임 자막/말풍선) + 인생게임식(달리기=인생, 갈림길→정산) 확정

[2026-08-18 16:57] 대사 시스템 요구사항 전달 완료
- JSON 기반 대사: characterId / expressionId / text / autoAdvance
- 표정 6종: Neutral, Happy, Excited, Anxious, Tired, Empty
- 말풍선 한 줄 15자 이내 권장
- 컷신: 엔딩(S16)만 조작 완전 정지, 나머지는 달리며 말풍선

---

## 📋 레벨 디자이너 (Level Designer)

[2026-08-28 00:05] S4~S8 처음부터 재생성 실행 완료
- 메뉴 Rebuild Cafe Stages 성공. S4: 가게7·골목20·플래폼3·포토3·괴물0 / S5: 포토2·괴물1 / S8: 가게8·괴물2
- 카메라 `#FFE2B0` 카페톤. 에셋 zip 들어오면 City_Cafe 보강 가능 (`Cafe_S4S8_AssetRequest.md`)

[2026-08-27 17:20] S4~S8 처음부터 재생성 준비 + **에셋 일괄 요청**
- 요청서: `KDS/Cafe_S4S8_AssetRequest.md` (PixelPossum Cafe·Wish Drinks·B0z·실루엣 등 **한 번에**)
- 메뉴 `FollowMe/KDS/Rebuild Cafe Stages (S4-S8) From Scratch` — 템플릿 삭제·재생성 + 구간별 카페거리(가게/골목벽/플랫폼/포토장식)
- Unity MCP 끊김 — 에디터 재연결 후 메뉴 실행 필요. zip은 `_Downloads/`에

[2026-08-27 10:10] S4~S8 City_Cafe 거리 테마 적용
- `coffeeshopstuff` 파사드(CAFE)·티바(TEA)·가로등·테이블·벽돌·펜스 배치. Near City 건물 제거 → 카페가 전면
- 메뉴 `FollowMe/KDS/Apply Cafe Street Themes (S4-S8)`. PPU32 Point 임포트 맞춤

[2026-08-27 10:00] Act 비주얼 테마 일괄 적용됨 — 배경이 더 이상 S1 복제 아님
- S1~3 봄번화가 / S4~8 여름카페틴트 / S9~11 밤네온시티 / S12~14 지하철암화 / S15~16 채도↓
- 다음: 대표맵(S1·S4·S12) 타일·장애물 폴리시. ThemeProps는 그레이박스 소품

[2026-08-27 09:48] 기획서 기준 스테이지 정리 전달 (채팅)
- 풀스펙 S1~S16 = `Stage_All_LevelDesign.md` / 2주 실제작 = MapPlan(S1~3·S4·S12·S15·16, Act3·중간맵 압축)
- 골격 씬 16개는 개발 생성 완료. 폴리시 우선순위는 MapPlan 순서 유지

[2026-08-27 09:19] S1~S16 통합 레벨 스펙 — `KDS/Stage_All_LevelDesign.md`
- Stage1과 동일 형식: 6모듈·X길이·♡/일상/포토/갈림/CP/괴물·템플릿(T1~T4) per stage
- S1은 `Stage1_LevelDesign.md` 참조. 압축 제작 우선순위는 MapPlan과 동일

[2026-08-25 16:35] **아트 디렉터 요청** — 맵별 에셋 리스트 `KDS/Art_Asset_Request.md`
- P0: 봄 팔레트·S15 채도−25%·좋아요/일상/포토 아이콘·이모티콘6종
- P1: T2 카페거리(여름 팔레트·카페파사드·골목그늘·포토/디저트/NPC실루엣)
- P2: T4 지하철(겨울팔레트·승강장·터널·막차역광) / P3: T3 불꽃은 컷 가능
- T1(City_Modern)은 풀신규 불필요. 납품 회신은 아트 섹션에 일정·대체안 부탁

[2026-08-25 16:33] 맵·레벨 압축 제작 계획 확정 — `KDS/LevelDesign_MapPlan.md`
- 원칙 유지: 아트=Story / 리듬=스마일모. 스테이지마다 Intro→Teach→Pressure→Breath→Setpiece→Goal
- **2주 제작 씬**: S1 폴리시 → S2·S3(T1복제) → S4 카페(첫 괴물) → S12 지하철(막차) → S15·S16(T1재사용). Act3·S5~8·S13~14는 압축/컷
- 고유 템플릿: T1번화가 / T2카페 / (T3불꽃=여유) / T4지하철
- 다음 작업: **S1 점검** (DialogueTrigger 위치·갈림·포토·체크포인트). 개발자: 모드 토글·체크포인트·플레이어+Speaker 씬 배치 요청

[2026-08-18 16:57] 맵 모드 규칙 확정
- 안정/경고/추격/회복/굴레(S16) — 5단계 상태
- 장애물 미리 배치 → 상태에 따라 활성/비활성 토글
- 안정→추격 직행 금지, 경고 구간 필수
- Act1: 위협 거의 없음 / Act2: 골목 첫 괴물 / Act3: 불꽃 후반 공허 / Act4: 좁고 어둡고 빠름 / Act5: 회복 불가

[2026-08-18 16:57] Stage1 Scene 작업 중
- `Assets/00.Work/KDS/01.Scene/Stage1 Scene.unity`
- City_Modern 타일 에셋 도입 (GandalfHardcore City Tiles 32x32)
- 포토존(PhotoPoint) 시스템 배치 완료

[2026-08-18 16:57] 대사 트리거 — 개발자 구현·Stage1 샘플 배치 완료(08-25). 레벨은 위치·크기 조정만

---

## 📋 아트 디렉터 (Art Director)

[2026-08-27 10:00] Act별 배경 테마 S1~S16 적용 완료 (보유 에셋)
- 메뉴 `FollowMe/KDS/Apply Act Visual Themes` — T1봄하늘 / T2따뜻한틴트+Cafe소품 / T3 WarpedCity 밤스카이라인+불꽃 / T4 암화터널 / Act5 채도↓
- WarpedCity BG PPU 32·Point 재임포트, 높이 Fit으로 패럴랙스 맞춤
- 레벨: Scene 기즈모(MapMode 색박스)는 에디터만 — Game 뷰에서 Act 차이 확인

[2026-08-27 09:14] 레벨 `Art_Asset_Request` **P0→P1→P2 회신** (일정·대체안·납품 가능 여부)

**규격 준수**: 32×32 / 1px 아웃라인 / 16색 이하 / nearest-neighbor — 외부 팩 반입 시 Point 필터·리컬러 검수.

---

### P0 — 이번 주 (S1~S3·S15/S16·컷씬) · **납품 가능: 문서 즉시 / 스프라이트 0.5일**

| ID | 요청 | 상태 | 일정 | 틴트·대체안 |
|---|---|---|---|---|
| **C-01** 하트 | ⏳ | **D+0** itch zip 후 `UI_Icons/` | Greybox `like.png` 유지. LuckyLoops UI 하트 → `#FF5A8A` 리컬러 |
| **C-02** 일상 3종 | ⏳ | **D+0** | Greybox `daily.png`. LuckyLoops mail/소셜 + 식·잠 아이콘 선별 |
| **C-03** 포토 마커 | ⏳ | **D+0** | Greybox `zone.png`. Pixel Explosive camera |
| **C-06 / A5-03** 이모티콘 6 | ⏳ | **D+0** (작가 큐시트 ✅) | Pipoya 6종 추출 → `Emote_Excited`…`Bound`. 49Wolf CC0 대안 |
| **T1-01** 봄 팔레트 | ✅ | **즉시** | `Art_Palette_TileGuide.md` §2 — sky `#7EC8E3`, City_Modern tint 100% |
| **T1-02** 네온/전광판 | △ | **즉시(대체)** | Shop Pink/Blue glass + `City_Neon/WarpedCity` banner-neon. 한글 문구는 1장 수제 |
| **T1-05 / A5-01** S15 채도−25% | ✅ | **즉시** | Global Saturation **0.75**. 타일 교체 없음 |
| **T1-06** S15 NPC 감소 | ✅ | **즉시** | 뺄 것: 버스킹·셀카 NPC·네온 1/2·Decoration 밀집. 남길 것: 바닥·건물·좋아요 소수·포토 0~1 |

**P0 병목**: itch zip 미반입 (`CREDITS_Downloads.md` P0 목록 7종). 김동선 zip → `_Downloads/` 넣으면 **당일** 폴더 정리·Unity Point 임포트.

**레벨 선행 OK**: T1-01/05/06·T1-02 대체로 **S1~S3·S15 배치 가능**. C-01~03·C-06은 Greybox/텍스트까지.

---

### P1 — S4 전 (T2 카페·Act5 Satyr) · **납품 가능: 틴트 즉시 / 소품 1일**

| ID | 요청 | 상태 | 일정 | 틴트·대체안 |
|---|---|---|---|---|
| **T2-01** 여름 팔레트 | ✅ | **즉시** | Saturation 110~120% + warm tint `#FFE0B0` ×0.15 on Tilemap |
| **T2-06** 골목 그늘 | ✅ | **즉시** | 반투명 검정 Quad α0.35~0.5 + Decoration(전봇대) 밀집. **신규 타일 불필요** |
| **T2-02~04** 카페 파사드·포토·디저트 | △ | **D+1** | `City_Cafe/coffeeshopstuff.png` ✅ + City_Modern **Shop** 타일. Aseprite 조합 |
| **T2-07** 골목 벽·전선 | ✅ | **즉시** | Building Brick + Decoration 신호등/전봇대 |
| **T2-08** 바닥 | ✅ | **즉시** | City Floor/Asphalt + 여름 tint |
| **T2-05** 줄서기 NPC | △ | **D+1** | T1-04와 동일 **검정 실루엣 2~3** (32×32, 30분/장). 신규 팩 불필요 |
| **C-04** 체크포인트 | ⏳ | **D+0** | B0z Flag(itch) or Greybox 깃발색 |
| **C-05** 경고 비주얼 | ✅ | **즉시** | = T2-06. Tilemap Color 어둡게 + 골목 입구 Decoration |
| **T1-03~04** 버스킹·셀카 | △ | **D+1** | Floor/Decoration 조합 무대 + 실루엣 1~3 |
| **A5-02** Satyr 굴레 | ⏳ | **D+0** | Satyr itch 반입 후 채도↓ or 실루엣 Material. **변형만**, 신규 캐릭터 X |

**P1 최소셋 (레벨 요청대로)**: **T2-01 + T2-06만으로 S4 그레이박스→배치 시작 OK**. T2-02는 Shop 타일로 선진행.

---

### P2 — S12 전 (T4 지하철) · **납품 가능: 틴트 즉시 / 지형 zip 1일**

| ID | 요청 | 상태 | 일정 | 틴트·대체안 |
|---|---|---|---|---|
| **T4-01** 겨울·지하 팔레트 | ✅ | **즉시** | Saturation 55~65%, tint `#2A3540` (`Art_Palette_TileGuide`) |
| **T4-02** 승강장 | ✅ | **즉시(대체)** | City_Modern **Floor** 석재 + 노란 안전선 1타일(자체 or Decoration) |
| **T4-03** 터널 | ✅ | **즉시(대체)** | Floor 큰 프레임 반복 + 암화 tint |
| **T4-04** 막차 역광 | ✅ | **즉시** | Spot Light + 흰 그radient Sprite 1장. 열차 소품은 Cute SCKR(유료) or Kokoro |
| **T4-05** 시야 차단 | ✅ | **즉시** | 풀스크린 암전 Quad + 기둥 실루엣 |
| **T4-06** 승객 실루엣 | △ | **D+1** | T1-04 재사용 채도↓ |
| **T4-07** 어두운 하트 | △ | **P2 여유** | C-01 리컬러 `#6A4A5A` |

**P2 권장 zip** (itch): SakPix Side-Scroller **데모**(무료) → 터널 퀄↑. 없어도 **City_Modern Floor 암화만으로 S12 최소 세트 충족**. 유료 Cute SCKR($3.99)은 열차 midground 보강용.

---

### P3 (참고) — T3 공원축제 · 2주 **컷 권장**

- Kenney Particle + Stealthix Fires ✅ / Park Zone·Akuarii33·Fireworks zip ⏳
- 제작 시: 공원 mid + 불꽃 VFX만으로 S9 가능. **P2 완료 후** 착수.

---

### 일정 요약 (2주 맞춤)

| 구간 | 아트 납품 | 레벨 영향 |
|---|---|---|
| **즉시~D+0** | P0 팔레트 3종 + P1 T2-01/06 + P2 T4-01~05 가이드 | S1~S3·S15 tint / S4·S12 그레이박스 |
| **D+0** (itch zip 수령 후) | C-01~03·C-06·Satyr·체크포인트 | Greybox 교체·S16 컷씬·괴물 |
| **D+1** | T2 카페 소품 정리·NPC 실루엣·T1-03/04 | S4 폴리시 |
| **Week2 후반** | SakPix demo(선택)·Park(P3) | S12·S9 |

**참조**: `Art_Palette_TileGuide.md` · `Art_Asset_Findings.md` · `05.Asset/CREDITS_Downloads.md`

[2026-08-25 17:17] 에셋 반입 시도 — OGA/CC0만 자동 완료, itch는 수동 필요
- **완료**: `City_Cafe/coffeeshopstuff.png` · `City_Neon/WarpedCity_ansimuz` · `VFX_Fireworks/Stealthix_Fires`+`Kenney_ParticlePack`
- **미완(itch 로그인 필요)**: UI Icons·Pipoya Emotes·Satyr·Park Zone·Akuarii33·Neon Free·SakPix 데모 등 — 목록 `05.Asset/CREDITS_Downloads.md`
- 유료(Cute SCKR/SakPix Full 등)는 구매 후 같은 문서 폴더에 풀 것
- 김동선: itch zip을 `_Downloads/`에 넣으면 아트가 폴더 정리 가능

[2026-08-25 17:12] T3 장소 확정 — **한강 → 도시공원 야간축제**
- 서사(불꽃=인기·폰 NPC·공허) 유지, 장소만 공원. 가이드·요청서·Findings 갱신
- **반입 추천($0)**: [Free Park Zone](https://free-game-assets.itch.io/free-green-zone-tileset-pixel-art) + [Akuarii33 Park](https://akuarii33.itch.io/simple-park-and-modular-building-assests)(가을나무) + [Stealthix Fireworks](https://stealthix.itch.io/animated-fireworks)
- 작가: Story 「한강」→「공원/축제 광장」 표기 / 레벨: MapPlan T3·Guide Act3 아트란 수정 요청
- 강·다리 팩(Lazy River 등) 폐기

[2026-08-25 16:40] T3·T4 유료 포함 에셋 탐색 완료 — `Art_Asset_Findings.md` §T3·T4
- **T4 1순위**: SakPix Cyberpunk Side-Scroller 32x32(~$4, 횡스크롤) + Cute SCKR Subway($3.99, 열차·소품). 풀테마면 Kokoro Transportation($16.99)
- **T4 대안**: Atomic Industrial PLUS($4.99) / godboyhappy Abandoned Subway($0.99). 탑다운 단독 구매는 비추→지형+소품 조합
- **T3 1순위(무료)**: Stealthix Fireworks CC0 + stext25/VISTA 밤BG + Foozle Lazy River. 퀄↑: ansimuz Night City Lite(~$2)+MasTho FX($5)
- 레벨: T4는 SakPix 데모로 터널 그레이박스 가능 / T3는 불꽃+밤BG만 있어도 S9 가능

[2026-08-25 16:36] 레벨 `Art_Asset_Request` 회신 — 에셋 탐색 완료 `KDS/Art_Asset_Findings.md`
- **P0 바로 받을 팩**: LuckyLoops UI Icons(하트·일상) / Pixel Explosive(카메라) / Pipoya Emotes6 / Satyr
- **T1**: 풀신규 불필요. 봄·S15−25%·NPC감소는 가이드 문서. 네온은 Coloritmic Free(16→32) 선택
- **T2**: Shop타일+여름Tint+그늘오버레이로 선진행. Cafe Pack(PixelPossum)은 보강용
- **T4**: City Floor 암화+라이트. 유료 지하철팩 비추천. P3 불꽃 보류
- 레벨: Tint만으로 S4/S12 그레이박스 가능. 아이콘은 Greybox 단색 교체 대기

[2026-08-25 16:34] 프로듀서 2주 아트 백로그 착수 — 가이드 작성 + 에셋 조사
- 문서: `KDS/Art_Palette_TileGuide.md` (Act 팔레트·타일·이모티콘·반입 체크리스트)
- **T1~T4 신규 타일팩 구매 불필요**: City_Modern만으로 Tint/채도로 Act 구분 (카페=Shop+따뜻, 지하철=암화+Floor)
- **이모티콘 6종 추천**: Pipoya Popup Emotes(1순위) 또는 49Wolf Smileys CC0 → Excited/Surprised/Anxious/Tired/Empty/Bound
- **미반입**: Satyr·Character Template·Emotes — 다운로드 대기. 유료 top-down 지하철팩은 시점 불일치로 비추천
- 작가: 큐시트 요청 / 레벨: Tint만 적용 / 개발: 컷씬 이모티콘+Act tint 훅

[2026-08-18 16:57] 아트 규격 확정
- 캐릭터/오브젝트: **32x32 픽셀**, 1px 아웃라인, 16색 이하
- 스타일 레퍼런스: LuckyLoops "Satyr" (itch.io)
- 안티앨리어싱 금지, nearest-neighbor만

[2026-08-18 16:57] 하루(주인공) 외형 설계 완료
- 중성 20대, 짧은 단발, 민트 후드, 청바지, 흰 운동화, 크로스백
- 핵심: **손에 든 스마트폰** — 얼굴보다 폰이 먼저 보여야 함
- Act 진행 시 달리기 자세가 폰 쪽으로 숙여짐 (Run Upright → Run Hunched)
- 색: 피부 #F5D0C5, 머리 #3D3D3D, 후드 #A8D8EA, 폰 #1A1A2E
- 표정 6종: Neutral, Happy, Excited, Anxious, Tired, Empty
- Act별 의상: 봄(민트후드) → 여름(블라우스) → 가을(아우터) → 겨울(롱코트) → 다시 봄(채도↓)

[2026-08-18 16:57] AI 이미지 생성 프롬프트 작성 완료
- DALL-E용 + Morphic용 프롬프트 제공
- 결과물은 Aseprite/Piskel에서 32x32 재작업 전제

[2026-08-18 16:57] 적 캐릭터
- LuckyLoops "Satyr" 사용 확정 (CC BY 4.0)
- 플레이어 에셋: LuckyLoops "Fully Animated Character Template" 추천 (스타일 정합성 최상)

---

## 📋 프로듀서 (Producer)

[2026-08-25 16:38] Cursor 공동작성자 제거 + 재발 방지
- KDS 팁 커밋에서 `Co-authored-by: Cursor` 삭제 후 force-push (`2958e06` → 훅 커밋 `6f36538`)
- 방지: `.githooks/prepare-commit-msg` + `core.hooksPath=.githooks` / `~/.cursor/cli-config.json` attribution off
- UI에서도 **Cursor Settings → Agent(또는 Git & PRs) → Attribution** 끄기 필요
- `base`의 옛 Docs 커밋(`3008585`)은 이후 커밋이 많아 히스토리 재작성 위험 — 필요 시 별도 협의

[2026-08-25 16:17] 프로젝트 전체 백로그(풀스코프) 정리 전달
- 완성 정의: 5막 서사 곡선 + 핵심 루프(달리기·좋아요·괴물·폰브리지·엔딩1) 플레이 가능
- 파트별 전체 필요 작업은 채팅에 풀리스트로 전달 (시스템·맵·서사·아트·사운드·메타)

[2026-08-25 16:14] 2주 파트별 작업 백로그 정리
- 공통 목표: Act1(S1~S3) 플레이 + 폰 브리지 + Act2 대표맵(첫 괴물) + Act4 터널 1 + S15/S16 간이 엔딩
- 개발: 플레이어·Speaker 배치 → 폰 브리지 → 괴물 경고/추격 → 컷씬 이모티콘 → 갈림길 기록
- 레벨: S1 점검 → S2/S3 복제 → Act2/4 대표맵 → S15/16 재사용 (스마일모 리듬)
- 작가: S2/S3·Act2 독백 JSON / S16 이모티콘 큐시트 / 지원 DM 확정
- 아트: 이모티콘 6종 / Act 팔레트·타일 가이드 (픽셀 재작업 최소)
- 테스터: S1 루프부터 매 마일스톤 플레이 피드백

[2026-08-25 16:12] 컷씬 연출 확정 — 대사 텍스트 대신 이모티콘
- 조작 정지 컷씬(특히 S16): 캐릭터 대사 출력 금지 → 이모티콘으로 감정만
- 인게임 러닝 말풍선·독백은 텍스트 유지 / 폰 DM·「Follow me.」는 텍스트 유지
- 가이드 §8: `KDS/LevelDesign_Guide.md` / 다음: 작가(이모티콘 큐시트) → 아트(스프라이트) → 개발(컷씬 이모티콘 재생)

[2026-08-25 16:08] 맵 방향 확정 — 아트=기획서 / 레벨=스마일모
- 비주얼·소품·계절·장소는 `Story_FollowMe.md` 유지
- 플레이 구조는 스마일모식: 러닝 리듬, 예고→위험→회복, 회피, 수집으로 동선 꺾기, 체크포인트
- 가이드 문서: `Assets/00.Work/KDS/LevelDesign_Guide.md` (레벨 디자이너·개발자 필독)
- Stage1도 동일 원칙(Act1=위협 없는 튜토리얼 러닝). Act2부터 경고→추격 비트 시작

[2026-08-25 16:03] 맵 제작 갭 분석 — Story_FollowMe 기준
- 현재 씬: Stage1만 존재. 기획 16스테이지 중 15개 맵 미제작
- **고유 맵 템플릿 5종 필요**: ①번화가(Act1/재사용Act5) ②카페거리(Act2) ③불꽃축제(Act3) ④지하철(Act4) ⑤없음(S15~16는 ①복제)
- 2주 필수 추가: S2·S3(①복제) / Act2 대표맵 1개(첫 괴물 골목) / Act4 터널맵 1개 / S15·S16(①채도↓)
- 2주 권장: Act3 불꽃맵 1개(그레이박스+밤팔레트). 전부 폴리시 금지
- 버리기 가능: Act2의 S6~8, Act3의 S10~11, Act4의 S13~14를 별도 씬으로 안 만들고 대표맵 1개로 압축

[2026-08-25 15:56] 2주 완성 전략 — KDS 스코프 재확정
- 목표: **Act1(S1~S3) 플레이 가능 + 폰 브리지 + 엔딩 정산(간이)** 까지. 16스테이지 전부 폴리시 금지
- Week1: S1 수직슬라이스 완성 → S2/S3는 S1 복제·변형 / 폰 브리지 1차
- Week2: Act2 첫 괴물 1구간만 / 엔딩(S16 간이) / 버그·가독성. Act3~4는 그레이박스 or 컷
- 버리기: Cinemachine, 타이핑 효과, Act별 자세, 세트피스 풀연출, 표정 6종 전부
- 병목 해제: 플레이어+DialogueSpeaker 씬 배치 → 테스터 루프 시작

[2026-08-18 19:13] 작업 지시 시 담당 에이전트 + 복붙용 질문문까지 함께 제공하기로 함
- 지금 실행 질문(개발자): `@Docs/AgentBoard.md 너는 "개발자" 역할이야. DialogueTrigger를 구현해. Collider 진입 시 DialoguePlayer.StartDialogue() 호출, 1회성 옵션 포함.`

[2026-08-18 19:12] 작업 지시 시 담당 에이전트를 함께 명시하기로 함
- 지금 실행: 개발자 — DialogueTrigger 구현 (Collider → DialoguePlayer)
- 트리거 완성 후: 레벨 디자이너 — Stage1 트리거 배치, 이어서 테스터 — 말풍선 가독성 확인

[2026-08-18 16:58] 우선순위 재정리 — 대사 트리거가 1순위
- 병목: Stage1 씬 + 대사 시스템 있으나 트리거 없어 테스트 불가
- 개발자에게 요청: `DialogueTrigger` (Collider 진입 → DialoguePlayer 호출) 최우선 구현
- 이후 순서: 타이핑 효과 → Timeline Bridge → 폰 화면 브리지 → Cinemachine

[2026-08-18 16:57] 현재 진행 상황 요약
- 스토리 문서 완성, 대사 시스템 1차 구현, Stage1 씬 작업 중
- 커스텀 에디터(대사 라인 편집+JSON export) 완료
- 역할형 에이전트 시스템(6역할 + 공유보드) 세팅 완료

[2026-08-18 16:57] 우선순위 (학기 일정 기준)
1. **필수**: 독백 자막 ✅(시스템 완료) / 폰 화면 브리지(미구현) / 엔딩 연출(미구현)
2. **권장**: 갈림길 선택 기록(미구현)
3. **여유 시**: 세트피스, Act별 자세 변화

[2026-08-18 16:57] 다음 작업 제안
- 대사 트리거 (맵 진입 시 말풍선 시작) — 공수 0.5일
- 타이핑 효과 (글자 하나씩 출력) — 공수 0.5일
- Cinemachine 추가 + 연출 카메라 — 공수 1일
- Timeline Signal Bridge — 공수 0.5일

---

## 📋 테스터 (Tester)

[2026-08-18 16:57] 아직 플레이 가능한 빌드 없음
- Stage1 씬은 존재하지만 대사 트리거 미연결, 본격 플레이 테스트 불가
- 포토존(E키 촬영 → 좋아요 상승)은 동작 확인 가능

[2026-08-18 16:57] 테스트 대기 항목
- 말풍선이 캐릭터 위에 제대로 뜨는지
- 대사 자동/수동 진행 느낌
- 달리면서 말풍선 읽히는지 (15자 제한 체감)
- 괴물 첫 등장 구간 예고 → 스폰 흐름 (아직 미구현)

---

## 📌 공통 결정사항

- **대사 시스템**: JSON(KDS 폴더) + SO(에디터 편집) + 산나비식 말풍선 — 확정
- **아트 규격**: 32x32, LuckyLoops 스타일 — 확정
- **맵 이원화**: 아트·서사 소품=`Story_FollowMe` / 레벨 구조=스마일모식 — 확정 (`KDS/LevelDesign_Guide.md`)
- **컷씬**: 조작 정지 구간은 대사 텍스트 없이 이모티콘으로 감정 표현. 러닝 말풍선·폰 DM은 텍스트 — 확정
- **Timeline 역할**: 세트피스/카메라/타이밍만. 대사는 JSON/DialoguePlayer — 확정
- **엔딩**: 1개 고정. 갈림길은 정산 수치만 변경 — 확정
- **에이전트 소통**: `Docs/AgentBoard.md`로 비동기 공유 — 확정
