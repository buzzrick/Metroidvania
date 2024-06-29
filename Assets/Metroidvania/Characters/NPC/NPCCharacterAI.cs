using Buzzrick.AISystems.BehaviourTree;
using Buzzrick.UnityLibs.Attributes;
using Metroidvania.AISystems.Blackboard;
using Metroidvania.Characters.NPC.AI;
using NaughtyAttributes;
using UnityEngine;

namespace Metroidvania.Characters.NPC
{
    public class NPCCharacterAI : MonoBehaviour 
    {
        [Header("Dependencies")]
        [SerializeField, RequiredField] protected NPCCharacterController _npcCharacterController;
        [SerializeField, RequiredField] protected NPCPlayerDetector _playerDetector;
        [SerializeField, RequiredField] protected BehaviourTree LinkedBT;

        [Header("AI")]
        [SerializeField, RequiredField] private NPC_AI_Base AIBrain;

        private Blackboard<BlackboardKey> _blackboard;
        private readonly BlackboardKey _characterControllerKey = new BlackboardKey { Name = "CharacterController" };
        

        private void Awake()
        {
            SetupBlackboard();
            AIBrain.BuildBehaviourTree(LinkedBT, _blackboard, transform);
        }

        private void SetupBlackboard()
        {
            _blackboard = BlackboardManager.Instance.GetIndividualBlackboard<BlackboardKey>(this);
            _blackboard.SetGeneric(_characterControllerKey, _npcCharacterController);
            AIBrain.InitialiseBlackboard(_blackboard, transform);
        }

        [Button("Install required components")]
        public void InstallRequiredComponents()
        {
            AIBrain?.InstallRequiredComponents(transform);
        }

        private void OnDrawGizmosSelected()
        {
            AIBrain?.RenderGizmos(_blackboard, transform);
        }
    }
}