using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] public Image bg;
    public void PopulateBackGround(string bgName)
    {
        if (!string.IsNullOrEmpty(bgName))
        {
            Debug.LogWarning("Background not specified!");
            if (bg != null)
            {
                Sprite loadedSprite = Resources.Load<Sprite>($"Sprites/Backgrounds/{bgName}");
                if (loadedSprite != null)
                {
                    bg.sprite = loadedSprite;
                }
                else
                {
                    Debug.LogError($"Background {bgName} not found!");
                }
            }
        }
    }

    public IEnumerator FadeBackground(float targetAlpha)
    {
        while (Mathf.Abs(bg.color.a - targetAlpha) > 0.01f)
        {
            float bgAlpha = bg.color.a;
            bgAlpha = Mathf.MoveTowards(bgAlpha, targetAlpha, 0.05f);
            bg.color = new(1f, 1f, 1f, bgAlpha);
            yield return null;
        }
    }
}
