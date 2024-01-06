using UnityEngine;

namespace Metroidvania.Interactables
{
    public static class IPlayerInteractableExtensions 
    {
        public static string InteractableLayerName = "PlayerInteractable";

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void EnsureCorrectInteractableLayer(this IPlayerInteractable playerInteractable)
        {
            Debug.Log($"Testing {playerInteractable}");
            if (playerInteractable != null)
            {
                MonoBehaviour monoBehaviour = playerInteractable as MonoBehaviour;
                if (monoBehaviour != null)
                {
                    int layer = monoBehaviour.gameObject.layer;
                    if (layer != LayerMask.NameToLayer(InteractableLayerName))
                    {
                        Debug.LogError($"Object {playerInteractable} needs to have Layer set to {InteractableLayerName}", monoBehaviour);
                    }
                }
                else
                {
                    Debug.LogError($"Object {playerInteractable} is not a MonoBehaviour");
                }
            }
        }
    }
}