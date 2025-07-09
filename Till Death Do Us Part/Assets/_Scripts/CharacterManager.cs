using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public struct Characters
    {
        public enum Position
        {
            Left,
            Right
        }
        public int CharID;
        public string Name;
        public bool isActive;
        public Position position;
        //public Sprite Image;
        //public bool isFlipped;
    }
}
