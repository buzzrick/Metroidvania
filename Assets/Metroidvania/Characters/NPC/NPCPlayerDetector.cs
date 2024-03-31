#nullable enable
using Buzzrick.UnityLibs.Attributes;
using Metroidvania.Characters.Player;
using UnityEngine;
using Zenject;

namespace Assets.Metroidvania.Characters.NPC
{
    /// <summary>
    /// Used for detecting the distance and vector to the player - used for AI.
    /// This ais a great potential for an ECS system.
    /// </summary>
    public class NPCPlayerDetector : MonoBehaviour
    {
        [SerializeField, RequiredField] public Transform PlayerTransform;
        [Tooltip("The number of frames to skip between updates - used for reducing overhead before we get to an ECS implementation")]
        [SerializeField] public int RateLimiter = 1;

        public Vector3 PlayerDirection { get; private set; }
        public float PlayerDistanceSqr { get; private set; }

        private int _frameCounter = 0;

        [Inject]
        private void Initialise([InjectOptional]PlayerCore playerCore)
        {
            if (playerCore != null)
            {
                PlayerTransform = playerCore.GetPlayerRoot().transform;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (_frameCounter < RateLimiter)
            {
                _frameCounter++;
                return;
            }

            PlayerDirection = PlayerTransform.position - transform.position;
            //  flatten the PlayerDirection vector to 2D
            PlayerDirection = new Vector3(PlayerDirection.x, 0, PlayerDirection.z);

            PlayerDistanceSqr = PlayerDirection.sqrMagnitude;
            _frameCounter = 0;
        }
    }
}