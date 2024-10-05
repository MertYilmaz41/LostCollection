using UnityEngine;
using UnityEditor;

public class MirrorTool : EditorWindow
{
    [MenuItem("Tools/Mirror Tool")]
    public static void ShowWindow()
    {
        GetWindow<MirrorTool>("Mirror Tool");
    }

    private bool mirrorX = false;
    private bool mirrorY = false;
    private bool mirrorZ = false;

    void OnGUI()
    {
        GUILayout.Label("Select axes to mirror:", EditorStyles.boldLabel);

        mirrorX = GUILayout.Toggle(mirrorX, "Mirror X Axis");
        mirrorY = GUILayout.Toggle(mirrorY, "Mirror Y Axis");
        mirrorZ = GUILayout.Toggle(mirrorZ, "Mirror Z Axis");

        if (GUILayout.Button("Mirror Selected Objects"))
        {
            MirrorSelectedObjects();
        }
    }

    private void MirrorSelectedObjects()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Mirror Object");

            Vector3 localScale = obj.transform.localScale;
            if (mirrorX)
                localScale.x = -localScale.x;
            if (mirrorY)
                localScale.y = -localScale.y;
            if (mirrorZ)
                localScale.z = -localScale.z;

            obj.transform.localScale = localScale;
        }
    }
}
