using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RV
{
	[CustomEditor(typeof(ReferenceSetting))]
	internal sealed class ReferenceSettingInspector : Editor
	{
		private SerializedProperty enabled = default;
		private SerializedProperty pauseInPlaymode = default;
		private SerializedProperty traceSceneObject = default;
		private SerializedProperty useEditorUtilityWhenSearchDependencies = default;
		private SerializedProperty localization = default;

		private void OnEnable()
		{
			enabled = serializedObject.FindProperty(nameof(enabled));
			pauseInPlaymode = serializedObject.FindProperty(nameof(pauseInPlaymode));
			traceSceneObject = serializedObject.FindProperty(nameof(traceSceneObject));
			useEditorUtilityWhenSearchDependencies = serializedObject.FindProperty(nameof(useEditorUtilityWhenSearchDependencies));
			localization = serializedObject.FindProperty(nameof(localization));
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			EditorGUI.BeginChangeCheck();
			
			EditorGUILayout.PropertyField(enabled, new GUIContent(Localize.Inst.setting_enabled));
			EditorGUILayout.Space(12);
			
			EditorGUI.BeginDisabledGroup(!enabled.boolValue);
			{
				// EditorGUI.indentLevel++;
				{
					EditorGUILayout.PropertyField(pauseInPlaymode, new GUIContent(Localize.Inst.setting_pauseInPlaymode));
					EditorGUILayout.PropertyField(traceSceneObject, new GUIContent(Localize.Inst.setting_traceSceneObjects));
					EditorGUILayout.PropertyField(useEditorUtilityWhenSearchDependencies, new GUIContent(Localize.Inst.setting_useEditorUtilityWhenSearchDependencies));
					
					var root = Path.GetFullPath($"Packages/kr.seonghwan.reference/Languages");
					var currentLocale = localization.stringValue;

					var localNames = new List<string>();
					var languageFiles = Directory.GetFiles(root, "*.json");
					foreach (string file in languageFiles)
					{
						var fi = new FileInfo(file);
						localNames.Add(fi.Name.Replace(".json",""));
					}

					int selected = localNames.IndexOf(currentLocale);
					int changed = EditorGUILayout.Popup("Localization", selected, localNames.ToArray());

					if (selected != changed)
					{
						ReferenceSetting.Localization = localNames[changed];
						Localize.Inst = ReferenceSetting.LoadLocalization;
					}
				}
				// EditorGUI.indentLevel--;
			}
			EditorGUI.EndDisabledGroup();

			if (EditorGUI.EndChangeCheck())
			{
				ReferenceWindow.isDirty = true;
			}
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}