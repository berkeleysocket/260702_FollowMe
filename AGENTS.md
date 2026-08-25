# FollowMe — 통합 개발 지침서

> Cursor가 매 요청마다 자동으로 읽는 **단일 지침 파일**이다. AI 규칙(1~6장)과 사람용 환경 설정 가이드(7장~)를 모두 담는다.
> 이 파일에 내용을 추가할 때는 간결하게 — 여기 적힌 만큼 모든 AI 요청이 무거워진다. 긴 문서는 `Docs/`에 두고 여기서 포인터만 남길 것.

---

## 1. 프로젝트 핵심

- **모든 응답은 한국어로.**
- **「팔로우 미 (Follow Me)」** — Unity **6000.3.11f1** · 2D URP · PC · 횡스크롤 러닝 액션 플랫포머. 2026 통합사회 사회게임 제작 과제.
- 주제: **SNS 중독으로 인한 우울증.** 핵심 루프: 맵의 좋아요/팔로우를 수집(목표 100만 → 달성 후 첫 스테이지 회귀, 목표 1,000만). 일정 시간 못 모으면 **우울증 괴물** 스폰 — 저항 불가·회피만 가능, 좋아요를 다시 모으면 디스폰, 팔로워 감소 시 스폰 가속. 총 **16스테이지, 엔딩 1개** — 스테이지 16은 괴물 디스폰 불가라 강제 게임 오버가 되고, 그것이 곧 엔딩이다.
- 팀: 유현우(팀장·YHW) / 강승영(KSY) / **김동선(KDS — 이 사용자, 맵 제작 + 스토리 담당)**

## 2. 폴더 소유권 (엄수)

- `Assets/00.Work/KDS/` — 이 사용자의 작업 폴더. **모든 작업은 여기서만**: `01.Scene` / `02.Script` / `08.SO`
- `Assets/00.Work/KSY/`, `Assets/00.Work/YHW/` — 팀원 폴더. **절대 수정 금지.**
- 서드파티(`Assets/Plugins`, `Assets/Hierarchy Designer`) 수정 금지.
- **KDS 맵·레벨 설계 상세**는 `Assets/00.Work/KDS/AGENTS.md`에만 둔다. KSY/YHW 작업·에이전트는 그 파일을 **읽거나 수정하지 말 것.** 루트 이 파일에는 팀 공통만 유지.

## 3. 기술 규칙

- 입력은 **Input System** (legacy `Input.GetKey` 금지) · 트윈은 **DOTween** 사용 가능 · URP 2D 렌더러.
- `.meta` 파일 직접 편집/삭제 금지. 에셋 경로는 `Assets/` 기준, 슬래시(`/`).
- 씬/프리팹 대량 수정 전에 현재 씬 저장 여부 확인.
- **에디터 연동은 Cursor 전용, Unity 원격 제어는 Unity MCP 전용.** 다른 에디터 연동이나 HTTP 브리지류를 추가·제안하지 말 것.

## 4. C# 컨벤션

- 클래스/메서드/프로퍼티 PascalCase, private 필드는 기존 파일 스타일 우선(없으면 `_camelCase`).
- MonoBehaviour 파일명 = 클래스명. 새 스크립트는 `Assets/00.Work/KDS/02.Script/`, ScriptableObject 데이터는 `Assets/00.Work/KDS/08.SO/`.
- 참조 주입은 `[SerializeField]` 우선. `FindObjectOfType`/`GameObject.Find` 남용 금지.

## 5. 컴파일 검증 (Unity MCP) — 코드 수정 후 반드시

1. `refresh_unity`(compile: request)로 에셋 리프레시 + 재컴파일, ready 대기
2. `read_console`(types: error, warning)로 에러 확인
3. 에러가 있으면 수정 후 1부터 반복. **에러 0건 확인 전에는 "컴파일 된다"고 단정하지 말 것**

Unity MCP가 응답하지 않으면 = Unity가 꺼져 있거나 연동이 끊긴 상태. 사용자에게 확인을 요청하고, 추측으로 완료 보고 금지.

## 6. 스토리·스테이지·맵

> 대사 전문, 연출안, 맵별 체크리스트는 **`Docs/Story_FollowMe.md`** — 필요할 때만 열 것.

