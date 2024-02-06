﻿#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania.World
{
    /// <summary>
    /// Encapsulates the logic for unlocking and displaying an object in the world (ie growing or shrinking)
    /// </summary>
    public class UnlockAnimator : MonoBehaviour
    {
        private float _timer;
        private const float ANIMATION_TIME = 0.5f;
        private bool _isAnimatingToLocked;
        private bool _isAnimatingToUnlocked;

        private WorldUnlockNodeBase? _node;
        private Dictionary<GameObject, Vector3> _defaultScales = new Dictionary<GameObject, Vector3>();
        private bool _isUnlocked;

        private void Update()
        {
            if (_isAnimatingToLocked || _isAnimatingToUnlocked)
            {
                _timer += Time.deltaTime;
                if (_timer >= ANIMATION_TIME)
                {
                    if (_isAnimatingToLocked)
                    {
                        SetLocked();
                    }
                    else
                    {
                        SetUnlocked();
                    }
                    _isAnimatingToLocked = false;
                    _isAnimatingToUnlocked = false;
                    Debug.Log($"Animation Complete:{name}, {_isUnlocked}");
                }
                else
                {
                    float percent = _timer / ANIMATION_TIME;
                    if (_isAnimatingToLocked)
                    {
                        foreach (var scaleObjectKV in _defaultScales)
                        {
                            scaleObjectKV.Key.transform.localScale = Vector3.Lerp(scaleObjectKV.Value, Vector3.zero, percent);
                        }                        
                    }
                    else
                    {
                        foreach (var scaleObjectKV in _defaultScales)
                        {
                            scaleObjectKV.Key.transform.localScale = Vector3.Lerp(Vector3.zero, scaleObjectKV.Value, percent);
                        }                        
                    }
                }
            }
        }

        public void SetNode(WorldUnlockNodeBase node)
        {
            _node = node;
            //  store the starting scales for all of the objects 
            GatherDefaultScales();
            
            if (_node.IsUnlocked)
            {
                SetUnlocked();
            }
            else
            {
                SetLocked();
            }
        }
        
        private void GatherDefaultScales()
        {
            if (_defaultScales.Count == 0)
            {
                foreach (var scaleObject in _node.GetObjects())
                {
                    _defaultScales.Add(scaleObject, scaleObject.transform.localScale);
                }
            }
        }

        public void SetLocked()
        {
            if (isActiveAndEnabled)
            {
                Debug.Log($"Setting Locked {name}", this);
                _isUnlocked = false;
                foreach (var scaleObjectKV in _defaultScales)
                {
                    scaleObjectKV.Key.transform.localScale = Vector3.zero;
                }
            }
        }

        /// <summary>
        /// set the unlocked state immediately
        /// </summary>
        /// <param name="isUnlocked"></param>
        public void SetUnlockedImmediate(bool isUnlocked)
        {
            foreach (var scaleObjectKV in _defaultScales)
            {
                scaleObjectKV.Key.transform.localScale =
                    isUnlocked ? scaleObjectKV.Value : Vector3.zero;
            }
        }

        public void SetUnlocked()
        {
            Debug.Log($"Setting Unlocked {name}", this);
            _isUnlocked = true;
            foreach (var scaleObjectKV in _defaultScales)
            {
                scaleObjectKV.Key.transform.localScale = scaleObjectKV.Value;
            }
        }

        public void Animate(bool isUnlocked)
        {
            if (isUnlocked)
            {
                AnimateToUnlocked();
            }
            else
            {
                AnimateToLocked();
            }
        }
        
        public void AnimateToUnlocked()
        {
            if (!_isUnlocked)
            {
                _isAnimatingToLocked = false;
                _isAnimatingToUnlocked = true;
                _timer = 0f;
            }
        }

        public void AnimateToLocked()
        {
            //  immediate mode - don't animate locking, just do it immediately
            foreach (var scaleObjectKV in _defaultScales)
            {
                scaleObjectKV.Key.transform.localScale = Vector3.zero;
            }

            //if (_isUnlocked)
            //{
            //    _isAnimatingToLocked = true;
            //    _isAnimatingToUnlocked = false;
            //    _timer = 0f;
            //}
        }
    }
}