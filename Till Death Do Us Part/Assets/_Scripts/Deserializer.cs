using System;
using System.Collections.Generic;
using UnityEngine;

public class Deserializer : MonoBehaviour
{
    DialogManager dialogManager;

    [SerializeField] TextAsset tsvFile; // Reference to the TSV file in the Resources folder

    private void Awake()
    {
        // Ensure that the DialogManager component is attached to the same GameObject
        dialogManager = GetComponent<DialogManager>();
        if (dialogManager == null)
        {
            Debug.LogError("DialogManager component not found on this GameObject.");
            return;
        }
        dialogManager.SanityCheck();
    }

    // This method reads a TSV file and deserializes it into a Dialog object
    public void ReadTSV(DialogManager dm)
    {
        dialogManager = dm; // Assign the passed DialogManager instance to the local variable

        Debug.Log("Reading TSV file...");
        //TextAsset tsvFile = Resources.Load<TextAsset>("Dialogue V2");

        if (tsvFile == null)
        {
            Debug.LogError("TSV file not found in Resources/Dialogue");
            return;
        }

        string[] lines = tsvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            // Here you would parse each line and convert it into DialogLine objects
            string line_t = line.Trim();
            if (string.IsNullOrEmpty(line_t))
            {
                continue; // Skip empty lines
            }

            // Tokenize the line by tab character
            string[] tokens = line_t.Split('\t'); // Split by tab character
            Debug.Log($"Processing line: {line_t} with {tokens.Length} tokens");

            if (tokens.Length < 12)
            {
                Debug.LogWarning($"Skipping malformed line: {line_t}");
                continue;
            }

            if (tokens[0] == "lineID")
            {
                Debug.LogWarning($"Skipping header line");
                continue;
            }

            DialogManager.DialogLine dialogLine = new DialogManager.DialogLine
            {
                Day = ParseDayOfWeek(tokens[0]), // Parse the day of the week from the first token
                ID = tokens[0],
                Text = tokens[1], // Assuming the second token is the text
                Options = ParseDialogOptions(tokens[2]), // Parse options from the third token
                CharactersInvolved = ParseCharacters(new ArraySegment<string>(tokens, 3, 2).ToArray()), //TIL
                ScoreDelta = int.Parse(tokens[5]),
                AudioData = ParseAudioClips(new ArraySegment<String>(tokens, 6, 3).ToArray()), // Parse audio clips from the sixth to eighth token
                BG = tokens[9], // Assuming the tenth token is the background
                GOTO = tokens[10], // Assuming the eleventh token is the GOTO value
                VFX = tokens[11]
            };

            dialogManager.MasterBank.Lines.Add(dialogLine); // Add the dialog line to the master bank
            //Debug.Log(line);
        }

        foreach (DialogManager.DialogLine line in dialogManager.MasterBank.Lines)
        {
            Debug.Log($"ID: {line.ID}, Text: {line.Text}, ScoreDelta: {line.ScoreDelta}");
            if (line.Options == null || line.Options.Count == 0)
            {
                Debug.LogWarning($"No options for dialog line ID: {line.ID}");
            }
            else
            {
                foreach (var option in line.Options)
                {
                    Debug.Log($"Option: {option.OptionText}, NextDialogID: {option.NextDialogID}");
                }
            }
            foreach (var character in line.CharactersInvolved)
            {
                Debug.Log($"Name: {character.Name} {character.Pose} {character.Emotion}, {character.position}, Active: {character.isActive}");
            }
        }

