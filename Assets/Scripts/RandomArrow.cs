using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class RandomArrow : MonoBehaviour
{
    public Action OnAnimationComplete;

    private Animator animator;
    private Animator Animator {
        get {
            if (animator == null) {
                animator = GetComponent<Animator>();
            }

            return animator;
        }
    }

    private AudioSource audioSource;
    private AudioSource AudioSource {
        get {
            if (audioSource == null) {
                audioSource = GetComponent<AudioSource>();
            }

            return audioSource;
        }
    }

    public void StartAnimation(int _playerId) {
        gameObject.SetActive(true);
        Animator.SetTrigger(string.Format("SelectPlayer_{0}", _playerId));

        AudioSource.Play();
    }

    #region Animation Event methods
    public void AnimationComplete() {
        if (OnAnimationComplete != null) {
            OnAnimationComplete();
        }

        gameObject.SetActive(false);
    }

    public void RollingComplete() {
        AudioSource.Stop();
    }
    #endregion
}
