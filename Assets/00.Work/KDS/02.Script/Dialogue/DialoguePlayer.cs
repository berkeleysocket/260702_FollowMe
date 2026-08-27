using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FollowMe.KDS
{
    /// <summary>
    /// KDS JSON을 읽고, 씬의 DialogueSpeaker 위에 산나비식 말풍선을 띄운다.
    /// </summary>
    public class DialoguePlayer : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private string _jsonFileName = "stage01_intro";
        [SerializeField] private TextAsset _jsonOverride;

        [Header("View")]
        [SerializeField] private SpeechBubbleView _bubble;

        [Header("Input")]
        [Tooltip("트리거로 시작할 땐 끄세요. DialogueTrigger가 StartDialogue를 호출합니다.")]
        [SerializeField] private bool _autoStartOnEnable;
        [SerializeField] private string _advanceActionPath = "Player/Interact";

        private DialogueSequenceJson _sequence;
        private int _lineIndex = -1;
        private bool _isPlaying;
        private InputAction _advanceAction;
        private Coroutine _autoAdvanceRoutine;

        private void Awake()
        {
            _advanceAction = InputSystem.actions != null
                ? InputSystem.actions.FindAction(_advanceActionPath, throwIfNotFound: false)
                : null;

            if (_advanceAction == null)
            {
                _advanceAction = new InputAction("DialogueAdvance", InputActionType.Button);
                _advanceAction.AddBinding("<Keyboard>/space");
                _advanceAction.AddBinding("<Mouse>/leftButton");
                _advanceAction.AddBinding("<Gamepad>/buttonSouth");
            }

            if (_bubble == null)
                _bubble = GetComponent<SpeechBubbleView>();

            if (_bubble == null)
                _bubble = gameObject.AddComponent<SpeechBubbleView>();
        }

        private void OnEnable()
        {
            _advanceAction.Enable();
            _advanceAction.performed += OnAdvancePerformed;

            if (_autoStartOnEnable)
                StartDialogue();
        }

        private void OnDisable()
        {
            _advanceAction.performed -= OnAdvancePerformed;
            _advanceAction.Disable();
        }

        public bool IsPlaying => _isPlaying;

        public void StartDialogue()
        {
            StartDialogue(_jsonFileName);
        }

        public void StartDialogue(DialogueSequenceSO sequence)
        {
            if (sequence == null)
            {
                Debug.LogWarning("[DialoguePlayer] DialogueSequenceSO가 비어 있습니다.", this);
                return;
            }

            // SO에 라인이 있으면 바로 재생, 없으면 Export된 JSON(JsonFileName)으로 폴백
            if (sequence.Lines != null && sequence.Lines.Count > 0)
            {
                PlaySequence(sequence.ToRuntime());
                return;
            }

            if (string.IsNullOrWhiteSpace(sequence.JsonFileName))
            {
                Debug.LogWarning($"[DialoguePlayer] SO 라인/JsonFileName이 모두 비어 있습니다: {sequence.name}", this);
                return;
            }

            StartDialogue(sequence.JsonFileName);
        }

        public void StartDialogue(string jsonFileName)
        {
            if (!TryLoadSequence(jsonFileName, out DialogueSequenceJson sequence))
            {
                Debug.LogWarning($"[DialoguePlayer] JSON 로드 실패: {jsonFileName}", this);
                return;
            }

            PlaySequence(sequence);
        }

        private void PlaySequence(DialogueSequenceJson sequence)
        {
            if (_autoAdvanceRoutine != null)
            {
                StopCoroutine(_autoAdvanceRoutine);
                _autoAdvanceRoutine = null;
            }

            _sequence = sequence;
            _isPlaying = true;
            _lineIndex = -1;
            ShowNextLine();
        }

        public void EndDialogue()
        {
            _isPlaying = false;
            _lineIndex = -1;

            if (_autoAdvanceRoutine != null)
            {
                StopCoroutine(_autoAdvanceRoutine);
                _autoAdvanceRoutine = null;
            }

            if (_bubble != null)
                _bubble.Hide();
        }

        private void OnAdvancePerformed(InputAction.CallbackContext _)
        {
            if (!_isPlaying || _sequence == null)
                return;

            if (_autoAdvanceRoutine != null)
                return;

            ShowNextLine();
        }

        private void ShowNextLine()
        {
            _lineIndex++;

            if (_sequence.lines == null || _lineIndex >= _sequence.lines.Length)
            {
                EndDialogue();
                return;
            }

            var line = _sequence.lines[_lineIndex];
            var speaker = DialogueSpeaker.FindById(line.characterId);

            if (speaker != null)
                speaker.ApplyExpression(line.expressionId);
            else if (!string.IsNullOrEmpty(line.characterId))
                Debug.LogWarning($"[DialoguePlayer] DialogueSpeaker를 찾지 못함: {line.characterId}");

            Transform follow = speaker != null ? speaker.BubbleAnchor : null;
            bool showPrompt = !line.autoAdvance;
            _bubble.Show(line.text, follow, showPrompt);

            if (line.autoAdvance)
            {
                if (_autoAdvanceRoutine != null)
                    StopCoroutine(_autoAdvanceRoutine);
                _autoAdvanceRoutine = StartCoroutine(AutoAdvanceAfter(line.autoAdvanceSeconds));
            }
        }

        private IEnumerator AutoAdvanceAfter(float seconds)
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0.01f, seconds));
            _autoAdvanceRoutine = null;

            if (_isPlaying)
                ShowNextLine();
        }

        private bool TryLoadSequence(string jsonFileName, out DialogueSequenceJson sequence)
        {
            string json = null;

            if (_jsonOverride != null)
                json = _jsonOverride.text;

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(json))
            {
                string fullPath = DialogueJsonPaths.GetFullPath(jsonFileName);
                if (File.Exists(fullPath))
                    json = File.ReadAllText(fullPath);
            }
#endif

            if (string.IsNullOrEmpty(json))
            {
                sequence = null;
                return false;
            }

            sequence = JsonUtility.FromJson<DialogueSequenceJson>(json);
            return sequence != null;
        }
    }
}
