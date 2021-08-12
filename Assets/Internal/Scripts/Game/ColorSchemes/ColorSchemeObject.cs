using System.Collections.Generic;
using UnityEngine;

namespace Internal.Scripts.Game.ColorSchemes {
    [CreateAssetMenu(fileName = "ColorSchemeData", menuName = "ScriptableObjects/ColorSchemeObject", order = 1)]
    public class ColorSchemeObject : ScriptableObject {
        [SerializeField] private List<Color> numberColors = new List<Color>() {Color.gray};
        public List<Color> NumberColors => numberColors;
        
        [SerializeField] private List<Color> textColors = new List<Color>() {Color.white};
        public List<Color> TextColors => textColors;
    }
}