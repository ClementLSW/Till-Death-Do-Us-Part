using UnityEngine;

public class UIAudio : MonoBehaviour
{
    [SerializeField] AudioSource uiAudioSource;
    [SerializeField] AudioSource typingAudioSource;
    [SerializeField] AudioClip nextDialog;

    public void MoveToNextDialog()
    {
        uiAudioSource.PlayOneShot(nextDialog);
    }

    public void PlayTypingSound(bool isTyping)
    {
        switch (isTyping)
        {
            case true:
                typingAudioSource.Play();
                break;
            case false:
                typingAudioSource.Stop();
                break;
        }
    }
}
