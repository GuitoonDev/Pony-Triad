using Audio;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSoundEffect : MonoBehaviour
{
    [SerializeField] private AudioClip source = null;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(PlayButtonSound);
    }

    private void PlayButtonSound() {
        AudioManager.Instance.PlaySound(source);
    }
}
