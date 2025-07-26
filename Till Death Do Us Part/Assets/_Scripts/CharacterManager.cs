using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    DialogManager dialogManager;
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

    public struct Characters
    {
        public enum Position
        {
            Left,
            Right
        }
        //public int CharID;
        public string Name;
        public string Pose;
        public string Emotion;
        public bool isActive;
        public Position position;
        //public Sprite Image;
        //public bool isFlipped;
    }

    public void PopulateCharacter(Characters c)
    {
        if (dialogManager == null)
        {
            Debug.LogError("DialogManager is not assigned in CharacterManager.");
            dialogManager = GetComponent<DialogManager>();
            return;
        }

        if (c.position == Characters.Position.Left)
        {
            // Only Set Character Name if it is active
            if (c.isActive)
            {
                dialogManager.CharacterNameField.text = c.Name;

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

            Debug.Log($"Sprites/Character Sprites/{c.Name}/Pose/{c.Pose}");

            // Assign the left character's pose and emotion sprites
            dialogManager.CharacterLPoseSprite.sprite = Resources.Load<Sprite>(
                $"Sprites/Character Sprites/{c.Name}/Pose/{c.Pose}"
                );
            dialogManager.CharacterLEmotionSprite.sprite = Resources.Load<Sprite>(
                $"Sprites/Character Sprites/{c.Name}/Emotion/{c.Emotion}"
                );
        }
        else
        {
            //TODO: Change back to negative X scale once assets have be readjusted

            if (c.isActive)
            {
                dialogManager.CharacterNameField.text = c.Name;

                //dialogManager.CharacterPanelR.transform.localScale = activeScaleR;
                var charColor = dialogManager.CharacterLEmotionSprite.color;
                charColor.a = 1.0f;
                dialogManager.CharacterREmotionSprite.color = new(1f, 1f, 1f);
                dialogManager.CharacterRPoseSprite.color = new(1f, 1f, 1f);

                //dialogManager.CharacterRPoseSprite.rectTransform.localScale = activeScaleL;
                //dialogManager.CharacterREmotionSprite.rectTransform.localScale = activeScaleL;
                //dialogManager.CharacterRPoseSprite.rectTransform.localScale = activeScaleR;
                //dialogManager.CharacterREmotionSprite.rectTransform.localScale = activeScaleR;
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

            // Assign the right character's pose and emotion sprites
            dialogManager.CharacterRPoseSprite.sprite = Resources.Load<Sprite>(
                $"Sprites/Character Sprites/{c.Name}/Pose/{c.Pose}"
                );
            dialogManager.CharacterREmotionSprite.sprite = Resources.Load<Sprite>(
                $"Sprites/Character Sprites/{c.Name}/Emotion/{c.Emotion}"
                );

        }
    }
}
