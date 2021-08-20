using System.Collections.Generic;
using UnityEngine;

namespace Internal.Scripts.Game.DataObjects.ScriptableObjects {
    [CreateAssetMenu(fileName = "AudioSchemeData", menuName = "ScriptableObjects/AudioSchemeObject", order = 1)]
    public class AudioSchemeObject : ScriptableObject {
        [SerializeField] private List<AudioClip> hitSounds = new List<AudioClip>();
        [SerializeField] private List<AudioClip> ballDestructionSounds = new List<AudioClip>();

        public List<AudioClip> HitSounds => hitSounds;

        public List<AudioClip> BallDestructionSounds => ballDestructionSounds;
    }
}