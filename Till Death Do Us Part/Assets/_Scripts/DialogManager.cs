using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    BackgroundManager bgManager;

    [SerializeField]
    DayOfWeek dayManager;

    #region Data Struct and Init
    // DialogLine represents a single line of dialog
    public struct DialogLine
    {
        public DayOfWeek.Day Day;
        public string ID;
        public string Text;
        public List<DialogOptions> Options;
        public List<CharacterData> CharactersInvolved;
        public int ScoreDelta;
        public AudioData AudioData;
        public string BG;
        public string GOTO;
        public string VFX;
    }

    // DialogOptions represents a choice in the dialog
    public struct DialogOptions
    {
        public string OptionText;
        public string NextDialogID;
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
        bgManager = GetComponent<BackgroundManager>();

        deserializer = GetComponent<Deserializer>();
        deserializer.ReadTSV(this);

        CurrentDialogID = "Mon001";
    }

    private void Start()
    {
        StartCoroutine(ProceedWithDialog());
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

    #region Other Serialized Fields
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

    [Header("Options Panel")]
    [SerializeField] public GameObject OptionsPanel;
    [SerializeField] public Button BtnL;
    [SerializeField] public Button BtnR;

    [Header("Next Dialog Button")]
    [SerializeField] public Button nextDialogButton;
    [SerializeField] public Image nextDialogAvailableIndicator;

    [Header("Typing Settings")]
    [SerializeField] float typingDelay = 0.05f;

    #endregion

    bool skipTyping = false;
    bool isTyping = false;
    bool isWaitingForOptions = false; // Flag to check if waiting for options
    bool hasCrossfadeQueued = false;

    [Header("Debug - Do not alter")]
    [SerializeField] private string currentDialogID;
    public string CurrentDialogID
    {
        get => currentDialogID;
        private set => currentDialogID = value;
    }
    public DayOfWeek.Day PreviousDialogDay { get; private set; }

    private const string END_BAD = "Sun042";
    private const string END_OKAY = "Sun011";
    private const string END_GOOD = "Sun070";

    /// <summary>
    /// Util Function to be called by Save Load
    /// </summary>
    /// <param name="id"></param>
    public void SetDialogue(string id)
    {
        CurrentDialogID = id;
        StartCoroutine(ProceedWithDialog());
    }

    public void NextDialog()
    {
        Debug.Log($"NextDialog called with CurrentDialogID: {CurrentDialogID}");
        StartCoroutine(ProceedWithDialog());
    }

    public IEnumerator ProceedWithDialog()
    {
        if (isTyping)
        {
            Debug.Log("Skipping typing animation as it is currently in progress.");
            SkipTypingAnimation(); // Skip typing if it's currently in progress
            yield break; // Exit if typing is still in progress
        }

        DialogLine currentLine = MasterBank.Lines.Find(line => line.ID == CurrentDialogID);

        if (currentLine.Day != PreviousDialogDay)
        {
            nextDialogButton.interactable = false;
            gameManager.CurrentDay = currentLine.Day; // Update the current day in GameManager
            yield return StartCoroutine(dayManager.PlayDayTransition(currentLine.Day, () =>
            {
                if (currentLine.CharactersInvolved != null)
                {
                    foreach (CharacterData c in currentLine.CharactersInvolved)
                    {
                        characterManager.PopulateCharacter(c);
                    }
                }

            })); // Wait for the day transition to complete
            nextDialogButton.interactable = true;
        }

        PreviousDialogDay = currentLine.Day; // Store the previous dialog Day


        switch (currentLine.VFX)
        {
            case "flashred":
                bgManager.PopulateBackGround(currentLine.BG);
                vfxManager.TriggerFlashRed();
                break;
            case "changeLoc":
                hasCrossfadeQueued = true;
                bgManager.PopulateBackGround(currentLine.BG);
                break;
            default:
                if (hasCrossfadeQueued)
                {
                    nextDialogButton.interactable = false;
                    yield return bgManager.FadeBackground(0f);
                    bgManager.PopulateBackGround(currentLine.BG);
                    yield return bgManager.FadeBackground(1f);
                    nextDialogButton.interactable = true;
                    hasCrossfadeQueued = false;
                }
                else
                {
                    bgManager.PopulateBackGround(currentLine.BG);
                }
                break;
        }

        yield return StartCoroutine(PopulateDialog(currentLine));
    }

    private IEnumerator PopulateDialog(DialogLine currentLine)
    {
        if (isWaitingForOptions)
        {
            yield break; // If waiting for options, do not proceed with dialog
        }

        //Step 1: Populate Background
        bgManager.PopulateBackGround(currentLine.BG);

        // Step 2: Populate Characters
        if (currentLine.CharactersInvolved == null || currentLine.CharactersInvolved.Count == 0)
        {
            Debug.LogWarning($"No characters involved in dialog ID {CurrentDialogID}. Skipping character population.");
            characterManager.ClearCharacters(); // Clear characters if none are involved
            characterManager.SetUnknownName();
        }
        else
        {
            foreach (CharacterData c in currentLine.CharactersInvolved)
            {
                characterManager.PopulateCharacter(c);
            }
            characterManager.ResetNameState();
        }

        // Step 3: Handle Audio
        if (!string.IsNullOrEmpty(currentLine.AudioData.SFX)) audioManager.PlaySFXOneShot(currentLine.AudioData.SFX);
        if (!string.IsNullOrEmpty(currentLine.AudioData.BGM)) audioManager.PlayBGM(currentLine.AudioData.BGM);
        if (!string.IsNullOrEmpty(currentLine.AudioData.DialogueVO)) audioManager.PlayDialogue(currentLine.AudioData.DialogueVO);

        // Step 4: Handle score change
        gameManager.AddScore(currentLine.ScoreDelta);

        // Step 5: Display the dialog text with typing effect
        yield return StartCoroutine(TypeText(currentLine.Text));

        // Step 6: Handle Options
        if (currentLine.Options == null || currentLine.Options.Count == 0)
        {
            if (string.Equals(currentLine.GOTO, "END"))
            {
                int score = gameManager.EvaluateEnding();

                if (score < 3)
                {
                    CurrentDialogID = END_BAD; // Set to bad ending dialog
                }
                else if (score < 5)
                {
                    CurrentDialogID = END_OKAY; // Set to okay ending dialog
                }
                else
                {
                    CurrentDialogID = END_GOOD; // Set to good ending dialog
                }
            }
            else if (!string.IsNullOrEmpty(currentLine.GOTO))
            {
                CurrentDialogID = currentLine.GOTO;
            }
        }
        else
        {
            PreviousDialogDay = currentLine.Day; // Store the previous dialog day
            DisplayOptions(currentLine.Options);
        }
    }

    #region Typing Utils
    IEnumerator TypeText(string text)
    {
        nextDialogAvailableIndicator.enabled = false;
        isTyping = true; // Mark typing as in progress

        DialogTextField.text = "";
        foreach (char c in text)
        {
            if (skipTyping)
            {
                DialogTextField.text = text;
                skipTyping = false;
                isTyping = false;
                nextDialogAvailableIndicator.enabled = true;
                yield break;
            }
            DialogTextField.text += c;
            yield return new WaitForSeconds(typingDelay);
        }

        skipTyping = false; // Reset skipTyping after finishing the text
        isTyping = false; // Mark typing as finished
        nextDialogAvailableIndicator.enabled = true;
    }

    /// <summary>
    /// Skips the typing animation for the current dialog line.
    /// </summary>
    public void SkipTypingAnimation()
    {
        skipTyping = true;
    }
    #endregion

    #region Options Utils
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
        isWaitingForOptions = true; // Set the flag to indicate waiting for options
    }

    // #VibeCoded
    public void SelectOption(string nextDialogID)
    {
        Debug.Log($"Selected option leading to dialog ID: {nextDialogID}");
        isWaitingForOptions = false; // Reset the flag when an option is selected
        CurrentDialogID = nextDialogID;
        OptionsPanel.SetActive(false);
        NextDialog(); // Proceed to the next dialog line
    }

    #endregion

    #region LEGACY CODE
    //public void NextDialog()
    //{
    //    if(CurrentDialogID == "END")
    //    {
    //        gameManager.EvaluateEnding();
    //    }

    //    // If Currently text is being populated, skip animation and return
    //    if (isTyping)
    //    {
    //        SkipTypingAnimation();
    //        return; // Prevent proceeding if typing is still in progress
    //    }

    //    // Find the current dialog line based on CurrentDialogID
    //    DialogLine currentLine = MasterBank.Lines.Find(line => line.ID == CurrentDialogID);

    //    if (currentLine.Day != PreviousDialogDay)
    //    {
    //        gameManager.CurrentDay = currentLine.Day; // Update the current day in GameManager
    //    }

    //    switch (currentLine.VFX)
    //    {
    //        case "flashred":
    //            vfxManager.TriggerFlashRed();
    //            break;
    //        default:
    //            break;
    //    }

    //    // Populate Background
    //    if (!string.IsNullOrEmpty(currentLine.BG))
    //    {
    //        if (bg != null)
    //        {
    //            Sprite loadedSprite = Resources.Load<Sprite>($"Sprites/BG/{currentLine.BG}");
    //            if (loadedSprite != null)
    //            {
    //                bg.sprite = loadedSprite;
    //            }
    //            else
    //            {
    //                Debug.LogWarning($"Background sprite '{currentLine.BG}' could not be found in the Resources folder.");
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Background GameObject not found in the scene.");
    //        }
    //    }

    //    if (currentLine.CharactersInvolved == null)
    //    {
    //        Debug.LogWarning($"No characters involved in dialog ID {CurrentDialogID}. Skipping character population.");
    //    }
    //    {
    //        // Populate the character information
    //        foreach (CharacterData c in currentLine.CharactersInvolved)
    //        {
    //            characterManager.PopulateCharacter(c);
    //        }
    //    }

    //    // Handle Audio
    //    if (!string.IsNullOrEmpty(currentLine.AudioData.SFX)) audioManager.PlaySFXOneShot(currentLine.AudioData.SFX);
    //    if (!string.IsNullOrEmpty(currentLine.AudioData.BGM)) audioManager.PlayBGM(currentLine.AudioData.BGM);
    //    if (!string.IsNullOrEmpty(currentLine.AudioData.DialogueVO)) audioManager.PlayDialogue(currentLine.AudioData.DialogueVO);

    //    // Display the dialog text with typing effect
    //    StopAllCoroutines();
    //    StartCoroutine(TypeText(currentLine.Text));
    //    gameManager.AddScore(currentLine.ScoreDelta);

    //    if(currentLine.Options == null)
    //    {
    //        Debug.LogWarning($"No options found for dialog ID {CurrentDialogID}. Proceeding to next dialog line.");
    //        // If there are no options, Register next line in the dialog based on GOTO value
    //        if (!string.IsNullOrEmpty(currentLine.GOTO))
    //        {
    //            PreviousDialogDay = currentLine.Day; // Store the previous dialog Day
    //            CurrentDialogID = currentLine.GOTO;
    //        }
    //        else Debug.LogWarning($"GOTO value is not set for dialog ID {CurrentDialogID}. No next dialog line will be registered.");
    //    }
    //    else
    //    {
    //        PreviousDialogDay = currentLine.Day; // Store the previous dialog day
    //        Debug.Log($"Displaying options for dialog ID {CurrentDialogID}.");
    //        Debug.Log($"Option 1: {currentLine.Options[0].OptionText}, Option 2: {currentLine.Options[1].OptionText}");
    //        DisplayOptions(currentLine.Options);
    //    }

    //}
    #endregion
}
