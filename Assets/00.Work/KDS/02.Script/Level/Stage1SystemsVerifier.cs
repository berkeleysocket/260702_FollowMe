using System.Text;
using UnityEngine;

namespace FollowMe.KDS
{
    /// <summary>
    /// Stage1 필수 시스템·트리거 연결 상태를 Play 시작 시 검증.
    /// </summary>
    public class Stage1SystemsVerifier : MonoBehaviour
    {
        [SerializeField] private bool _logOnStart = true;

        private void Start()
        {
            if (!_logOnStart) return;
            LogReport(BuildReportStatic());
        }

        public VerificationReport BuildReport() => BuildReportStatic();

        public static VerificationReport BuildReportStatic()
        {
            var sb = new StringBuilder();
            int errors = 0;
            int warnings = 0;

            sb.AppendLine("[Stage1SystemsVerifier] 연동 검증");

            // 맵 모드 존/서비스는 현재 미사용(추후 추격 도입 시 재연결)
            if (MapModeService.Instance != null)
                sb.AppendLine($"  · MapModeService (모드={MapModeService.GetDisplayName(MapModeService.Instance.CurrentMode)})");
            else
                sb.AppendLine("  · MapModeService 없음 (의도적 미사용)");

            if (CheckpointService.Instance == null)
            {
                sb.AppendLine("  ✗ CheckpointService 없음");
                errors++;
            }
            else
            {
                sb.AppendLine($"  ✓ CheckpointService (CP={CheckpointService.Instance.LastCheckpointId})");
            }

            if (SocialScoreService.Instance == null)
            {
                sb.AppendLine("  ✗ SocialScoreService 없음");
                errors++;
            }
            else
            {
                sb.AppendLine("  ✓ SocialScoreService");
            }

            var player = PlayerRespawn.FindInScene();
            if (player == null)
            {
                var legacy = Object.FindFirstObjectByType<PhotoProbePlayer>();
                if (legacy == null)
                {
                    sb.AppendLine("  ✗ Player 없음 (KSY Player + PlayerRespawn)");
                    errors++;
                }
                else
                {
                    sb.AppendLine("  ⚠ PhotoProbePlayer (KSY Player로 교체 권장)");
                    warnings++;
                }
            }
            else
            {
                sb.AppendLine("  ✓ KSY Player (PlayerRespawn)");
            }

            var dialoguePlayer = Object.FindFirstObjectByType<DialoguePlayer>();
            if (dialoguePlayer == null)
            {
                sb.AppendLine("  ⚠ DialoguePlayer 없음");
                warnings++;
            }
            else
            {
                sb.AppendLine("  ✓ DialoguePlayer");
            }

            var triggers = Object.FindObjectsByType<DialogueTrigger>(FindObjectsSortMode.None);
            sb.AppendLine($"  DialogueTrigger {triggers.Length}개");
            foreach (var t in triggers)
            {
                if (t == null) continue;
                if (!VerifyDialogueTrigger(t, sb))
                    errors++;
            }

            var photoPoints = Object.FindObjectsByType<PhotoPoint>(FindObjectsSortMode.None);
            if (photoPoints.Length > 0)
            {
                sb.AppendLine($"  ⚠ PhotoPoint {photoPoints.Length}개 (S1은 수집 튜토리얼 — S2로 이동 권장)");
                warnings++;
            }
            else
            {
                sb.AppendLine("  ✓ PhotoPoint 0개 (S1 의도)");
            }

            int follows = 0;
            int likes = 0;
            var pickups = Object.FindObjectsByType<YHW.Items.ItemPickup>(FindObjectsSortMode.None);
            foreach (var p in pickups)
            {
                if (p == null) continue;
                string n = p.name;
                if (n.StartsWith("Follow_") || n.Contains("heart"))
                    follows++;
                else if (n.StartsWith("Like_") || n.Contains("emoji"))
                    likes++;
            }

            sb.AppendLine($"  Collectibles  Follow≈{follows} Like≈{likes} (YHW ItemPickup {pickups.Length})");
            if (Object.FindFirstObjectByType<SocialItemScoreBridge>() == null)
            {
                sb.AppendLine("  ⚠ SocialItemScoreBridge 없음");
                warnings++;
            }
            else
            {
                sb.AppendLine("  ✓ SocialItemScoreBridge");
            }
            if (follows == 0)
            {
                sb.AppendLine("  ⚠ Follow(heart) 0개 (S1 핵심)");
                warnings++;
            }
            if (likes == 0)
            {
                sb.AppendLine("  ⚠ Like(emoji) 0개");
                warnings++;
            }

            var checkpoints = Object.FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
            if (checkpoints.Length == 0)
            {
                sb.AppendLine("  ⚠ Checkpoint 0개 (S1 스펙: Intro CP 1개)");
                warnings++;
            }
            else
            {
                sb.AppendLine($"  ✓ Checkpoint {checkpoints.Length}개");
            }

            sb.AppendLine(errors == 0
                ? $"완료 — 에러 {errors}, 경고 {warnings}"
                : $"실패 — 에러 {errors}, 경고 {warnings}");

            return new VerificationReport(sb.ToString(), errors > 0);
        }

        private static void LogReport(VerificationReport report)
        {
            if (report.HasErrors)
                Debug.LogError(report.Message);
            else
                Debug.Log(report.Message);
        }

        private static bool VerifyDialogueTrigger(DialogueTrigger trigger, StringBuilder sb)
        {
            bool ok = trigger.HasValidReferences;
            sb.AppendLine(ok
                ? $"    ✓ {trigger.name}"
                : $"    ✗ {trigger.name} — Player/Sequence 미연결");
            return ok;
        }

        public readonly struct VerificationReport
        {
            public VerificationReport(string message, bool hasErrors)
            {
                Message = message;
                HasErrors = hasErrors;
            }

            public string Message { get; }
            public bool HasErrors { get; }
        }
    }
}
