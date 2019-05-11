using System.Collections;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; set; }

        [Header("Playable Musics")]
        [SerializeField] private PlayableMusic gameMusic = null;
        [SerializeField] private PlayableMusic victoryMusic = null;

        [Header("Sound Effects Prefab")]
        [SerializeField] private SoundEffectPlayer soundEffectPlayerPrefab = null;

        private AudioSource musicAudioSource = null;
        private AudioSource MusicAudioSource {
            get {
                if (musicAudioSource == null) {
                    musicAudioSource = GetComponent<AudioSource>();
                }

                return musicAudioSource;
            }
        }

        private PlayableMusic currentMusic = null;

        private Coroutine waitForEndIntroMusicCoroutine = null;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else if (Instance != this) {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        public void PlayGameMusic() {
            if (currentMusic != gameMusic) {
                MusicAudioSource.Stop();

                currentMusic = gameMusic;

                MusicAudioSource.volume = currentMusic.Volume;

                if (currentMusic.IntroMusic != null) {
                    PlayerIntroMusic();
                }
                else if (currentMusic.LoopMusic != null) {
                    PlayerLoopMusic();
                }
            }
        }

        public void PlayVictoryMusic() {
            if (currentMusic != victoryMusic) {
                MusicAudioSource.Stop();

                currentMusic = victoryMusic;

                MusicAudioSource.volume = currentMusic.Volume;

                if (currentMusic.IntroMusic != null) {
                    PlayerIntroMusic();
                }
                else if (currentMusic.LoopMusic != null) {
                    PlayerLoopMusic();
                }
            }
        }

        public void PlaySound(AudioClip _audioClip) {
            SoundEffectPlayer newSoundEffect = Instantiate(soundEffectPlayerPrefab, transform, false);
            newSoundEffect.PlayAudioClip(_audioClip);
        }

        private void PlayerIntroMusic() {
            MusicAudioSource.PlayOneShot(currentMusic.IntroMusic);

            if (waitForEndIntroMusicCoroutine != null) {
                StopCoroutine(waitForEndIntroMusicCoroutine);
            }
            waitForEndIntroMusicCoroutine = StartCoroutine(WaitForEndIntroMusic());
        }

        private void PlayerLoopMusic() {
            MusicAudioSource.clip = currentMusic.LoopMusic;
            MusicAudioSource.loop = true;
            MusicAudioSource.Play();
        }

        private IEnumerator WaitForEndIntroMusic() {
            yield return new WaitForSeconds(currentMusic.IntroMusic.length + currentMusic.DelayAtIntroEnd);
            if (currentMusic.LoopMusic != null) {
                PlayerLoopMusic();
            }

            waitForEndIntroMusicCoroutine = null;
        }
    }
}
