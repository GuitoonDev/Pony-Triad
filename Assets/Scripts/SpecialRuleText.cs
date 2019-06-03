using UnityEngine;
using UnityEngine.Events;

public class SpecialRuleText : MonoBehaviour
{
    public UnityAction OnAnimationFinished;

    private void Start() {
        Destroy(gameObject, 0.85f);
    }

    private void OnDestroy() {
        if (OnAnimationFinished != null) {
            OnAnimationFinished();
        }
    }
}
