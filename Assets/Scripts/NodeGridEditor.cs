using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class NodeGridEditor : EditorWindow
{
    private static NodeGridEditor _window;
    private static NodeGrid _nodeGrid;
    private static SerializedObject _gridSerializedObject;
    private static SerializedProperty _rowCountProperty;
    private static SerializedProperty _columnCountProperty;
    private static SerializedProperty _unitDistanceProperty;
    private static SerializedProperty _xOffsetProperty;
    private static SerializedProperty _zOffsetProperty;
    private static SerializedProperty _yOffsetProperty;

    private static SerializedProperty gizmoRadiusProperty;

    static NodeGridEditor()
    {
        EditorApplication.update += CheckNodeGrid;
    }

    private static void CheckNodeGrid()
    {
        if (_nodeGrid == null)
        {
            _nodeGrid = FindObjectOfType<NodeGrid>();

            if (_nodeGrid != null)
            {
                _gridSerializedObject = new SerializedObject(_nodeGrid);
                _rowCountProperty = _gridSerializedObject.FindProperty("_rowCount");
                _columnCountProperty = _gridSerializedObject.FindProperty("_columnCount");
                _unitDistanceProperty = _gridSerializedObject.FindProperty("_unitDistance");
                _xOffsetProperty = _gridSerializedObject.FindProperty("_xOffset");
                _zOffsetProperty = _gridSerializedObject.FindProperty("_zOffset");
                _yOffsetProperty = _gridSerializedObject.FindProperty("_yOffset");

                gizmoRadiusProperty = _gridSerializedObject.FindProperty("_gizmoRadius");
            }
        }
    }

    [MenuItem("Window/Node Grid Editor")]
    public static void OpenWindow()
    {
        _window = GetWindow<NodeGridEditor>();
        _window.titleContent = new GUIContent("Node Grid Editor");
    }

    private void OnGUI()
    {
        if (_nodeGrid == null)
        {
            EditorGUILayout.HelpBox("No NodeGrid found in the scene.", MessageType.Error);
            return;
        }

        _gridSerializedObject.Update();

        EditorGUILayout.Space(5);
        DrawHeading("Grid Editor");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Node properties");

        EditorGUILayout.PropertyField(_rowCountProperty);
        EditorGUILayout.PropertyField(_columnCountProperty);
        EditorGUILayout.PropertyField(_unitDistanceProperty);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Container Offset");
        EditorGUILayout.PropertyField(_xOffsetProperty);
        EditorGUILayout.PropertyField(_zOffsetProperty);
        EditorGUILayout.PropertyField(_yOffsetProperty);
        

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Node gizmos");
        EditorGUILayout.PropertyField(gizmoRadiusProperty);
        _gridSerializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(25);
        if (GUILayout.Button("Generate Grid") && !_nodeGrid.IsGridAlreadyGenerated())
        {
            _nodeGrid.GenerateGrid();
        }
    }

    private void DrawHeading(string text)
    {
        GUIStyle headingStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 16,
            //padding = new RectOffset(0, 0, 10, 10)
        };

        EditorGUILayout.LabelField(text, headingStyle);
        EditorGUILayout.Space();
    }
}