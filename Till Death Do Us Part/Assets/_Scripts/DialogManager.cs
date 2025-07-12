using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CharacterManager;

public class DialogManager : MonoBehaviour
{
    Deserializer deserializer;
    CharacterManager characterManager;
    
    #region Data Struct and Init
    // DialogLine represents a single line of dialog
    public struct DialogLine
    {
        public int ID;
        public string Text;
        public List<DialogOptions> Options;
        public List<Characters> CharactersInvolved;
        public int ScoreDelta;
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

    // MasterBank is a collection of all dialog lines
    public Dialog MasterBank;
    private void Awake()
    {
        MasterBank = new();

        MasterBank.Lines = new List<DialogLine>();
        characterManager = GetComponent<CharacterManager>();
        deserializer = GetComponent<Deserializer>();
        deserializer.ReadTSV(this);

        CurrentDialogID = 1;
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
    [SerializeField] public Image CharacterLPoseSprite;
    [SerializeField] public Image CharacterLEmotionSprite;
    [SerializeField] public Image CharacterRPoseSprite;
    [SerializeField] public Image CharacterREmotionSprite;

    [Header("Options Panel")]
    [SerializeField] public GameObject OptionsPanel;
    [SerializeField] public Button BtnL;
    [SerializeField] public Button BtnR;

    [Header("Debug - Do not alter")]
    [SerializeField] private int CurrentDialogID;

    public void NextDialog()
    {
        // Find the current dialog line based on CurrentDialogID
        DialogLine currentLine = MasterBank.Lines.Find(line => line.ID == CurrentDialogID);

        // Populate the character information
        foreach (Characters c in currentLine.CharactersInvolved)
        {
            characterManager.PopulateCharacter(c);
        }

        // Display the dialog text
        DialogTextField.text = currentLine.Text;

        if(currentLine.Options == null)
        {
            Debug.LogWarning($"No options found for dialog ID {CurrentDialogID}. Proceeding to next dialog line.");
            // If there are no options, Register next line in the dialog
            CurrentDialogID++;
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

    // #VibeCoded
    public void SelectOption(int nextDialogID)
    {
        CurrentDialogID = nextDialogID;
        OptionsPanel.SetActive(false);
        NextDialog();
    }
}
