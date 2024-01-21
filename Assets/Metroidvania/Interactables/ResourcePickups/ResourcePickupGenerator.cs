using Metroidvania.ResourceTypes;
using UnityEngine;

namespace Metroidvania.Interactables.ResourcePickups
{
    public class ResourcePickupGenerator 
    {

        /// <summary>
        /// Spawn a resource pickup in the world
        /// </summary>
        /// <param name="resourceType">The ResourceType to spawn</param>
        /// <param name="amount">The amount to spawn</param>
        /// <param name="parent">The parent transform for the new ResourcePickup object</param>
        /// <param name="spawnPosition">The world position to spawn the new object</param>
        /// <param name="impulsePosition">The world position to send the object towards with a small impulse</param>
        /// <returns>The new ResourcePickup object</returns> 
        public ResourcePickup GeneratePickup(ResourceTypeSO resourceType, int amount, Transform parent, Vector3 spawnPosition, Vector3 impulsePosition)
        {
            ResourcePickup newObject = GameObject.Instantiate(resourceType.ResourcePickupPrefab, spawnPosition, Random.rotation, parent);
            //  add an impulse upwards and slightly towards the player position
            newObject.AddImpulseToward(impulsePosition);
            return newObject;
        }
    }
}