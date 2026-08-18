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

[2026-08-18 16:57] 맵 모드 규칙 확정
- 안정/경고/추격/회복/굴레(S16) — 5단계 상태
- 장애물 미리 배치 → 상태에 따라 활성/비활성 토글
- 안정→추격 직행 금지, 경고 구간 필수
- Act1: 위협 거의 없음 / Act2: 골목 첫 괴물 / Act3: 불꽃 후반 공허 / Act4: 좁고 어둡고 빠름 / Act5: 회복 불가

[2026-08-18 16:57] Stage1 Scene 작업 중
- `Assets/00.Work/KDS/01.Scene/Stage1 Scene.unity`
- City_Modern 타일 에셋 도입 (GandalfHardcore City Tiles 32x32)
- 포토존(PhotoPoint) 시스템 배치 완료

[2026-08-18 16:57] 대사 트리거 연동 필요
- 맵 Collider 진입 시 `DialoguePlayer.StartDialogue()` 호출하는 트리거 아직 없음
- Timeline Signal 또는 단순 Trigger 스크립트 필요 → 개발자에게 요청

---

## 📋 아트 디렉터 (Art Director)

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
- **Timeline 역할**: 세트피스/카메라/타이밍만. 대사는 JSON/DialoguePlayer — 확정
- **엔딩**: 1개 고정. 갈림길은 정산 수치만 변경 — 확정
- **에이전트 소통**: `Docs/AgentBoard.md`로 비동기 공유 — 확정
