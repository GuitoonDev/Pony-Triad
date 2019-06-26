using System.Collections.Generic;
using UnityEngine;

namespace PonyTriad.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; set; }

        [Header("Tracks")]
        [SerializeField] private AudioSource sfxTrack = null;
        [SerializeField] private AudioSource musicTrack = null;

        [Header("Playable Musics")]
        [SerializeField] private PlayableMusic gameMusic = null;
        [SerializeField] private PlayableMusic victoryMusic = null;

        private PlayableMusic currentMusic = null;

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
                musicTrack.Stop();

                currentMusic = _playableMusic;

                musicTrack.clip = currentMusic.LoopMusic;

                if (currentMusic.IntroMusic != null) {
                    musicTrack.PlayOneShot(currentMusic.IntroMusic);
                    musicTrack.PlayScheduled(AudioSettings.dspTime + currentMusic.IntroMusic.length);
                }
                else if (currentMusic.LoopMusic != null) {
                    musicTrack.Play();
                }
            }
        }

        public void PlaySound(AudioClip _audioClip) {
            sfxTrack.PlayOneShot(_audioClip);
        }
    }
}