        Debug.Log("TSV file read and deserialized successfully.");
    }

    #region Parsers
    private List<DialogManager.DialogOptions> ParseDialogOptions(string optionLine)
    {
        if (optionLine.Length == 0)
        {
            Debug.LogWarning("No Options");
            return default; // Return default if there are no options
        }

        string[] options = optionLine.Split(';');// Assuming options are separated by semicolons
        List<DialogManager.DialogOptions> outOptions = new();

        foreach (string option in options)
        {
            string[] optionParts = option.Split(':'); // Assuming format "OptionText:NextDialogID"
            if (optionParts.Length != 2)
            {
                Debug.LogWarning($"Invalid option format: {option}");
                continue; // Skip invalid options
            }

            string optionText = optionParts[0].Trim();
            string nextDialogID = optionParts[1];

            DialogManager.DialogOptions dialogOption = new DialogManager.DialogOptions
            {
                OptionText = optionText,
                NextDialogID = nextDialogID
            };

            outOptions.Add(dialogOption); // Return the first valid option found
        }

        return outOptions;
    }

    private List<CharacterManager.CharacterData> ParseCharacters(string[] characters)
    {
        if (characters.Length == 0 || characters[0].Length == 0)
        {
            Debug.LogWarning("No Characters");
            return default; // Return default if there are no characters
        }

        List<CharacterManager.CharacterData> outCharacters = new();

        for (int i=0; i< characters.Length;i++)
        {
            string character = characters[i];

            if (string.IsNullOrWhiteSpace(character))
            {
                Debug.LogWarning("Empty character string found, skipping.");
                CharacterManager.CharacterData dialogCharacter = new CharacterManager.CharacterData
                {
                    Name = null,
                    Pose = null,
                    Emotion = null,
                    isActive = false,
                    position = i == 0 ? CharacterManager.CharacterData.Position.Left : CharacterManager.CharacterData.Position.Right

            };
                outCharacters.Add(dialogCharacter); // Add the valid character to the list
                continue; // Skip empty character strings
            }
            else
            {
                string[] characterParts = character.Split(':'); // Assuming format "CharID:isActive"
                if (characterParts.Length != 2)
                {
                    Debug.LogWarning($"Invalid character format: {character}");
                    continue; // Skip invalid characters
                }

                string[] characterDetails = characterParts[0].Split('_');
                CharacterManager.CharacterData dialogCharacter = new CharacterManager.CharacterData
                {
                    Name = characterDetails[0].Trim(),
                    Pose = characterDetails[1].Trim(),
                    Emotion = characterDetails[2].Trim(),
                    isActive = characterParts[1].Equals("true"),
                    position = i == 0 ? CharacterManager.CharacterData.Position.Left : CharacterManager.CharacterData.Position.Right

            };
                outCharacters.Add(dialogCharacter); // Add the valid character to the list
            }
        }

        return outCharacters;
    }
    
    private DialogManager.AudioData ParseAudioClips(string[] audio)
    {
        DialogManager.AudioData audioData = new();
        string[] safeAudio = new string[3];

        for (int i = 0; i < 3; i++)
        {
            safeAudio[i] = (i < audio.Length && !string.IsNullOrWhiteSpace(audio[i]) && audio[i] != "null") ? audio[i] : "";
        }

        audioData.SFX = safeAudio[0];
        audioData.BGM = safeAudio[1];
        audioData.DialogueVO = safeAudio[2];

        return audioData;
    }

    private DayOfWeek.Day ParseDayOfWeek(string id)
    {
        if (string.IsNullOrEmpty(id) || id.Length < 3)
        {
            Debug.LogWarning($"Invalid dialog ID format: {id}");
            return DayOfWeek.Day.Monday;
        }

        string prefix = id.Substring(0, 3);

        if (DayAbbreviationMap.TryGetValue(prefix, out var day))
        {
            return day;
        }

        Debug.LogWarning($"Unrecognized day prefix: {prefix} in ID: {id}");
        return DayOfWeek.Day.Monday;
    }
    #endregion

    #region Utils
    private static readonly Dictionary<string, DayOfWeek.Day> DayAbbreviationMap = new()
    {
        { "Mon", DayOfWeek.Day.Monday },
        { "Tue", DayOfWeek.Day.Tuesday },
        { "Wed", DayOfWeek.Day.Wednesday },
        { "Thu", DayOfWeek.Day.Thursday },
        { "Fri", DayOfWeek.Day.Friday },
        { "Sat", DayOfWeek.Day.Saturday },
        { "Sun", DayOfWeek.Day.Sunday }
    };
    #endregion
}
