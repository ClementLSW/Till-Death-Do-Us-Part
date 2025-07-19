using UnityEngine;

//TODO: Change Directory to Sprites/Character Sprites

public class CharacterManager : MonoBehaviour
{
    DialogManager dialogManager;
    private void Awake()
    {
        dialogManager = GetComponent<DialogManager>();
    }

    Vector3 activeScaleL = new Vector3(6f, 6f, 1f);
    Vector3 inactiveScaleL = new Vector3(5f, 5f, 1f);

    Vector3 activeScaleR = new Vector3(-1.2f, 1.2f, 1f);
    Vector3 inactiveScaleR = new Vector3(-1f, 1f, 1f);

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

                dialogManager.CharacterPanelL.transform.localScale = activeScaleL;

                //dialogManager.CharacterLPoseSprite.rectTransform.localScale = activeScaleL ;
                //dialogManager.CharacterLEmotionSprite.rectTransform.localScale = activeScaleL;
            }
            else
            {
                dialogManager.CharacterPanelL.transform.localScale = inactiveScaleL;

                //dialogManager.CharacterLPoseSprite.rectTransform.localScale = inactiveScaleL;
                //dialogManager.CharacterLEmotionSprite.rectTransform.localScale = inactiveScaleL;
            }

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
            //TODO: Consider scaling parent object instead of inividual sprites
            if (c.isActive)
            {
                dialogManager.CharacterNameField.text = c.Name;

                dialogManager.CharacterPanelR.transform.localScale = activeScaleL;

                //dialogManager.CharacterRPoseSprite.rectTransform.localScale = activeScaleL;
                //dialogManager.CharacterREmotionSprite.rectTransform.localScale = activeScaleL;
                //dialogManager.CharacterRPoseSprite.rectTransform.localScale = activeScaleR;
                //dialogManager.CharacterREmotionSprite.rectTransform.localScale = activeScaleR;
            }
            else
            {
                dialogManager.CharacterPanelR.transform.localScale = inactiveScaleL;

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
