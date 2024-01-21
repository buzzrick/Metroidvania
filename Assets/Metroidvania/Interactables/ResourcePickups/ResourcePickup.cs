using Metroidvania.ResourceTypes;
using UnityEngine;

namespace Metroidvania.Interactables.ResourcePickups
{
    [RequireComponent(typeof(Collider))]
    public class ResourcePickup : MonoBehaviour
    {
        public ResourceTypeSO ResourceType;
        public int Amount;
    }
}