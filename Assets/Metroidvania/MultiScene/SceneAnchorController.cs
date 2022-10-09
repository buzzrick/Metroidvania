using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Metroidvania.GameCore;
using Metroidvania.Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Metroidvania.MultiScene
{
    public class SceneAnchorController : MonoBehaviour, IView, ICore
    {
        private List<string> _loadedScenes = new();
        private List<string> _pendingScenes = new();
        private PlayerCore _playerCore;
        private ScenePartAnchor[] _anchors;
        private int _sceneAnchorLayerMask;

        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }

        [Inject]
        private void Initialise(PlayerCore playerCore)
        {
            _playerCore = playerCore;
            _anchors = GetComponentsInChildren<ScenePartAnchor>();
            _sceneAnchorLayerMask = LayerMask.GetMask("SceneAnchors");
        }

        public void HandleSceneAnchorLoading(string sceneName)
        {
            _pendingScenes.Add(sceneName);
        }

        public void HandleSceneAnchorLoaded(string sceneName)
        {
            _pendingScenes.Remove(sceneName);
            _loadedScenes.Add(sceneName);
        }

        public void HandleSceneAnchorUnLoaded(string sceneName)
        {
            _loadedScenes.Remove(sceneName);
        }

        public async UniTask StartCore()
        {
            Debug.Log($"Starting SceneAnchor Controller");
            ForceCalculation();
            await UniTask.WaitUntil(IsReadyToStartGame);
        }

        private bool IsReadyToStartGame()
        {
            //Debug.Log($"SceneAnchor Controller Loaded:{_loadedScenes.Count}, Pending:{_pendingScenes.Count}");
            return _loadedScenes.Count > 0 && _pendingScenes.Count == 0;
        }

        private Collider[] _forceHits = new Collider[10];

        public bool ForceCalculation()
        {
            PlayerRoot playerRoot = _playerCore.GetPlayerRoot();
            int count = Physics.OverlapSphereNonAlloc(playerRoot.transform.position, 0.5f, _forceHits, _sceneAnchorLayerMask, QueryTriggerInteraction.Collide);
            bool hasHit = false;

            Debug.Log($"SceneAnchorController ForceCalculation found {count} hits");
            for (int i = 0; i < count; i++)
            {
                Collider sceneAnchorCollider = _forceHits[i];
                if (sceneAnchorCollider != null)
                {
                    ScenePartAnchor anchor = sceneAnchorCollider.GetComponent<ScenePartAnchor>();
                    anchor.OnPlayerEnteredZone(null);
                    hasHit = true;
                }
            }
            return hasHit;
        }
    }
}