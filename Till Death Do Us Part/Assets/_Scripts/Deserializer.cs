using System;
using System.Collections.Generic;
using UnityEngine;

public class Deserializer : MonoBehaviour
{
    DialogManager dialogManager;

    private void Awake()
    {
        // Ensure that the DialogManager component is attached to the same GameObject
        dialogManager = GetComponent<DialogManager>();
        if (dialogManager == null)
        {
            Debug.LogError("DialogManager component not found on this GameObject.");
            return;
        }
        dialogManager.SanityCheck(); // Perform sanity check on the DialogManager
        // Ensure the Deserializer is initialized when the game starts
        // ReadTSV();
    }

    // This method reads a TSV file and deserializes it into a Dialog object
    public void ReadTSV(DialogManager dm)
    {
        dialogManager = dm; // Assign the passed DialogManager instance to the local variable

        Debug.Log("Reading TSV file...");
        TextAsset tsvFile = Resources.Load<TextAsset>("Dialogue");

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


            int temp_ID;
            if (!int.TryParse(tokens[0], out temp_ID))
            {
                Debug.LogWarning($"Skipping line due to invalid ID: {line}");
                continue; // If the first token is not an integer, stop processing
            }

            DialogManager.DialogLine dialogLine = new DialogManager.DialogLine
            {
                ID = temp_ID,
                Text = tokens[1], // Assuming the second token is the text
                Options = ParseDialogOptions(tokens[2]), // Parse options from the third token
                CharactersInvolved = ParseCharacters(new ArraySegment<string>(tokens, 3, 2).ToArray()), //TIL
                ScoreDelta = int.Parse(tokens[5])
            };

            dialogManager.MasterBank.Lines.Add(dialogLine); // Add the dialog line to the master bank
            Debug.Log(line);
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
            int nextDialogID;

            if (!int.TryParse(optionParts[1].Trim(), out nextDialogID))
            {
                Debug.LogWarning($"Invalid NextDialogID for option '{optionText}': {optionParts[1]}");
                continue; // Skip if the next dialog ID is not an integer
            }

            DialogManager.DialogOptions dialogOption = new DialogManager.DialogOptions
            {
                OptionText = optionText,
                NextDialogID = nextDialogID
            };

            outOptions.Add(dialogOption); // Return the first valid option found
        }

        return outOptions;
    }

    private List<CharacterManager.Characters> ParseCharacters(string[] characters)
    {
        if (characters.Length == 0 || characters[0].Length == 0)
        {
            Debug.LogWarning("No Characters");
            return default; // Return default if there are no characters
        }

        List<CharacterManager.Characters> outCharacters = new();
        int loopcount = 0;
        foreach (string character in characters)
        {
            string[] characterParts = character.Split(':'); // Assuming format "CharID:isActive"
            if (characterParts.Length != 2)
            {
                Debug.LogWarning($"Invalid character format: {character}");
                continue; // Skip invalid characters
            }

            string[] characterDetails = characterParts[0].Split('_');
            CharacterManager.Characters dialogCharacter = new CharacterManager.Characters
            {
                Name = characterDetails[0].ToString().Trim(),
                Pose = characterDetails[1].ToString().Trim(),
                Emotion = characterDetails[2].ToString().Trim(),
                isActive = characterParts[1].Equals("true"),
                position = CharacterManager.Characters.Position.Left + loopcount // This is Jank

            };

            outCharacters.Add(dialogCharacter); // Add the valid character to the list
            loopcount++; // Increment loop count for position assignment
        }

        return outCharacters;
    }
    #endregion
}
