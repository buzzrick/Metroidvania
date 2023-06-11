using Cinemachine;
using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.Player;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Metroidvania.Camera
{
    public class CameraController : MonoBehaviour, ICore
    {

        private CinemachineVirtualCameraBase _mainCamera;
        [SerializeField] private CinemachineBrain _brain;

        private int _mainCameraPriority = 0;
        private CinemachineVirtualCameraBase[] _cameras;
        private List<CameraZone> _boostedCameras = new();
        private Dictionary<string, int> _defaultCameraPriorities = new();
        private PlayerCore _playerCore;
        private PlayerRoot _playerRoot;

        [Inject]
        private void Initialise(PlayerCore playerCore)
        {
            _playerCore = playerCore;
            _brain.enabled = false;
        }

        private void Awake()
        {
            _cameras = GetComponentsInChildren<CinemachineVirtualCameraBase>();

            int _mainCameraPriority = -1;
            foreach (var camera in _cameras)
            {
                _defaultCameraPriorities[camera.name] = camera.Priority;
                if (camera.Priority > _mainCameraPriority)
                {
                    _mainCameraPriority = camera.Priority;
                    _mainCamera = camera;
                }
            }
        }

        private CinemachineVirtualCameraBase GetCamera(string cameraName)
        {
            foreach (var camera in _cameras)
            {
                if (camera.name == cameraName)
                {
                    return camera;
                }
            }
            return null;
        }

        public void ClearPrioritisedCamera(CameraZone cameraZone)
        {
            if (_boostedCameras.Contains(cameraZone))
            {
                _boostedCameras.Remove(cameraZone);

                RecalculateCurrentCamera();
            }
        }

        public void SetPrioritisedCamera(CameraZone cameraZone)
        {
            _boostedCameras.Add(cameraZone);
            RecalculateCurrentCamera();
        }

        private void RecalculateCurrentCamera()
        {
            ResetCameraPriorities();
            foreach (CameraZone boostedCamera in _boostedCameras)
            {
                GetCamera(boostedCamera.CameraName).Priority = _mainCameraPriority + 10;
            }
        }

        private void ResetCameraPriorities()
        {
            foreach (var camera in _cameras)
            {
                camera.Priority = _defaultCameraPriorities[camera.Name];
            }
        }

        private void ResetCameraTargets()
        {
            foreach (var camera in _cameras)
            {
                camera.LookAt = _playerRoot.transform;
                camera.Follow = _playerRoot.transform;
            }
        }

        public UniTask StartCore()
        {
            _playerRoot = _playerCore.GetPlayerRoot();
            ResetCameraTargets();
            _brain.enabled = true;
            return UniTask.CompletedTask;
        }

    }
}