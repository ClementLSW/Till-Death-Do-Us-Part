using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static CharacterManager;

public class DialogManager : MonoBehaviour
{
    public struct DialogLine
    {
        public int ID;
        public string Text;
        public List<DialogOptions> Options;
        public List<Characters> CharactersInvolved;
        public List<int> ScoreDelta;
    }

    public struct DialogOptions
    {
        public string OptionText;
        public int NextDialogID;
    }

    public struct Dialog
    {
        public List<DialogLine> Lines;
    }
}
