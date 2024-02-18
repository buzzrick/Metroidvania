#nullable enable
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Metroidvania.ResourceTypes
{
    [CreateAssetMenu(fileName = "ResourceTypeDB", menuName = "Metroidvania/Resource Type DB")]
    public class ResourceTypeDB : ScriptableObject
    {
        [SerializeField] private ResourceTypeSO[] ResourceTypes = default!;

        public ResourceTypeSO GetResourceType(string resourceTypeID)
        {
            foreach (var resourceType in ResourceTypes)
            {
                if (resourceType.name == resourceTypeID)
                {
                    return resourceType;
                }
            }
            throw new InvalidDataException($"Unable to find ResourceTypeSO for {resourceTypeID}");
        }


#if UNITY_EDITOR
        private void Reset()
        {
            var allResourceTypes = UnityEditor.AssetDatabase.FindAssets("t:ResourceTypeSO");
            List<ResourceTypeSO> resourceTypes = new List<ResourceTypeSO>();
            foreach (var resourceGuid in allResourceTypes)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(resourceGuid);
                ResourceTypeSO resourceType = UnityEditor.AssetDatabase.LoadAssetAtPath<ResourceTypeSO>(assetPath);

                resourceTypes.Add(resourceType);
            }
            ResourceTypes = resourceTypes.ToArray();
        }
        
        public static ResourceTypeDB EditorInstance()
        {
            var resourceTypeDBGuids = UnityEditor.AssetDatabase.FindAssets("t:ResourceTypeDB");
            if (resourceTypeDBGuids.Length == 0)
            {
                throw new InvalidDataException("Unable to find ResourceTypeDB");
            }
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(resourceTypeDBGuids[0]);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<ResourceTypeDB>(assetPath);
        }
#endif
    }

}