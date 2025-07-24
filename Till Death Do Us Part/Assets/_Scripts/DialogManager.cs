using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using static CharacterManager;

public class DialogManager : MonoBehaviour
{
    Deserializer deserializer;
    CharacterManager characterManager;
    AudioManager audioManager;
    GameManager gameManager;
    VFXManager vfxManager;

    #region Data Struct and Init
    // DialogLine represents a single line of dialog
    public struct DialogLine
    {
        public int ID;
        public string Text;
        public List<DialogOptions> Options;
        public List<Characters> CharactersInvolved;
        public int ScoreDelta;
        public AudioData AudioData;
        public string BG;
        public int GOTO;
        public string VFX;
    }

    // DialogOptions represents a choice in the dialog
    public struct DialogOptions
    {
        public string OptionText;
        public int NextDialogID;
    }

    // Dialog is a collection of dialog lines
    public struct Dialog
    {
        public List<DialogLine> Lines;
    }

    public struct AudioData
    {
        public string SFX;
        public string BGM;
        public string DialogueVO;
    }

    // MasterBank is a collection of all dialog lines
    public Dialog MasterBank;
    private void Awake()
    {
        MasterBank = new();

        MasterBank.Lines = new List<DialogLine>();
        characterManager = GetComponent<CharacterManager>();
        audioManager = GetComponent<AudioManager>();
        gameManager = GetComponentInParent<GameManager>();
        vfxManager = transform.parent.GetComponentInChildren<VFXManager>();
        deserializer = GetComponent<Deserializer>();
        deserializer.ReadTSV(this);

        CurrentDialogID = 1;
    }

    private void Start()
    {
        NextDialog(); 
    }

    public void SanityCheck()
    {
        if (MasterBank.Lines == null)
        {
            Debug.LogWarning("MasterBank is not initialized or contains no dialog lines.");
            MasterBank.Lines = new List<DialogLine>();
            return;
        }
    }
    #endregion

    [Header("Text Panel")]
    [SerializeField] public TMP_Text CharacterNameField;
    [SerializeField] public TMP_Text DialogTextField;

    [Header("Character Sprites")]
    [SerializeField] public GameObject CharacterPanelL;
    [SerializeField] public Image CharacterLPoseSprite;
    [SerializeField] public Image CharacterLEmotionSprite;

    [SerializeField] public GameObject CharacterPanelR;
    [SerializeField] public Image CharacterRPoseSprite;
    [SerializeField] public Image CharacterREmotionSprite;

    [Header("Background")]
    [SerializeField] public Image bg; // Background GameObject to be populated with sprites

    [Header("Options Panel")]
    [SerializeField] public GameObject OptionsPanel;
    [SerializeField] public Button BtnL;
    [SerializeField] public Button BtnR;

    [Header("Typing Settings")]
    [SerializeField] float typingDelay = 0.05f;
    bool skipTyping = false;
    bool isTyping = false;

    [Header("Debug - Do not alter")]
    [SerializeField] public int CurrentDialogID { get; private set; }

    public void SetDialogue(int id)
    {
        CurrentDialogID = id;
        NextDialog();
    }

    public void NextDialog()
    {
        if(CurrentDialogID == 9999)
        {
            gameManager.EvaluateEnding();
        }

        // If Currently text is being populated, skip animation and return
        if (isTyping)
        {
            SkipTypingAnimation();
            return; // Prevent proceeding if typing is still in progress
        }

        // Find the current dialog line based on CurrentDialogID
        DialogLine currentLine = MasterBank.Lines.Find(line => line.ID == CurrentDialogID);

        switch (currentLine.VFX)
        {
            case "flashred":
                vfxManager.TriggerFlashRed();
                break;
            default:
                break;
        }

        // Populate Background
        if (!string.IsNullOrEmpty(currentLine.BG))
        {
            if (bg != null)
            {
                Sprite loadedSprite = Resources.Load<Sprite>($"Sprites/BG/{currentLine.BG}");
                if (loadedSprite != null)
                {
                    bg.sprite = loadedSprite;
                }
                else
                {
                    Debug.LogWarning($"Background sprite '{currentLine.BG}' could not be found in the Resources folder.");
                }
            }
            else
            {
                Debug.LogWarning("Background GameObject not found in the scene.");
            }
        }

        // Populate the character information
        foreach (Characters c in currentLine.CharactersInvolved)
        {
            characterManager.PopulateCharacter(c);
        }

        // Handle Audio
        if (!string.IsNullOrEmpty(currentLine.AudioData.SFX)) audioManager.PlaySFXOneShot(currentLine.AudioData.SFX);
        if (!string.IsNullOrEmpty(currentLine.AudioData.BGM)) audioManager.PlayBGM(currentLine.AudioData.BGM);
        if (!string.IsNullOrEmpty(currentLine.AudioData.DialogueVO)) audioManager.PlayDialogue(currentLine.AudioData.DialogueVO);

        // Display the dialog text with typing effect
        StopAllCoroutines();
        StartCoroutine(TypeText(currentLine.Text));
        gameManager.AddScore(currentLine.ScoreDelta);

        if(currentLine.Options == null)
        {
            Debug.LogWarning($"No options found for dialog ID {CurrentDialogID}. Proceeding to next dialog line.");
            // If there are no options, Register next line in the dialog based on GOTO value
            if(currentLine.GOTO > 0)CurrentDialogID = currentLine.GOTO;
            else Debug.LogWarning($"GOTO value is not set for dialog ID {CurrentDialogID}. No next dialog line will be registered.");
        }
        else
        {
            Debug.Log($"Displaying options for dialog ID {CurrentDialogID}.");
            Debug.Log($"Option 1: {currentLine.Options[0].OptionText}, Option 2: {currentLine.Options[1].OptionText}");
            DisplayOptions(currentLine.Options);
        }

    }

    public void DisplayOptions(List<DialogOptions> options)
    {
        OptionsPanel.SetActive(true);

        BtnL.onClick.RemoveAllListeners();
        BtnR.onClick.RemoveAllListeners();

        Debug.Log(options[0].OptionText);
        Debug.Log(options[1].OptionText);

        BtnL.GetComponentInChildren<TMP_Text>().text = options[0].OptionText;
        BtnR.GetComponentInChildren<TMP_Text>().text = options[1].OptionText;

        BtnL.onClick.AddListener(() => SelectOption(options[0].NextDialogID));
        BtnR.onClick.AddListener(() => SelectOption(options[1].NextDialogID));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true; // Mark typing as in progress

        DialogTextField.text = "";
        foreach (char c in text)
        {
            if (skipTyping)
            {
                DialogTextField.text = text;
                skipTyping = false;
                isTyping = false;
                yield break;
            }
            DialogTextField.text += c;
            yield return new WaitForSeconds(typingDelay);
        }

        skipTyping = false; // Reset skipTyping after finishing the text
        isTyping = false; // Mark typing as finished
    }

    /// <summary>
    /// Skips the typing animation for the current dialog line.
    /// </summary>
    public void SkipTypingAnimation()
    {
        skipTyping = true;
    }

    // #VibeCoded
    public void SelectOption(int nextDialogID)
    {
        CurrentDialogID = nextDialogID;
        OptionsPanel.SetActive(false);
        NextDialog();
    }
}
