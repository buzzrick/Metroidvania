using UnityEngine;

namespace Metroidvania.World
{
    public abstract class WorldUnlockNodeBase : MonoBehaviour
    {

        [SerializeField] private GameObject[] NodeObjects;
        [SerializeField] private WorldUnlockNode[] ChildNodes;

        [SerializeField] public string NodeID;
        protected string _zoneID;
        protected WorldUnlockData _worldUnlockData;
        protected WorldUnlockData.WorldUnlockNodeData _thisNode;

        public bool IsUnlocked { get; protected set; }

        public virtual void LoadData(WorldUnlockData worldUnlockData, string zoneID)
        {
            _worldUnlockData = worldUnlockData;
            _zoneID = zoneID;
            _thisNode = worldUnlockData.GetOrCreateZone(zoneID).GetOrCreateNode(NodeID);
            CalculateIsUnlocked();
            SetUnlockedState();
            UpdateChildren();
        }

        protected virtual void CalculateIsUnlocked()
        {
            IsUnlocked = _thisNode.IsUnlocked;
        }


        private void SetUnlockedState()
        {
            foreach (GameObject node in NodeObjects)
            {
                node.SetActive(IsUnlocked);
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