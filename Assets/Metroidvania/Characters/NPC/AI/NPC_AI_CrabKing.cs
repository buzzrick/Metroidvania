using Buzzrick.AISystems.BehaviourTree;
using Metroidvania.AISystems.Blackboard;
using Metroidvania.ResourceTypes;
using System;
using UnityEngine;

namespace Metroidvania.Characters.NPC.AI
{
    [Serializable, CreateAssetMenu(fileName = "NPC AI Crab King", menuName = "Metroidvania/AI/NPC AI Crab King")]
    public class NPC_AI_CrabKing : NPC_AI_Scared
    {
        public ResourceTypeSO ResourceToDetect;
        public float DetectionRadius = 5;
        private float _detectionRadiusSqr;

        protected static BlackboardKey _resourceDetectorKey = new BlackboardKey { Name = "ResourceDetector" };

        public override void InitialiseBlackboard(Blackboard<BlackboardKey> blackboard, Transform characterTransform)
        {
            blackboard.SetGeneric(_resourceDetectorKey, characterTransform.GetComponent<NPCResourceDetector>());

            base.InitialiseBlackboard(blackboard, characterTransform);
        }

        protected override void BuildBehaviourTreeNodes(BTNodeBase BTRoot, Blackboard<BlackboardKey> blackboard)
        {
            _detectionRadiusSqr = DetectionRadius * DetectionRadius;

            // This service will check if the player is closer than the minimum flee distance
            BTRoot.AddService<BTServiceBase>("Check for nearby Resource", (deltaTime) =>
            {
                NPCResourceDetector resourceDetector = blackboard.GetGeneric<NPCResourceDetector>(_resourceDetectorKey);
                blackboard.Set(_shouldFleeKey, (resourceDetector.ResourceDistanceSqr < _detectionRadiusSqr));
            });

            var collectResourceNode = BTRoot.Add<BTNode_Sequence>("Collect Resource");
            collectResourceNode.AddDecorator<BTDecoratorBase>("Can Collect Resource", () =>
            {
                //  this is raised as a separate decorator so that this node can be triggered immediately, even when we're in another later lesser priority node.
                return blackboard.GetGeneric<NPCResourceDetector>(_resourceDetectorKey).IsResourceDetected;
            });

            collectResourceNode.Add<BTNode_Action>(new BTNode_Action("Move To Resource", 
                () =>   //  on Enter state
                {
                    return blackboard.GetGeneric<NPCResourceDetector>(_resourceDetectorKey).IsResourceDetected ? BehaviourTree.ENodeStatus.InProgress : BehaviourTree.ENodeStatus.Failed;
                    //return BehaviourTree.ENodeStatus.InProgress;
                },
                () =>   // OnTick state
                {
                    var resourceDetector = blackboard.GetGeneric<NPCResourceDetector>(_resourceDetectorKey);
                    if (resourceDetector.IsResourceDetected)
                    {
                        if (resourceDetector.ResourceDistanceSqr > 0.25f)
                        {
                            MoveTowardsTarget(resourceDetector.ResourceWorldPosition, _maxVelocity, blackboard);
                        }
                        else
                        {
                            StopMovement(blackboard);
                        }
                        return BehaviourTree.ENodeStatus.InProgress;
                    }
                    return BehaviourTree.ENodeStatus.Succeeded;
                }
                ));

            base.BuildBehaviourTreeNodes(BTRoot, blackboard);
        }

        public override void InstallRequiredComponents(Transform characterTransform)
        {
#if UNITY_EDITOR
            if (characterTransform.GetComponent<NPCResourceDetector>() == null)
            {
                NPCResourceDetector resourceDetector = characterTransform.gameObject.AddComponent<NPCResourceDetector>();
                resourceDetector.DetectionRadius = 5;
                resourceDetector.ResourceToDetect = ResourceToDetect;
            }

            if (characterTransform.GetComponent<NPCResourceHolder>() == null)
            {
                NPCResourceHolder resourceHolder = characterTransform.gameObject.AddComponent<NPCResourceHolder>();
                resourceHolder.ResourceType = ResourceToDetect;
            }
#endif
        }
    }
}