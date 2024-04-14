using System;
using UnityEngine;

namespace Metroidvania.World
{
    /// <summary>
    /// Encapsulates the default stats for a character type.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterStats", menuName = "Metroidvania/CharacterStats")]
    [Serializable]
    public class CharacterStatsContainer : ScriptableObject
    {
        public CharacterStats Stats;
    }
}