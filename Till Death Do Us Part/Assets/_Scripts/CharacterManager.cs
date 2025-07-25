using UnityEngine;

/// <summary>
/// Handles populating and updating character visuals and names in the dialog UI.
/// Requires DialogManager component to function.
/// </summary>
public class CharacterManager : MonoBehaviour
{
    DialogManager dialogManager;

    // Scale values for active/inactive characters on the left side
    Vector3 activeScaleL = new Vector3(6f, 6f, 1f);
    Vector3 inactiveScaleL = new Vector3(5f, 5f, 1f);

    // Scale values for active/inactive characters on the right side
    Vector3 activeScaleR = new Vector3(-6f, 6f, 1f);
    Vector3 inactiveScaleR = new Vector3(-5f, 5f, 1f);

    private void Awake()
    {
        dialogManager = GetComponent<DialogManager>();
    }

    private void Start()
    {
        activeScaleL = dialogManager.CharacterPanelL.transform.localScale;
        activeScaleR = dialogManager.CharacterPanelR.transform.localScale;
    }


    Vector3 activeScaleL;

    Vector3 activeScaleR;

    /// <summary>
    /// Struct representing a character's data used to populate the UI.
    /// </summary>
    public struct Characters
    {
        public enum Position{ Left, Right }

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
    public void PopulateCharacter(Characters c)
    {
        bool nameSet = false;

        // Ensure dialogManager is assigned
        if (dialogManager == null)
        {
            Debug.LogError("DialogManager is not assigned in CharacterManager.");
            dialogManager = GetComponent<DialogManager>();
            return;
        }

        #region LEFT SIDE CHARACTER
        if (c.position == Characters.Position.Left)
        {
            if (!string.IsNullOrEmpty(c.Name))
            {
                // Only Set Character Name if it is active
                if (c.isActive)
                {
                    dialogManager.CharacterNameField.text = c.Name;
                    nameSet = true;

                //dialogManager.CharacterPanelL.transform.localScale = activeScaleL;
                var charColor = dialogManager.CharacterLEmotionSprite.color;
                charColor.a = 1.0f;
                dialogManager.CharacterLEmotionSprite.color = new(1f, 1f, 1f);
                dialogManager.CharacterLPoseSprite.color = new(1f, 1f, 1f);
            }
            else
            {
                var charColor = dialogManager.CharacterLEmotionSprite.color;
                charColor.a = 0.9f;
                dialogManager.CharacterLEmotionSprite.color = new(0.8f, 0.8f, 0.8f);
                dialogManager.CharacterLPoseSprite.color = new(0.8f, 0.8f, 0.8f);
                //dialogManager.CharacterPanelL.transform.localScale = inactiveScaleL;
            }
        }
        #endregion

        #region RIGHT SIDE CHARACTER
        else
        {
            if (!string.IsNullOrEmpty(c.Name))
            {
                if (c.isActive)
                {
                    dialogManager.CharacterNameField.text = c.Name;
                    nameSet = true;

                //dialogManager.CharacterPanelR.transform.localScale = activeScaleR;
                var charColor = dialogManager.CharacterLEmotionSprite.color;
                charColor.a = 1.0f;
                dialogManager.CharacterREmotionSprite.color = new(1f, 1f, 1f);
                dialogManager.CharacterRPoseSprite.color = new(1f, 1f, 1f);

                // Assign the right character's pose and emotion sprites
                dialogManager.CharacterRPoseSprite.sprite = Resources.Load<Sprite>(
                    $"Sprites/Character Sprites/{c.Name}/Pose/{c.Pose}"
                    );
                dialogManager.CharacterREmotionSprite.sprite = Resources.Load<Sprite>(
                    $"Sprites/Character Sprites/{c.Name}/Emotion/{c.Emotion}"
                    );
            }
            else
            {
                //dialogManager.CharacterPanelR.transform.localScale = inactiveScaleR;
                var charColor = dialogManager.CharacterLEmotionSprite.color;
                charColor.a = 0.9f;
                dialogManager.CharacterREmotionSprite.color = new(0.8f, 0.8f, 0.8f);
                dialogManager.CharacterRPoseSprite.color = new(0.8f, 0.8f, 0.8f);

                //dialogManager.CharacterRPoseSprite.rectTransform.localScale = inactiveScaleL;
                //dialogManager.CharacterREmotionSprite.rectTransform.localScale = inactiveScaleL;
                //dialogManager.CharacterRPoseSprite.rectTransform.localScale = inactiveScaleR;
                //dialogManager.CharacterREmotionSprite.rectTransform.localScale = inactiveScaleR;
            }

        }
        #endregion

        // If no name was set, clear the character name field
        if (!nameSet)
        {
            dialogManager.CharacterNameField.text = string.Empty;
        }
    }
}
