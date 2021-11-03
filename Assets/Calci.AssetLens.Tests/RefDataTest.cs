using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using AssetLens.Reference;

namespace AssetLens.Tests
{
    public class RefDataTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void ReferenceTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ReferenceTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }

        [Test]
        public void SampleAssetIndexedCheck()
        {
            AssetLensCache.CleanupAssets();
            AssetLensCache.IndexAssets();
            
            const string materialPath = "Assets/Content/Materials/MAT_Green.mat";
            const string prefabPath = "Assets/Content/Prefabs/Cube {0}.prefab";

            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

            Assert.NotNull(material);

            GameObject prefab1 = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format(prefabPath, 1));
            GameObject prefab2 = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format(prefabPath, 2));

            Assert.NotNull(prefab1);
            Assert.NotNull(prefab2);

            RefData materialRefData = RefData.Get(AssetDatabase.AssetPathToGUID(materialPath));

            Assert.True(materialRefData.referedByGuids.Contains(AssetDatabase.AssetPathToGUID(string.Format(prefabPath, 1))));
            Assert.True(materialRefData.referedByGuids.Contains(AssetDatabase.AssetPathToGUID(string.Format(prefabPath, 2))));
        }
    }
}