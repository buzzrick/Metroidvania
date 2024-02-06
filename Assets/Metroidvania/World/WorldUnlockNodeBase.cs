using System;
using Buzzrick.UnityLibs.Attributes;
using UnityEngine;

namespace Metroidvania.World
{
    public abstract class WorldUnlockNodeBase : MonoBehaviour
    {
        [SerializeField] private GameObject[] NodeObjects;
        [SerializeField] private WorldUnlockNode[] ChildNodes;
        public string NodeID => name;
        [SerializeField, RequiredField] private UnlockAnimator _unlockAnimator = default!;

        protected string _zoneID;
        private bool _parentIsUnlocked;
        protected WorldUnlockData _worldUnlockData;
        protected WorldUnlockData.WorldUnlockNodeData _nodeData;

        private bool _isUnlocked;
        private bool _firstLoad = true;

        public WorldUnlockData.WorldUnlockNodeData NodeData => _nodeData;

        public bool IsUnlocked 
        { 
            get=> _isUnlocked;
            protected set
            {
                if (_isUnlocked != value)
                {
                    OnUnlockedChanging(value);
                }
                _isUnlocked = value;
            }
        }

        private void OnDisable()
        {
            // If this node gets disabled (eg: during a reset), we want to reset the animation
            _unlockAnimator.SetLocked();
        }

        protected virtual void Reset()
        {
            _unlockAnimator = GetComponent<UnlockAnimator>();
        }

        public GameObject[] GetObjects() => NodeObjects;
        
        public virtual void LoadData(WorldUnlockData worldUnlockData, string zoneID, bool parentIsUnlocked)
        {
            _worldUnlockData = worldUnlockData;
            _zoneID = zoneID;
            _parentIsUnlocked = parentIsUnlocked;
            _nodeData = worldUnlockData.GetOrCreateZone(zoneID).GetOrCreateNode(NodeID);
            CalculateIsUnlocked();
            SetUnlockedState();
        }

        protected virtual void OnUnlockedChanging(bool isUnlocked) {}
        
        protected virtual void CalculateIsUnlocked()
        {
            IsUnlocked = _nodeData.IsUnlocked;
        }


        private void SetUnlockedState()
        {
            gameObject.SetActive(_parentIsUnlocked);
            if (_firstLoad)
            {
                _unlockAnimator.SetNode(this);
                _firstLoad = false;
            }
            else
            {
                _unlockAnimator.Animate(IsUnlocked);
            }
            UpdateChildren();
        }

        protected void UpdateChildren()
        {
            foreach (WorldUnlockNode child in ChildNodes)
            {
                child.LoadData(_worldUnlockData, _zoneID, _nodeData.IsUnlocked);
            }
        }

        //public void Unlock()
        //{
        //    IsUnlocked = true;
        //    foreach (GameObject node in NodeObjects)
        //    {
        //        node.SetActive(IsUnlocked);
        //    }
        //}

    }
}