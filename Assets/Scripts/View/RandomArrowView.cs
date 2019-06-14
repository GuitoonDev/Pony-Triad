using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class RandomArrowView : MonoBehaviour
{
    public UnityAction OnAnimationComplete;

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

    public void StartAnimation(PlayerNumber _playerId) {
        gameObject.SetActive(true);
        Animator.SetTrigger(string.Format("SelectPlayer_{0}", (int) _playerId));

        AudioSource.Play();
    }

    #region Animation Events Methods
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
