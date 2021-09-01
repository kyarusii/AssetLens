using System.IO;
using UnityEditor;

namespace RV
{
    internal static class Callbacks
    {
        [InitializeOnLoadMethod]
        private static async void InitializeOnLoadMethod()
        {
            DirectoryInfo rootDirectory = new DirectoryInfo(FileSystem.CacheDirectory);
            if (!rootDirectory.Exists)
            {
                rootDirectory.Create();

                const string title = "Reference";
                const string content = "인덱싱 데이터가 없습니다. 생성할까요?";
                
                if (EditorUtility.DisplayDialog(title, content, "생성", "스킵"))
                {
                    await ReferenceCache.IndexAssets();
                    Config.IsEnabled = true;
                }
                else
                {
                    Config.IsEnabled = false;
                }
            }
        }
    }
}
