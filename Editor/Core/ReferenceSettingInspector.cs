using UnityEditor;

namespace RV
{
	[CustomEditor(typeof(ReferenceSetting))]
	internal sealed class ReferenceSettingInspector : Editor
	{
		private SerializedProperty enabled = default;

		private void OnEnable()
		{
			enabled = serializedObject.FindProperty(nameof(enabled));
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(enabled);
			serializedObject.ApplyModifiedProperties();
		}
	}
}