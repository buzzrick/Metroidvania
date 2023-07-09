using Metroidvania.Player.Animation;
using UnityEngine;

namespace Metroidvania.ResourceTypes
{
    [CreateAssetMenu(fileName = "Resource Type", menuName = "Metroidvania/Resource Type")]
    public class ResourceTypeSO : ScriptableObject
    {
        public Sprite ResourceSprite;
        public InteractionActionType InteractionAction;
    }
}