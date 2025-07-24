using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashRed : MonoBehaviour
{
    [SerializeField] public Image VFX;

    /// <summary>
    /// Flash red overlay
    /// </summary>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    public void Trigger(Color color, float duration)
    {
        StartCoroutine(Flash(color, duration));
    }

    IEnumerator Flash(Color baseColor, float duration)
    {
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalizedTime = t / duration;

            float alpha = 0.35f * (1 - Mathf.Cos(2 * Mathf.PI * normalizedTime));

            baseColor.a = alpha;
            VFX.color = baseColor;

            yield return null;
        }
    }
}
