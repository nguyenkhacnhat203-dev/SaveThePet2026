
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PopupListLevel))]
public class PopupListLevelEditor : Editor
{
    private SerializedProperty prefab;
    private SerializedProperty levelGroup;
    private SerializedProperty parent;
    private SerializedProperty levels;

    private SerializedProperty blackOverlay;
    private SerializedProperty fadeDuration;

    private SerializedProperty btnLeft;
    private SerializedProperty btnRight;
    private SerializedProperty imgLeft;
    private SerializedProperty imgRight;
    private SerializedProperty levelsPerPage;
    private SerializedProperty disabledAlpha;

    private void OnEnable()
    {
        prefab = serializedObject.FindProperty("prefab");
        levelGroup = serializedObject.FindProperty("levelGroup");

        parent = levelGroup.FindPropertyRelative("Parent");
        levels = levelGroup.FindPropertyRelative("Levels");

        blackOverlay = serializedObject.FindProperty("blackOverlay");
        fadeDuration = serializedObject.FindProperty("fadeDuration");

        btnLeft = serializedObject.FindProperty("Btn_Left");
        btnRight = serializedObject.FindProperty("Btn_Right");
        imgLeft = serializedObject.FindProperty("Img_Left");
        imgRight = serializedObject.FindProperty("Img_Right");

        levelsPerPage = serializedObject.FindProperty("levelsPerPage");
        disabledAlpha = serializedObject.FindProperty("disabledAlpha");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(prefab);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Replay Transition", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(blackOverlay);
        EditorGUILayout.PropertyField(fadeDuration);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Pagination", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(btnLeft, new GUIContent("Button Left"));
        EditorGUILayout.PropertyField(btnRight, new GUIContent("Button Right"));

        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(imgLeft, new GUIContent("Image Left"));
        EditorGUILayout.PropertyField(imgRight, new GUIContent("Image Right"));

        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(levelsPerPage, new GUIContent("Levels Per Page"));
        EditorGUILayout.PropertyField(disabledAlpha, new GUIContent("Disabled Alpha"));

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Level Group", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(parent);

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Levels", EditorStyles.boldLabel);

        for (int i = 0; i < levels.arraySize; i++)
        {
            SerializedProperty element = levels.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"Level {i + 1}", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                element.FindPropertyRelative("LevelImage"),
                new GUIContent("Level Image")
            );

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                levels.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Add Level"))
        {
            levels.InsertArrayElementAtIndex(levels.arraySize);

            SerializedProperty newElement =
                levels.GetArrayElementAtIndex(levels.arraySize - 1);

            newElement.FindPropertyRelative("LevelImage").objectReferenceValue = null;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
