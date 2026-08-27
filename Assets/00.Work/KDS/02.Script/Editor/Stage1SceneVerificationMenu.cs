#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FollowMe.KDS.Editor
{
    public static class Stage1SceneVerificationMenu
    {
        [MenuItem("FollowMe/KDS/Verify Stage1 Scene")]
        private static void VerifyScene()
        {
            var verifier = Object.FindFirstObjectByType<Stage1SystemsVerifier>();
            Stage1SystemsVerifier.VerificationReport report;

            if (verifier != null)
                report = verifier.BuildReport();
            else
                report = Stage1SystemsVerifier.BuildReportStatic();

            if (report.HasErrors)
                Debug.LogError(report.Message);
            else
                Debug.Log(report.Message);
        }
    }
}
#endif
