using NaughtyAttributes;
using UnityEngine;

namespace Metroidvania.Characters.NPC.AI
{
    public class NPC_AI_DebugButton : MonoBehaviour
    {
        private NPCCharacterAI _npcAI;    
        
        private void Awake()
        {
            _npcAI = GetComponentInChildren<NPCCharacterAI>();
        }

        [Button]
        public void DebugAIGraph()
        {
            Debug.Log("Debugging AI graph");
        }
        
    }
}