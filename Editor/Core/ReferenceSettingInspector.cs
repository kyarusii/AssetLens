using UnityEditor;

namespace RV
{
	[CustomEditor(typeof(ReferenceSetting))]
	internal sealed class ReferenceSettingInspector : Editor
	{
		private SerializedProperty enabled = default;
		private SerializedProperty pauseInPlaymode = default;
		private SerializedProperty traceSceneObject = default;
		private SerializedProperty useEditorUtilityWhenSearchDependencies = default;

		private void OnEnable()
		{
			enabled = serializedObject.FindProperty(nameof(enabled));
			pauseInPlaymode = serializedObject.FindProperty(nameof(pauseInPlaymode));
			traceSceneObject = serializedObject.FindProperty(nameof(traceSceneObject));
			useEditorUtilityWhenSearchDependencies = serializedObject.FindProperty(nameof(useEditorUtilityWhenSearchDependencies));
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			
			EditorGUILayout.PropertyField(enabled);
			
			EditorGUI.BeginDisabledGroup(!enabled.boolValue);
			{
				EditorGUI.indentLevel++;
				{
					EditorGUILayout.PropertyField(pauseInPlaymode);
					EditorGUILayout.PropertyField(traceSceneObject);
					EditorGUILayout.PropertyField(useEditorUtilityWhenSearchDependencies);
				}
				EditorGUI.indentLevel--;
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