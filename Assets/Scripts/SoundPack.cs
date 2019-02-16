using UnityEngine;
using System.Collections.Generic;
using System;

namespace EscapeHorror.Prototype {
    [CreateAssetMenu(menuName = "Data Pack/Sound Pack")]
    public class SoundPack : ScriptableObject
    {
        [SerializeField]
        public List<AudioClip> Sounds;
    }
}