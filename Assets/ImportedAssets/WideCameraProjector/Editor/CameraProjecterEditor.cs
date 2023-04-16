using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WideCameraProjector)), CanEditMultipleObjects] 
public class WideCameraProjectorEditor : Editor {

	static public class Styles
    {
        static public string[] _excludes = new string[] {
            "m_Script", "projection", "resolution"
        };

        static public GUIContent[] _resolutionGUIs = new GUIContent[] {
            new GUIContent("4"),
            new GUIContent("8"),
            new GUIContent("16"),
            new GUIContent("32"),
            new GUIContent("64"),
            new GUIContent("128"),
            new GUIContent("256"),
            new GUIContent("512"),
            new GUIContent("1024"),
            new GUIContent("2048"),
            new GUIContent("4096"),
        };

        static public int[] _resolutionValues = new int[] {
            4,
            8,
            16,
            32,
            64,
            128,
            256,
            512,
            1024,
            2048,
            4096,
        };

        static public GUIContent _Resolution = new GUIContent("Resolution");
    }

    static public void DrawResolutionPopup (SerializedProperty prop)
    {
        EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
        EditorGUI.BeginChangeCheck();
        var res = EditorGUILayout.IntPopup(Styles._Resolution, prop.intValue, Styles._resolutionGUIs, Styles._resolutionValues);
        if (EditorGUI.EndChangeCheck())
            prop.intValue = res;
        EditorGUI.showMixedValue = false;
    }

    SerializedProperty projection;
    SerializedProperty resolution;

    void OnEnable ()
    {
        projection = serializedObject.FindProperty("projection");
        resolution = serializedObject.FindProperty("resolution");
    }

    public override void OnInspectorGUI()
    {
        var t = ((WideCameraProjector)target);

        serializedObject.Update();
        EditorGUILayout.PropertyField(projection);
        DrawResolutionPopup(resolution);
        DrawPropertiesExcluding(serializedObject, Styles._excludes);
        serializedObject.ApplyModifiedProperties();
    }

}
