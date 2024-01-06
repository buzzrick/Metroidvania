using Metroidvania.ResourceTypes;

namespace Metroidvania.Interactables.WorldObjects
{
    public interface IResourceNode : IPlayerInteractable
    {
        ResourceTypeSO GetResourceType();
        public (ResourceTypeSO resourceType, int amount) GetResource();
    }
}