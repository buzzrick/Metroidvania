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
        protected static BlackboardKey _resourceHolderDetectorKey = new BlackboardKey { Name = "ObjectDetector_ResourceHolder" };

        public override void InitialiseBlackboard(Blackboard<BlackboardKey> blackboard, Transform characterTransform)
        {
            var resourceDetector = characterTransform.GetComponent<NPCResourceDetector>();
            // set up the ResourceHolderDetector to detect the resource we're looking for
            var resourceHolderDetector = characterTransform.GetComponent<NPCObjectDetector_ResourceHolder>();
            resourceHolderDetector.IsDetectionValid = (resourceHolder) =>
                        resourceHolder.IsHoldingResource
                        && resourceHolder.ResourceType == ResourceToDetect;

            blackboard.SetGeneric(_resourceDetectorKey, resourceDetector);
            blackboard.SetGeneric(_resourceHolderDetectorKey, resourceHolderDetector);
            
            base.InitialiseBlackboard(blackboard, characterTransform);
        }

        protected override void BuildBehaviourTreeNodes(BTNodeBase BTRoot, Blackboard<BlackboardKey> blackboard)
        {
            _detectionRadiusSqr = DetectionRadius * DetectionRadius;
            BuildCollectResourceNodes(BTRoot, blackboard);
            BuildChaseSoulHolderNPCNodes(BTRoot, blackboard);
            base.BuildBehaviourTreeNodes(BTRoot, blackboard);
        }

        protected void BuildCollectResourceNodes(BTNodeBase BTRoot, Blackboard<BlackboardKey> blackboard)
        {

            // This service will check if the player is closer than the minimum flee distance
            BTRoot.AddService<BTServiceBase>($"Check for nearby {ResourceToDetect}", (deltaTime) =>
            {
                NPCResourceDetector resourceDetector = blackboard.GetGeneric<NPCResourceDetector>(_resourceDetectorKey);
                blackboard.Set(_shouldFleeKey, (resourceDetector.ResourceDistanceSqr < _detectionRadiusSqr));
            });

            var attackCooldownNode = BTRoot.Add<BTNode_Action>(new BTNode_Action("Attack Cooldown", //  if we've attacked recently, wait for the cooldown to expire before doing anything else
                () => { 
                    NPCObjectDetector_ResourceHolder resourceDetector = blackboard.GetGeneric<NPCObjectDetector_ResourceHolder>(_resourceHolderDetectorKey);
                    return (resourceDetector.PauseTimerRemaining > 0f) ? BehaviourTree.ENodeStatus.InProgress : BehaviourTree.ENodeStatus.Failed;
                },
                () =>
                {
                    NPCObjectDetector_ResourceHolder resourceDetector = blackboard.GetGeneric<NPCObjectDetector_ResourceHolder>(_resourceHolderDetectorKey);
                    return (resourceDetector.PauseTimerRemaining > 0f) ? BehaviourTree.ENodeStatus.InProgress : BehaviourTree.ENodeStatus.Succeeded;
                }));

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
                            return BehaviourTree.ENodeStatus.InProgress;
                        }
                        else
                        {
                            StopMovement(blackboard);
                        }
                    }
                    return BehaviourTree.ENodeStatus.Succeeded;
                }
                ));

            collectResourceNode.Add<BTNode_Action>(new BTNode_Action("Take Resource Home",
                () =>
                {
                    return BehaviourTree.ENodeStatus.InProgress;
                },
                () =>
                {
                    var resourceHolder = blackboard.GetGeneric<NPCResourceHolder>(_resourceDetectorKey);
                    if (resourceHolder.IsHoldingResource)
                    {
                        var homePosition = blackboard.GetVector3(_startPositionKey);
                        if ((homePosition - resourceHolder.transform.position).sqrMagnitude > 0.25f)
                        {
                            MoveTowardsTarget(homePosition, _maxVelocity, blackboard);
                            return BehaviourTree.ENodeStatus.InProgress;
                        }
                        else
                        {
                            StopMovement(blackboard);
                            return BehaviourTree.ENodeStatus.Succeeded;
                        }
                    }
                    return BehaviourTree.ENodeStatus.Failed;
                }
                ));
        }


        protected void BuildChaseSoulHolderNPCNodes(BTNodeBase BTRoot, Blackboard<BlackboardKey> blackboard)
        {
            // This service will check if the player is closer than the minimum flee distance
            BTRoot.AddService<BTServiceBase>($"Check for nearby {ResourceToDetect} holder", (deltaTime) =>
            {
                NPCObjectDetector_ResourceHolder resourceDetector = blackboard.GetGeneric<NPCObjectDetector_ResourceHolder>(_resourceHolderDetectorKey);
                blackboard.Set(_shouldFleeKey, (resourceDetector.ObjectDistanceSqr < _detectionRadiusSqr));
            });

            var collectResourceNode = BTRoot.Add<BTNode_Sequence>($"Attack {ResourceToDetect} holder");
            collectResourceNode.AddDecorator<BTDecoratorBase>("Can See holder", () =>
            {
                //  this is raised as a separate decorator so that this node can be triggered immediately, even when we're in another later lesser priority node.
                return blackboard.GetGeneric<NPCObjectDetector_ResourceHolder>(_resourceHolderDetectorKey).IsObjectDetected;
            });

            collectResourceNode.Add<BTNode_Action>(new BTNode_Action("Move To holder",
                () =>   //  on Enter state
                {
                    return blackboard.GetGeneric<NPCObjectDetector_ResourceHolder>(_resourceHolderDetectorKey).IsObjectDetected ? BehaviourTree.ENodeStatus.InProgress : BehaviourTree.ENodeStatus.Failed;
                    //return BehaviourTree.ENodeStatus.InProgress;
                },
                () =>   // OnTick state
                {
                    var resourceDetector = blackboard.GetGeneric<NPCObjectDetector_ResourceHolder>(_resourceHolderDetectorKey);
                    if (resourceDetector.IsObjectDetected)
                    {
                        if (resourceDetector.ObjectDistanceSqr > 3f)
                        {
                            //Debug.Log($"Moving to target {resourceDetector.ObjectDistanceSqr}");
                            MoveTowardsTarget(resourceDetector.ObjectWorldPosition, _maxVelocity, blackboard);
                            return BehaviourTree.ENodeStatus.InProgress;
                        }
                        else
                        {
                            StopMovement(blackboard);
                            return BehaviourTree.ENodeStatus.Succeeded;
                        }
                    }
                    return BehaviourTree.ENodeStatus.Failed;
                }
                ));

            collectResourceNode.Add<BTNode_Action>(new BTNode_Action("Attack Holder",
                () => { //  on Enter state
                    var resourceDetector = blackboard.GetGeneric<NPCObjectDetector_ResourceHolder>(_resourceHolderDetectorKey);
                    if (resourceDetector.IsObjectDetected)
                    {
                        //Debug.Log($"Triggering Attack on {resourceDetector.DetectedObject.name}");
                        resourceDetector.DetectedObject.DropResource();
                        resourceDetector.PauseDetection(2f);
                        return BehaviourTree.ENodeStatus.InProgress;
                    }
                    return BehaviourTree.ENodeStatus.Failed;
                },
                () => { //  OnTick state
                    var resourceDetector = blackboard.GetGeneric<NPCObjectDetector_ResourceHolder>(_resourceHolderDetectorKey);

                    //Debug.Log($"Attack pause {resourceDetector.PauseTimerRemaining}");    //  timer currently not ticking down 100%. Another node is taking over, but it works fine for now.
                    return (resourceDetector.PauseTimerRemaining > 0f)
                        ? BehaviourTree.ENodeStatus.InProgress 
                        : BehaviourTree.ENodeStatus.Succeeded;
                }
                ));
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

            if (characterTransform.GetComponent<NPCObjectDetector_ResourceHolder>() == null)
            {
                NPCObjectDetector_ResourceHolder objectDetector = characterTransform.gameObject.AddComponent<NPCObjectDetector_ResourceHolder>();
                objectDetector.DetectionRadius = 5;
            }
#endif
        }
    }
}