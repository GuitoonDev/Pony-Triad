using UnityEngine;
using UnityEngine.Events;

public class SpecialRuleText : MonoBehaviour
{
    public UnityAction OnAnimationFinished;

    private void Start() {
        Destroy(gameObject, 2);
    }

    private void OnDestroy() {
        if (OnAnimationFinished != null) {
            OnAnimationFinished();
        }
    }
}
