using System.Collections;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectPlayer : MonoBehaviour
    {
        private AudioSource audioSource = null;
        private AudioSource AudioSource {
            get {
                if (audioSource == null) {
                    audioSource = GetComponent<AudioSource>();
                }
                return audioSource;
            }
        }

        public void PlayAudioClip(AudioClip _audioClip) {
            StartCoroutine(PlayOneShotCoroutine(_audioClip));
        }

        private IEnumerator PlayOneShotCoroutine(AudioClip _audioClip) {
            AudioSource.PlayOneShot(_audioClip);
            yield return new WaitForSeconds(_audioClip.length);
            Destroy(gameObject);
        }
    }
}
