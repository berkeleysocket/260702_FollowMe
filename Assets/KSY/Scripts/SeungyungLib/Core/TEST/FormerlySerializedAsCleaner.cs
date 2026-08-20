using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SeungyungLib.Core.Utility
{
    public class FormerlySerializedAsCleaner : EditorWindow
    {
        [MenuItem("Tools/Clean Up FormerlySerializedAs")]
        public static void CleanUp()
        {
            // 1. 프로젝트 내의 모든 프리팹, 씬 등을 강제로 다시 저장 (데이터 마이그레이션 완료)
            Debug.Log("1/2. 에셋 강제 재직렬화 중... (데이터를 새 이름으로 안전하게 이전합니다)");
            AssetDatabase.ForceReserializeAssets();

            // 2. 스크립트 폴더 내의 .cs 파일들을 돌며 FormerlySerializedAs 구문을 찾아 삭제
            Debug.Log("2/2. 스크립트 코드 정적 정리 중...");
            string[] scriptGUIDs = AssetDatabase.FindAssets("t:MonoScript");
        
            // FormerlySerializedAs 패턴 정의 (네임스페이스 및 어트리뷰트)
            string pattern = @"\[FormerlySerializedAs\(\"".*?\""\)\]\s*";
            string namespacePattern = @"using UnityEngine.Serialization;\s*\r?\n";

            int cleanedCount = 0;

            foreach (var guid in scriptGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.StartsWith("Assets/")) continue; // 패키지나 외부 폴더 제외

                string content = File.ReadAllText(path);
            
                if (Regex.IsMatch(content, pattern))
                {
                    // 어트리뷰트 제거 및 사용하지 않는 네임스페이스 제거
                    content = Regex.Replace(content, pattern, string.Empty);
                    content = Regex.Replace(content, namespacePattern, string.Empty);
                
                    File.WriteAllText(path, content);
                    cleanedCount++;
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"정리 완료! 총 {cleanedCount}개의 스크립트에서 [FormerlySerializedAs]를 안전하게 제거했습니다.");
        }
    }
}
