#nullable enable
using Cysharp.Threading.Tasks;
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
        private bool _isAnimatingToUnlocked;

        private WorldUnlockNodeBase? _node;
        private Dictionary<GameObject, Vector3> _defaultScales = new Dictionary<GameObject, Vector3>();
        private MeshRenderer? _unlockZoneMesh;
        private Collider? _collider;
        private bool IsUnlocked => _node!.IsUnlocked;

        private bool ParentIsUnlocked => _node?.ParentIsUnlocked ?? true; //  if there's no parent, then it's unlocked
        
        private void Update()
        {
            if (_isAnimatingToUnlocked)
            {
                _timer += Time.deltaTime;
                if (_timer >= ANIMATION_TIME)   //  timer complete
                {
                    SetImmediate();
                    _isAnimatingToUnlocked = false;
                    Debug.Log($"Animation Complete:{name}, {IsUnlocked}");
                }
                else
                {
                    float percent = _timer / ANIMATION_TIME;
                    foreach (var scaleObjectKV in _defaultScales)
                    {
                        scaleObjectKV.Key.transform.localScale = Vector3.Lerp(Vector3.zero, scaleObjectKV.Value, percent);
                    }                        
                }
            }
        }

        public void SetNode(WorldUnlockNodeBase node)
        {
            _node = node;
            //  store the starting scales for all of the objects 
            GatherDefaultScales();
            SetImmediate();
        }
        
        private void GatherDefaultScales()
        {
            //  only need to do this once
            if (_defaultScales.Count == 0)
            {
                // Debug.Log($"Gathering DefaultScales for :{name}", gameObject);
                _unlockZoneMesh = GetComponent<MeshRenderer>();
                _collider = GetComponent<Collider>();
                foreach (var scaleObject in _node!.GetObjects())
                {
                    _defaultScales.Add(scaleObject, scaleObject.transform.localScale);
                }
            }
        }

        /// <summary>
        /// set the unlocked state immediately
        /// </summary>
        public void SetImmediate()
        {
            EnableUnlockZoneMesh();

            EnableNodeObjects();
            foreach (var scaleObjectKV in _defaultScales)
            {
                scaleObjectKV.Key.transform.localScale =
                    IsUnlocked ? scaleObjectKV.Value : Vector3.zero;
            }
        }

        private void EnableUnlockZoneMesh()
        {
            if (_unlockZoneMesh != null)
            {
                //  Show the unlocked Zone if our parent is unlocked and we are still locked
                _unlockZoneMesh.enabled = ParentIsUnlocked && !_node!.IsUnlocked;
            }

            if (_collider != null)
            {
                _collider.enabled = ParentIsUnlocked && !_node!.IsUnlocked;
            }
        }


        public void Animate()
        {
            EnableNodeObjects();
            if (ParentIsUnlocked)
            {
                if (IsUnlocked)
                {
                    AnimateToUnlocked();
                }
                else
                {
                    //  setting locked is immediate
                    SetImmediate();
                }
            }
        }

        private void EnableNodeObjects()
        {
            //  objects are enabled if our parent is unlocked, and we are also unlocked
            bool isEnabled = ParentIsUnlocked && _node!.IsUnlocked;

            //Debug.Log($"Enabling NodeObjects {name}=>{isEnabled}", this);
            foreach (GameObject nodeObject in _node!.GetObjects())    
            {
                nodeObject.SetActive(isEnabled);
            }

            if (_node.ChildScene != null)
            {
                Debug.Log($"Setting WorldUnlockScene {isEnabled}");
                if (isEnabled)
                {
                    _node.ChildScene.LoadChildNode().Forget();
                }
                else
                {
                    _node.ChildScene.UnloadChildNode().Forget();
                }
            }
        }

        private void AnimateToUnlocked()
        {
            if (!_isAnimatingToUnlocked)
            {
                EnableUnlockZoneMesh();
                _isAnimatingToUnlocked = true;
                _timer = 0f;
            }
        }
    }
}