using Metroidvania.Interactables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Metroidvania.Player
{
    [RequireComponent(typeof(Collider))]
    public class PlayerTriggerDetector : MonoBehaviour
    {
        private PlayerRoot _playerRoot;
        private List<Collider> _collidersToCheck;
        private List<Collider> _currentColliders = new();
        private bool _checkForTriggerExits;

        public bool LogTriggers = false;

        private void Awake()
        {
            _playerRoot = GetComponent<PlayerRoot>();
        }

        private void OnTriggerEnter(Collider other)
        {
            IPlayerEnterTriggerZone triggerZone = other.GetComponent<IPlayerEnterTriggerZone>();
            if (triggerZone != null)
            {
                if (!_currentColliders.Contains(other))
                {
                    if (LogTriggers) Debug.Log($"Player Entered trigger {other.name}", other);
                    _currentColliders.Add(other);
                    triggerZone.OnPlayerEnteredZone(_playerRoot);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            RaiseOnTriggerExit(other);
        }

        private void RaiseOnTriggerExit(Collider other)
        {
            IPlayerExitTriggerZone triggerZone = other.GetComponent<IPlayerExitTriggerZone>();
            if (triggerZone != null)
            {
                if (LogTriggers) Debug.Log($"Player Exited trigger {other.name}", other);
                //ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);
                if (_currentColliders.Contains(other))
                {
                    _currentColliders.Remove(other);
                    triggerZone.OnPlayerExitedZone(_playerRoot);
                }
                else if (_collidersToCheck?.Contains(other) ?? false)
                {
                    triggerZone.OnPlayerExitedZone(_playerRoot);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_checkForTriggerExits)
            {
                if (LogTriggers) Debug.Log($"Post Teleport Re-Adding trigger zone {other.name}");
                if (!_currentColliders.Contains(other))
                {
                    _currentColliders.Add(other);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_checkForTriggerExits)
            {
                CheckPreviousCollidersStillExist();
                _checkForTriggerExits = false;
            }
        }

        private void CheckPreviousCollidersStillExist()
        {
            foreach (Collider other in _collidersToCheck)
            {
                if (!_currentColliders.Contains(other))
                {
                    if (LogTriggers) Debug.Log($"Post Teleport Trigger not found {other}");
                    RaiseOnTriggerExit(other);
                }
                else
                {
                    if (LogTriggers) Debug.Log($"Post Teleport Trigger still found {other}");
                }
            }

            if (LogTriggers) Debug.Log($"<color=cyan>Post Teleport Complete</color>");
            _collidersToCheck = null;
        }

        public void OnTeleport()
        {
            _collidersToCheck = _currentColliders;
            _currentColliders = new List<Collider>();
            _checkForTriggerExits = true;

            string currentColliderNames = string.Join(",", _collidersToCheck.Select(i => i.name).ToArray());
            if (LogTriggers) Debug.Log($"<color=cyan>On Teleport, the following triggers were active {currentColliderNames}</color>");
        }
    }
}