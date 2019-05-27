using Audio;
using UnityEngine;

public class ButtonSoundEffect : MonoBehaviour
{
    [SerializeField] private AudioClip source = null;

    public void PlayButtonSound() {
        AudioManager.Instance.PlaySound(source);
    }
}
