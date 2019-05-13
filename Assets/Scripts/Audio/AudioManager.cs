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
            PlayMusic(gameMusic);
        }

        public void PlayVictoryMusic() {
            PlayMusic(victoryMusic);
        }

        private void PlayMusic(PlayableMusic _playableMusic) {
            if (currentMusic != _playableMusic) {
                MusicAudioSource.Stop();

                currentMusic = _playableMusic;

                MusicAudioSource.clip = currentMusic.LoopMusic;

                if (currentMusic.IntroMusic != null) {
                    MusicAudioSource.PlayOneShot(currentMusic.IntroMusic);
                    MusicAudioSource.PlayScheduled(AudioSettings.dspTime + currentMusic.IntroMusic.length);
                }
                else if (currentMusic.LoopMusic != null) {
                    MusicAudioSource.Play();
                }
            }

        }

        public void PlaySound(AudioClip _audioClip) {
            // TODO Faire un système de pistes de SFX !!!
            AudioSource.PlayClipAtPoint(_audioClip, Camera.main.transform.position);

            // SoundEffectPlayer newSoundEffect = Instantiate(soundEffectPlayerPrefab, transform, false);
            // newSoundEffect.PlayAudioClip(_audioClip);
        }
    }
}
