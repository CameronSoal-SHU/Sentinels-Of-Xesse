using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    public static class ControlScheme
    {
        public static KeyCode m_meleeKey { get; set; } = KeyCode.E;
        public static KeyCode m_jumpKey { get; set; } = KeyCode.Space;
        public static KeyCode m_toggleSkillTreeMenu { get; set; } = KeyCode.Tab;
        public static KeyCode m_toggleMainMenu { get; set; } = KeyCode.Escape;
        public static KeyCode m_dashAbility { get; set; } = KeyCode.LeftShift;
    }
}

