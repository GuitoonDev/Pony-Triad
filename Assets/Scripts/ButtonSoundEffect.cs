using Audio;
using UnityEngine;

public class ButtonSoundEffect : MonoBehaviour
{
    [SerializeField] private AudioClip source;

    public void PlayButtonSound() {
        AudioManager.Instance.PlaySound(source);
    }
}
