using UnityEngine;

public class PauseSFX : MonoBehaviour
{
    [SerializeField] AudioClip pauseSFX;
    [SerializeField] AudioSource pauseSFXSource;

    public void PlayOneShotPauseSFX()
    {
        pauseSFXSource.PlayOneShot(pauseSFX);
    }
}
