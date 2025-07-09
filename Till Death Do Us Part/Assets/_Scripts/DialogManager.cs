using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static CharacterManager;

public class DialogManager : MonoBehaviour
{
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
}
