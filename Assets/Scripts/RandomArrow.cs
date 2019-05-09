using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomArrow : MonoBehaviour
{
    public Action OnAnimationComplete;

    private Animator animator;
    public Animator Animator {
        get {
            if (animator == null) {
                animator = GetComponent<Animator>();
            }

            return animator;
        }
    }

    public void StartAnimation(int _playerId) {
        gameObject.SetActive(true);
        Animator.SetTrigger(string.Format("SelectPlayer_{0}", _playerId));
    }

    #region Animation Event methods
    public void AnimationComplete() {
        if (OnAnimationComplete != null) {
            OnAnimationComplete();
        }

        gameObject.SetActive(false);
    }
    #endregion
}
