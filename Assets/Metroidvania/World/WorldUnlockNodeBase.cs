#nullable enable
using Buzzrick.UnityLibs.Attributes;
using NaughtyAttributes;
using System.Text.RegularExpressions;
using Metroidvania.ResourceTypes;
using UnityEngine;
using Assets.Metroidvania.Camera;

namespace Metroidvania.World
{
    public abstract class WorldUnlockNodeBase : MonoBehaviour
    {
        [SerializeField] private GameObject[] NodeObjects = default!;
        [SerializeField] private WorldUnlockNode[] ChildNodes = default!;
        public WorldUnlockScene? ChildScene;
        [SerializeField, RequiredField] private UnlockAnimator? _unlockAnimator;
        public string NodeID => name;
        protected string _zoneID = "";
        protected WorldUnlockData _worldUnlockData = default!;
        protected WorldUnlockData.WorldUnlockNodeData _nodeData = default!;
        protected WorldUnlockData.WorldUnlockNodeData? _parentNodeData;
        [SerializeField] protected CutsceneSimple? _unlockCutscene;
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

        protected bool HasUnlockedChildren()
        {
            foreach (WorldUnlockNode child in ChildNodes)
            {
                if (child.IsUnlocked)
                {
                    return true;
                }
            }
            return false;
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



#if UNITY_EDITOR

        protected const string PrefabPath = "Assets/Metroidvania/World/WorldUnlockNodePrefab.prefab";

        [Button(enabledMode: EButtonEnableMode.Editor)]
        private void GenerateChildNode()
        {
            string newName = GenerateNextNodeName(name);

            WorldUnlockNode nodePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<WorldUnlockNode>(PrefabPath);
            WorldUnlockNode newObject = Instantiate(nodePrefab, transform.position + Vector3.forward, Quaternion.identity, transform);
            newObject.name = newName;
            newObject.transform.SetParent(transform.parent);

            // Set a default Unlock cost of 10 wood (otherwise it'll automatically get unlocked due to having no cost( as the default).
            newObject.ResourceAmounts.Add(ResourceTypeDB.EditorInstance().GetResourceType("Wood"), 10);
            ConvertToPrefab(newObject, nodePrefab);

            // Create a child object to hold the node objects
            GameObject childObjects = new GameObject($"{newName}Objects");
            childObjects.transform.SetParent(newObject.transform);
            childObjects.transform.position = newObject.transform.position;
            childObjects.transform.SetParent(transform.parent);
            newObject.NodeObjects = new GameObject[] { childObjects };

            // Add the new node to the list of child nodes
            WorldUnlockNode[] newChildNodes = new WorldUnlockNode[ChildNodes.Length + 1];
            for (int i = 0; i < ChildNodes.Length; i++)
            {
                newChildNodes[i] = ChildNodes[i];
            }
            newChildNodes[ChildNodes.Length] = newObject;
            ChildNodes = newChildNodes;
            
            Debug.Log($"Create Node: {newName}");
        }


        protected void ConvertToPrefab(WorldUnlockNode worldObject, WorldUnlockNode nodePrefab)
        {
            //  check if we are already a prefab object
            if (IsPrefabObject(worldObject))
            {
                Debug.Log($"This object is already a Prefab object");
                return;
            }

            UnityEditor.ConvertToPrefabInstanceSettings settings = new UnityEditor.ConvertToPrefabInstanceSettings();
            settings.changeRootNameToAssetName = false; // Change root GameObject name to match prefab
            settings.componentsNotMatchedBecomesOverride = true; // Add unmatched components
            settings.gameObjectsNotMatchedBecomesOverride = true; // Add unmatched GameObjects
            settings.recordPropertyOverridesOfMatches = true;   //  Keep property settings

            GameObject prefab = nodePrefab.gameObject;

            UnityEditor.PrefabUtility.ConvertToPrefabInstance(worldObject.gameObject, prefab, settings, UnityEditor.InteractionMode.UserAction);

            Debug.Log($"Linked to Prefab");
        }

        /// <summary>
        /// Detects if we are already a prefab object
        /// </summary>
        /// <returns></returns>
        public bool IsPrefabObject(MonoBehaviour monoBehaviour)
        {
            var prefabInstanceStatus = UnityEditor.PrefabUtility.GetPrefabInstanceStatus(monoBehaviour);
            switch (prefabInstanceStatus)
            {
                case UnityEditor.PrefabInstanceStatus.NotAPrefab:
                case UnityEditor.PrefabInstanceStatus.MissingAsset:
                    return false;
                //case PrefabInstanceStatus.Connected:
                //case PrefabInstanceStatus.Disconnected:
                default:
                    return true;
            }

        }


        private static Regex LastCharacters = new Regex("\\d+$");
        private string GenerateNextNodeName(string nodeName)
        {
            string numericPart = LastCharacters.Match(nodeName).Groups[0].Value;
            string newName;
            string nonNumericPart = nodeName;
            if (int.TryParse(numericPart, out int number))
            {
                nonNumericPart = nodeName.Substring(0, nodeName.Length - numericPart.Length);
            }
            while (true)
            {
                number++;
                newName = $"{nonNumericPart}{number}";
                // se if we can find an object with the same name, and if so increment the number and try again
                var foundObject = GameObject.Find(newName);
                if (foundObject == null)
                {
                    break;
                }
            }

            return newName;
        }
#endif

    }
}