using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// 이모티콘·정지 연출 컷씬 재생기. 시작 시 시네마스코프 레터박스를 켠다.
    /// </summary>
    public class CutscenePlayer : MonoBehaviour
    {
        public static CutscenePlayer Instance { get; private set; }

        [SerializeField] private CinemascopeLetterbox _letterbox;
        [SerializeField] private bool _instantLetterbox;

        private Coroutine _playRoutine;

        public bool IsPlaying { get; private set; }

        public event Action CutsceneStarted;
        public event Action CutsceneEnded;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (_letterbox == null)
                _letterbox = GetComponent<CinemascopeLetterbox>();

            if (_letterbox == null)
                _letterbox = FindFirstObjectByType<CinemascopeLetterbox>();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void PlayFromJson(string jsonFileName)
        {
            if (!TryLoadSequence(jsonFileName, out CutsceneSequenceJson sequence))
            {
                Debug.LogWarning($"[CutscenePlayer] JSON 로드 실패: {jsonFileName}", this);
                return;
            }

            Play(sequence);
        }

        public void Play(CutsceneSequenceJson sequence)
        {
            if (sequence == null || sequence.beats == null || sequence.beats.Length == 0)
            {
                Debug.LogWarning("[CutscenePlayer] 비어 있는 컷씬 시퀀스입니다.", this);
                return;
            }

            if (_playRoutine != null)
                StopCoroutine(_playRoutine);

            _playRoutine = StartCoroutine(PlayRoutine(sequence));
        }

        public void Stop()
        {
            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
                _playRoutine = null;
            }

            EndCutscene();
        }

        private IEnumerator PlayRoutine(CutsceneSequenceJson sequence)
        {
            IsPlaying = true;
            CutsceneStarted?.Invoke();

            if (_letterbox != null)
                _letterbox.Show(_instantLetterbox);

            for (int i = 0; i < sequence.beats.Length; i++)
            {
                var beat = sequence.beats[i];
                if (beat == null)
                    continue;

                if (!string.IsNullOrEmpty(beat.characterId))
                {
                    var speaker = DialogueSpeaker.FindById(beat.characterId);
                    speaker?.ApplyExpression(beat.expressionId);
                }

                float wait = Mathf.Max(0.01f, beat.durationSeconds);
                yield return new WaitForSecondsRealtime(wait);
            }

            EndCutscene();
            _playRoutine = null;
        }

        private void EndCutscene()
        {
            if (!IsPlaying && _playRoutine == null)
                return;

            IsPlaying = false;

            if (_letterbox != null)
                _letterbox.Hide(_instantLetterbox);

            CutsceneEnded?.Invoke();
        }

        private static bool TryLoadSequence(string jsonFileName, out CutsceneSequenceJson sequence)
        {
            sequence = null;
            if (string.IsNullOrWhiteSpace(jsonFileName))
                return false;

            string json = null;

#if UNITY_EDITOR
            string fullPath = CutsceneJsonPaths.GetFullPath(jsonFileName);
            if (File.Exists(fullPath))
                json = File.ReadAllText(fullPath);
#endif

            var textAsset = Resources.Load<TextAsset>($"Cutscenes/{jsonFileName}");
            if (textAsset != null)
                json = textAsset.text;

            if (string.IsNullOrEmpty(json))
                return false;

            sequence = JsonUtility.FromJson<CutsceneSequenceJson>(json);
            return sequence != null;
        }
    }
}
