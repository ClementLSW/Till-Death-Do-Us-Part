using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles populating and updating character visuals and names in the dialog UI.
/// Requires DialogManager component to function.
/// </summary>
[RequireComponent(typeof(DialogManager))]
public class CharacterManager : MonoBehaviour
{
    DialogManager dialogManager;

    private void Awake()
    {
        dialogManager = GetComponent<DialogManager>();
    }

    /// <summary>
    /// Struct representing a character's data used to populate the UI.
    /// </summary>
    public struct CharacterData
    {
        public enum Position { Left, Right }

        public string Name;
        public string Pose;
        public string Emotion;
        public bool isActive;
        public Position position;
    }

    /// <summary>
    /// Populates the character portrait, pose, and emotion on screen based on the provided character data.
    /// </summary>
    /// <param name="c">Character data to populate.</param>
    public void PopulateCharacter(CharacterData c)
    {
        if (dialogManager == null)
        {
            Debug.LogError("DialogManager is not assigned in CharacterManager.");
            dialogManager = GetComponent<DialogManager>();
            return;
        }

        bool nameSet = false;

        #region LEFT SIDE CHARACTER
        if (c.position == CharacterData.Position.Left)
        {
            {
                SetCharacterVisuals(
                    c,
                    dialogManager.CharacterLPoseSprite,
                    dialogManager.CharacterLEmotionSprite,
                    dialogManager.CharacterPanelL.transform,
                    dialogManager.CharacterNameField,
                    ref nameSet
                );
            }
        }
        #endregion

        #region RIGHT SIDE CHARACTER
        else
        {
            SetCharacterVisuals(
                c,
                dialogManager.CharacterRPoseSprite,
                dialogManager.CharacterREmotionSprite,
                dialogManager.CharacterPanelR.transform,
                dialogManager.CharacterNameField,
                ref nameSet
            );
        }
        #endregion
    }

    #region Helper Methods
    private static void SetCharacterVisuals( CharacterData c, Image poseSprite, Image emotionSprite, TMP_Text namefield, ref bool nameSet)
    {
        if (string.IsNullOrEmpty(c.Name))
        {
            poseSprite.sprite = null;
            emotionSprite.sprite = null;
            return;
        }
        
        poseSprite.sprite = Resources.Load<Sprite>($"Sprites/Character Sprites/{c.Name}/Pose/{c.Pose}");
        emotionSprite.sprite = Resources.Load<Sprite>($"Sprites/Character Sprites/{c.Name}/Emotion/{c.Emotion}");

        if (c.isActive)
        {
            poseSprite.color = new Color(1f, 1f, 1f, 1f);
            emotionSprite.color = new Color(1f, 1f, 1f, 1f);
            namefield.text = c.Name;
            nameSet = true;
        }
        else
        {
            poseSprite.color = new Color(0.8f, 0.8f, 0.8f, 0.9f);
            emotionSprite.color = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        }
    }
    #endregion
}
