using System;
using Buzzrick.UnityLibs.Attributes;
using UnityEngine;

namespace Metroidvania.World
{
    public abstract class WorldUnlockNodeBase : MonoBehaviour
    {
        [SerializeField] private GameObject[] NodeObjects;
        [SerializeField] private WorldUnlockNode[] ChildNodes;
        [SerializeField] public string NodeID;
        [SerializeField, RequiredField] private UnlockAnimator _unlockAnimator = default!;

        protected string _zoneID;
        
        protected WorldUnlockData _worldUnlockData;
        protected WorldUnlockData.WorldUnlockNodeData _nodeData;

        private bool _isUnlocked;
        private bool _firstLoad = true;

        public string ZoneID => _zoneID;
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

        public GameObject[] GetObjects() => NodeObjects;
        
        public virtual void LoadData(WorldUnlockData worldUnlockData, string zoneID)
        {
            _worldUnlockData = worldUnlockData;
            _zoneID = zoneID;
            _nodeData = worldUnlockData.GetOrCreateZone(zoneID).GetOrCreateNode(NodeID);
            CalculateIsUnlocked();
            SetUnlockedState();
            UpdateChildren();
        }

        protected virtual void OnUnlockedChanging(bool isUnlocked) {}
        
        protected virtual void CalculateIsUnlocked()
        {
            IsUnlocked = _nodeData.IsUnlocked;
        }


        private void SetUnlockedState()
        {
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
                if (_nodeData.IsUnlocked)
                {
                    child.LoadData(_worldUnlockData, _zoneID);
                }
                child.gameObject.SetActive(_nodeData.IsUnlocked);
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