using NaughtyAttributes;
using UnityEngine;


namespace Metroidvania.Configuration
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Metroidvania/Game Configuration")]
    public class GameConfiguration : ScriptableObject
    {
        [SerializeField] private bool _debugOptionsEnabled;
        public bool DebugOptionsEnabled => _debugOptionsEnabled;

        [SerializeField, EnableIf("DebugOptionsEnabled")] private bool _freeWorldUnlocks;
        public bool FreeWorldUnlocks => DebugOptionsEnabled &&  (_freeWorldUnlocks || FreeWorldUnlocksDebugging);
        
        /// <summary>
        /// Allows us to modify free world unlocks for debugging at runtime (resets every time the game starts
        /// </summary>
        public bool FreeWorldUnlocksDebugging { get; set; } 
    }
}