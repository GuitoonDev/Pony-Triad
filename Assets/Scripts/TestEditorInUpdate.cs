using UnityEngine;

[ExecuteInEditMode]
public class TestEditorInUpdate : MonoBehaviour
{
    public int testValue = 0;

    private void Update() {
        testValue++;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
#endif
    }
}
