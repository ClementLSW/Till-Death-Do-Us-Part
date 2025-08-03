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

    bool nameSet = false;

    public void ResetNameState()
    {
        nameSet = false;
    }

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
            //Debug.LogError("DialogManager is not assigned in CharacterManager.");
            dialogManager = GetComponent<DialogManager>();
            return;
        }

        #region LEFT SIDE CHARACTER
        if (c.position == CharacterData.Position.Left)
        {
            {
                SetCharacterVisuals(
                    c,
                    dialogManager.CharacterLPoseSprite,
                    dialogManager.CharacterLEmotionSprite,
                    dialogManager.CharacterNameField,
                    ref nameSet
                );
            }
            Debug.Log("Set Left Name");
        }
        #endregion

        #region RIGHT SIDE CHARACTER
        else
        {
            SetCharacterVisuals(
                c,
                dialogManager.CharacterRPoseSprite,
                dialogManager.CharacterREmotionSprite,
                dialogManager.CharacterNameField,
                ref nameSet
            );
            //Debug.Log("Set Right Name");
        }
        #endregion

        if(!nameSet && dialogManager.CharacterNameField != null)
        {
            //Debug.LogWarning("No Name Set!");
            dialogManager.CharacterNameField.text = string.Empty; // Set name to ??? if no character is active
        }
    }

    #region Helper Methods
    private static void SetCharacterVisuals( CharacterData c, Image poseSprite, Image emotionSprite, TMP_Text namefield, ref bool nameSet)
    {
        if (string.IsNullOrEmpty(c.Name))
        {
            poseSprite.color = new Color(0f, 0f, 0f, 0f); // Make invisible
            poseSprite.sprite = null;
            emotionSprite.color = new Color(0f, 0f, 0f, 0f); // Make invisible
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
            poseSprite.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            emotionSprite.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        }

        if(poseSprite.sprite == null)
        {
            Debug.LogWarning($"Character visuals not found for {c.Name}. Pose: {c.Pose}");
        }

        if(emotionSprite.sprite == null)
        {
            Debug.LogWarning($"Character visuals not found for {c.Name}. Emotion: {c.Emotion}");
        }
    }

    public void SetUnknownName()
    {
        if (dialogManager.CharacterNameField != null)
        {
            dialogManager.CharacterNameField.text = "???";
        }
    }

    public void ClearCharacters()
    {
        if (dialogManager == null)
        {
            Debug.LogError("DialogManager is not assigned in CharacterManager.");
            dialogManager = GetComponent<DialogManager>();
        }

        // Clear all character visuals
        dialogManager.CharacterLPoseSprite.sprite = null;
        dialogManager.CharacterLEmotionSprite.sprite = null;
        dialogManager.CharacterRPoseSprite.sprite = null;
        dialogManager.CharacterREmotionSprite.sprite = null;

        dialogManager.CharacterLPoseSprite.color = new Color(0f, 0f, 0f, 0f); // Make invisible
        dialogManager.CharacterLEmotionSprite.color = new Color(0f, 0f, 0f, 0f); // Make invisible
        dialogManager.CharacterRPoseSprite.color = new Color(0f, 0f, 0f, 0f); // Make invisible
        dialogManager.CharacterREmotionSprite.color = new Color(0f, 0f, 0f, 0f); // Make invisible

        // Clear character names
        if (dialogManager.CharacterNameField != null)
        {
            dialogManager.CharacterNameField.text = string.Empty;
        }
    }
    #endregion
}