- 주인공 **"하루"**: 좋아요를 좇는 인플루언서 지망생. Act가 진행될수록 달리는 자세가 폰 쪽으로 숙여짐.
- 괴물 **"언팔로워"**: 빛을 빨아들이는 검은 덩어리. 알림음이 늘어지는 소리로 접근 예고. 저항 불가, 회피만 가능.
- 진행 방식: 컷씬 없는 인게임 자막(산나비식 — **연출 방식**일 뿐 시나리오 차용 아님) + 달리기=인생·갈림길 선택·엔딩 정산(인생게임식). **엔딩은 1개 고정**, 갈림길 선택은 정산 수치만 바꾼다.

| Act | 스테이지 | 테마 | 계절 | 레벨 키워드 |
|---|---|---|---|---|
| 1 처음 | 1~3 | 홍대/강남 번화가 | 봄 | 평탄, 좋아요 저지대 다량, 위협 없음, 밝음 |
| 2 달콤함 | 4~8 | 성수/종로/동대문 카페 | 여름 | 좋아요 위해 동선 꺾기 시작, 골목 그늘에 첫 괴물 |
| 3 불꽃 | 9~11 | 서울 불꽃축제 | 가을 | 불꽃 타이밍 연동 좋아요 대량 생성, 후반 공허 구간 |
| 4 터널 | 12~14 | 지하철 | 겨울 | 좁고 어두움, 좋아요 희소·위험 배치, 빠른 괴물, 막차 추격 세트피스 |
| 5 굴레 | 15~16 | Act1 맵 재사용 | 봄 | 15: 채도 다운·요소 제거 / 16: 무적 괴물 → 강제 게임오버 = 엔딩 |

맵 공통: 막마다 갈림길 1~2곳(상단=좋아요 밀집 / 하단=일상 오브젝트), 세트피스용 직선 구간 1곳, 계절 팔레트 구분.

---

## 7. 환경 설정 가이드 (사람용 — AI는 참조만)

### 7-1. Cursor를 Unity 외부 에디터로

1. Unity → **Window → Package Manager → + → Add package from git URL**:
   `https://github.com/boxqkrtm/com.unity.ide.cursor.git` (패키지명 `com.boxqkrtm.ide.cursor`)
2. **Edit → Preferences → External Tools → External Script Editor**에서 **Cursor** 선택.
   없으면 `Browse...`로 `%LOCALAPPDATA%\Programs\cursor\Cursor.exe` 지정.
3. 같은 화면 **Regenerate project files** 클릭 → 스크립트 더블클릭 시 Cursor로 열리면 성공.

### 7-2. C# 언어 지원

- MS **C# Dev Kit은 Cursor에서 라이선스상 사용 불가.** Cursor 확장 탭에서 **"C#"**(Anysphere) 설치.

### 7-3. Unity MCP 연결

1. Unity → **Window → MCP for Unity** → Client에서 **Cursor** 선택 → **Configure**.
2. Cursor 재시작 → **Cursor Settings → MCP**에서 UnityMCP 초록불 확인.
3. 테스트: 에이전트 채팅에 "현재 씬 게임오브젝트 목록 보여줘" → MCP 도구 호출이 뜨면 성공.

### 7-4. 인덱싱 제외

`.cursorignore`가 서드파티(`Assets/Plugins`, `Hierarchy Designer`)를 AI 인덱싱에서 제외해 토큰과 검색 노이즈를 줄인다. `Library/Temp` 등은 `.gitignore`로 이미 제외된다.

### 7-5. 이행 체크리스트

- [ ] Cursor 설치, **저장소 루트**로 프로젝트 열기 (`Assets/`만 열면 이 파일이 로드 안 됨)
- [ ] 7-1 외부 에디터 등록 + Regenerate project files
- [ ] 7-2 C# 확장 설치, 자동완성 확인
- [ ] 7-3 MCP 연동 초록불 확인
- [ ] 에이전트에게 "이 프로젝트 규칙 뭐 알고 있어?"라고 물어 이 파일 로드 확인

## 8. AI 사용 수칙 (토큰 절약)

1. **작업 단위마다 새 채팅** — 긴 채팅은 매 턴 전체 내용을 다시 보낸다.
2. **@ 참조를 좁게** — `@Codebase`보다 `@파일`·`@폴더` (예: `@Assets/00.Work/KDS/02.Script`).
3. **긴 문서 붙여넣기 금지** — `@Docs/Story_FollowMe.md`처럼 참조로.
4. **에러는 로그 원문 줄만** 붙여넣기.
5. 간단 수정은 작은 모델, 시스템 설계는 큰 모델 (Auto 모드면 자동).
6. 이 파일이 답할 질문은 묻지 않기.
7. 같은 지시를 세 번째 타이핑 중이면 이 파일에 추가할 신호 — 단, 간결하게.
