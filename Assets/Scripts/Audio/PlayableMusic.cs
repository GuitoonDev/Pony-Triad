using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class PlayableMusic
    {
        [SerializeField] private float delayAtIntroEnd = 0f;
        public float DelayAtIntroEnd => delayAtIntroEnd;

        [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;
        public float Volume => volume;

        [SerializeField] private AudioClip introMusic = null;
        public AudioClip IntroMusic => introMusic;

        [SerializeField] private AudioClip loopMusic = null;
        public AudioClip LoopMusic => loopMusic;
    }
}