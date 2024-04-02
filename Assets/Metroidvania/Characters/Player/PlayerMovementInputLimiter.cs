using System;
using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania.Characters.Player
{
    public class PlayerMovementInputLimiter 
    {
        private List<object> _limiters = new List<object>();

        public event Action<bool> OnInputAllowedChanged;
        public bool IsMovementInputAllowed { get; private set; } = true;

        public void RegisterLimiter(object limiter)
        {
            if (_limiters.Contains(limiter))
            {
                throw new System.Exception("Limiter already registered");
            }

            _limiters.Add(limiter);
            IsMovementInputAllowed = false;

            //Debug.Log($"Registering {IsMovementInputAllowed}");
            OnInputAllowedChanged?.Invoke(false);
        }


        public void UnregisterLimiter(object limiter)
        {
            if (!_limiters.Contains(limiter))
            {
                throw new System.Exception("Limiter not registered");
            }

            _limiters.Remove(limiter);
            IsMovementInputAllowed = _limiters.Count == 0;
            Debug.Log($"Unregistering {IsMovementInputAllowed} ({_limiters.Count})");
            OnInputAllowedChanged?.Invoke(IsMovementInputAllowed);
        }
    }
}