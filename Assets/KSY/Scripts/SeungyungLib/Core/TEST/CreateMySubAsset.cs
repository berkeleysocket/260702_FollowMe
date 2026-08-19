using UnityEditor;
using UnityEngine;
namespace SeungyungLib.Core.Utility
{
    public class MyData : ScriptableObject
    {
    }

    public class SubAssetExample
    {
        [MenuItem("Tools/Create Sub-Asset")]
        public static void CreateMySubAsset()
        {
            // 1. 부모가 될 .asset 파일을 메모리에 만듭니다 (ScriptableObject)
            MyData parentAsset = ScriptableObject.CreateInstance<MyData>();
            parentAsset.name = "MyMainAsset";

            // 2. 부모 파일을 먼저 물리 디스크에 생성(저장)합니다.
            AssetDatabase.CreateAsset(parentAsset, "Assets/KSY/Scripts/SeungyungLib/Core/Utility/TEST/MySubAsset.asset");

            // 3. 자식으로 집어넣을 오브젝트(예: 머티리얼)를 생성합니다.
            Material childMaterial = new Material(Shader.Find("Standard"));
            childMaterial.name = "MyChildMaterial"; // 자식 오브젝트의 이름 설정

            // ⭐ 4. [핵심] 자식 오브젝트를 부모 에셋에 '집어넣습니다'
            AssetDatabase.AddObjectToAsset(childMaterial, parentAsset);

            // 5. 디스크에 변경사항을 물리적으로 기록(저장)합니다.
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        
            Debug.Log("자식 오브젝트를 부모 .asset에 안전하게 집어넣었습니다!");
        }
    }
}
