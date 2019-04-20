using UnityEngine;

[ExecuteInEditMode]
public class TestEditorInUpdate : MonoBehaviour
{
    static TestEditorInUpdate() {
        UnityEditor.EditorApplication.update -= EditorUpdate;
    }

    private static void EditorUpdate() {
        Debug.Log("ok");
    }
}
