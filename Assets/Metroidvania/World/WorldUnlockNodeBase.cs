#nullable enable
using System;
using Buzzrick.UnityLibs.Attributes;
using UnityEngine;

namespace Metroidvania.World
{
    public abstract class WorldUnlockNodeBase : MonoBehaviour
    {
        [SerializeField] private GameObject[] NodeObjects;
        [SerializeField] private WorldUnlockNode[] ChildNodes;
        public WorldUnlockScene? ChildScene;
        public string NodeID => name;
        [SerializeField, RequiredField] private UnlockAnimator? _unlockAnimator;

        protected string _zoneID;
        protected WorldUnlockData _worldUnlockData;
        protected WorldUnlockData.WorldUnlockNodeData _nodeData;
        protected WorldUnlockData.WorldUnlockNodeData? _parentNodeData;
        public bool ParentIsUnlocked => _parentNodeData?.IsUnlocked ?? true; //  if there's no parent, then it's unlocked

        private bool _isUnlocked;

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

        protected virtual void Reset()
        {
            _unlockAnimator = GetComponent<UnlockAnimator>();
        }

        public GameObject[] GetObjects() => NodeObjects;
        
        public virtual void LoadData(string zoneID, WorldUnlockData worldUnlockData, WorldUnlockData.WorldUnlockNodeData? parentNodeData, bool firstLoad)
        {
            _worldUnlockData = worldUnlockData;
            _parentNodeData = parentNodeData;
            _zoneID = zoneID;
            _nodeData = worldUnlockData.GetOrCreateZone(zoneID).GetOrCreateNode(NodeID);
            CalculateIsUnlocked();
            SetUnlockedState(firstLoad);
        }

        protected virtual void OnUnlockedChanging(bool isUnlocked) {}
        
        protected virtual void CalculateIsUnlocked()
        {
            IsUnlocked = _nodeData.IsUnlocked;
        }


        private void SetUnlockedState(bool firstLoad)
        {
            if (firstLoad)
            {
                _unlockAnimator!.SetNode(this);
            }
            else
            {
                _unlockAnimator!.Animate();
            }
            UpdateChildren(firstLoad);
        }

        protected void UpdateChildren(bool firstLoad)
        {
            foreach (WorldUnlockNode child in ChildNodes)
            {
                child.LoadData(_zoneID, _worldUnlockData, _nodeData, firstLoad);
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

        private static Color ChildGizmoColor = new Color(1f, 0.5f, 0f, 1f); 

        private void OnDrawGizmos()
        {
            //Gizmos.color = Color.yellow;
            Gizmos.color = ChildGizmoColor;
            foreach (WorldUnlockNode child in ChildNodes)
            {
                Gizmos.DrawLine(transform.position, child.transform.position);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            foreach (var node in NodeObjects)
            {
                Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }
    }
}