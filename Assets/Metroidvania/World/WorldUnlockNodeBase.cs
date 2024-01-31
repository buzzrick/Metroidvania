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
        protected WorldUnlockData.WorldUnlockNodeData _thisNode;

        private bool _isUnlocked;
        private bool _firstLoad = true;

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
            _thisNode = worldUnlockData.GetOrCreateZone(zoneID).GetOrCreateNode(NodeID);
            CalculateIsUnlocked();
            SetUnlockedState();
            UpdateChildren();
        }

        protected virtual void OnUnlockedChanging(bool isUnlocked) {}
        
        protected virtual void CalculateIsUnlocked()
        {
            IsUnlocked = _thisNode.IsUnlocked;
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
                if (_thisNode.IsUnlocked)
                {
                    child.LoadData(_worldUnlockData, _zoneID);
                }
                child.gameObject.SetActive(_thisNode.IsUnlocked);
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