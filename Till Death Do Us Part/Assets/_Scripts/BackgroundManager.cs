using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] public Image bg;
    public void PopulateBackGround(string bgName)
    {
        if (!string.IsNullOrEmpty(bgName))
        {
            if (bg != null)
            {
                Sprite loadedSprite = Resources.Load<Sprite>($"Sprites/BG/{bgName}");
                if (loadedSprite != null)
                {
                    bg.sprite = loadedSprite;
                }
            }
        }
    }
}
