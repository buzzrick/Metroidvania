using Buzzrick.AISystems.BehaviourTree;
using Buzzrick.UnityLibs.Attributes;
using KinematicCharacterController.Examples;
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

        private void Awake()
        {
            SetupBrain();
        }

        private void SetupBrain()
        {
            _blackboard = BlackboardManager.Instance.GetIndividualBlackboard<BlackboardKey>(this);

            // Populate all standard keys from Inspector-assigned fields.
            // Graph-based AIs read directly from these; code-based AIs may supplement via InitialiseBlackboard.
            _blackboard.SetGeneric(NPCBlackboardKeys.CharacterController, _npcCharacterController);
            _blackboard.SetGeneric(NPCBlackboardKeys.PlayerDetector,      _playerDetector);
            _blackboard.SetGeneric(NPCBlackboardKeys.Transform,           transform);
            _blackboard.Set(NPCBlackboardKeys.StartPosition,              transform.position);
            _blackboard.SetGeneric(NPCBlackboardKeys.Inputs,              new AICharacterInputs());
            _blackboard.Set(NPCBlackboardKeys.WanderTarget,               transform.position);
            _blackboard.Set(NPCBlackboardKeys.WanderVelocity,             0f);

            var animator = GetComponentInChildren<Animator>();
            if (animator != null)
                _blackboard.SetGeneric(NPCBlackboardKeys.Animator, animator);

            AIBrain.InitialiseBlackboard(_blackboard, transform);
            AIBrain.BuildBehaviourTree(LinkedBT, _blackboard);
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
