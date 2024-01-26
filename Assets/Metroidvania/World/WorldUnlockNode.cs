#nullable enable

using System;
using UnityEngine;
using Zenject;

namespace Metroidvania.World
{
    /// <summary>
    /// Fill this out 
    /// </summary>
    public class WorldUnlockNode : MonoBehaviour
    {
        [SerializeField] private GameObject[] NodeObjects;
        [SerializeField] private WorldUnlockNode[] ChildNodes;
        
        [SerializeField] public string NodeID;
        private string _zoneID;
        private WorldUnlockData _worldUnlockData;
        private WorldUnlockData.WorldUnlockNodeData _thisNode ;

        public bool IsUnlocked { get; private set; }
        
        public void LoadData(WorldUnlockData worldUnlockData, string zoneID)
        {
            _worldUnlockData = worldUnlockData;
            _zoneID = zoneID;
            _thisNode = worldUnlockData.GetOrCreateZone(zoneID).GetOrCreateNode(NodeID);
            IsUnlocked = _thisNode.IsUnlocked;
            UpdateChildren();
        }
        
        

        private void SetUnlockedState()
        {
            foreach (GameObject node in NodeObjects)
            {
                node.SetActive(IsUnlocked);
            }

            UpdateChildren();
        }

        private void UpdateChildren()
        {
            foreach (WorldUnlockNode child in ChildNodes)
            {
                child.LoadData(_worldUnlockData, _zoneID);
            }
        }

        public void Unlock()
        {
            IsUnlocked = true;
            foreach (GameObject node in NodeObjects)
            {
                node.SetActive(IsUnlocked);
            }
        }
    }
}
