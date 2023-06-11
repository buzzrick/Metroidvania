using UnityEngine;

namespace Metroidvania.MultiScene
{
    [CreateAssetMenu(fileName = "SceneAnchorOverrides", menuName = "Metriodvania/SceneAnchorOverrides")]
    public class SceneAnchorOverridesSO : ScriptableObject
    {
        public string OverrideAnchorSceneName;
        public Vector3 PlayerPosition;
    }
}