using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public struct Characters
    {
        public int CharID;
        public string Name;
        public Sprite Image;
        public enum Position
        {
            Left,
            Right
        }
        public bool isFlipped;
    }
}
