using UnityEngine;
using System.Collections.Generic;

namespace EscapeHorror.Prototype { 
    [CreateAssetMenu(menuName = "Data Pack/Character Pack")]
    public class CharacterPack : ScriptableObject
    {
        public enum CharacterType {
            AKANE,TAKERU,MIKI,RYOTA
        }
    
        [SerializeField]
        public CharacterType Character;
        [SerializeField]
        public Rect Size;
        [SerializeField]
        public List<Sprite> Sprites;
    }
}