using Cinemachine;
using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.Characters.Player;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using Buzzrick.UnityLibs.Attributes;
using System;

namespace Metroidvania.Cameras
{
    public class CameraController : MonoBehaviour, ICore
    {

        private CinemachineVirtualCameraBase _mainCamera;
        [SerializeField] private CinemachineBrain _brain;

        private int _mainCameraPriority = 0;
        private CinemachineVirtualCameraBase[] _cameras;
        private ICinemachineCamera _cutsceneCameraInterface;
        [SerializeField, RequiredField] private CinemachineVirtualCameraBase _cutsceneCamera;
        private List<CameraZone> _boostedCameras = new();
        private Dictionary<string, int> _defaultCameraPriorities = new();
        private PlayerCore _playerCore;
        private PlayerRoot _playerRoot;
        private Transform _cameraTarget;
        private CameraZone _lastBoostedCamera;
        private bool _pendingRecalculation;

        [Inject]
        private void Initialise(PlayerCore playerCore)
        {
            _playerCore = playerCore;
            _brain.enabled = false;
        }


        public UniTask StartCore()
        {
            _playerRoot = _playerCore.GetPlayerRoot();
            _cameraTarget = _playerRoot.CameraTarget;
            ResetCameraTargets();
            _brain.enabled = true;
            return UniTask.CompletedTask;
        }

        private void Awake()
        {
            _cameras = GetComponentsInChildren<CinemachineVirtualCameraBase>().Where(i=>i.name != "CameraCutscene").ToArray();
            _cutsceneCameraInterface = _cutsceneCamera as ICinemachineCamera;
            //_cutsceneCamera = GetComponentsInChildren<CinemachineVirtualCameraBase>().Where(i => i.name == "CameraCutscene").First();

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

        private void Update()
        {
            if (_pendingRecalculation)
            {
                RecalculateCurrentCamera();
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
                if (_boostedCameras.Count == 0)
                {
                    _lastBoostedCamera = cameraZone;
                }

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
            if (_playerCore.IsPlayerMoving)
            {
                _pendingRecalculation = true;
                return;
            }

            ResetCameraPriorities();
            if (_boostedCameras.Count > 0)
            {
                foreach (CameraZone boostedCamera in _boostedCameras)
                {
                    SetCameraPriority(boostedCamera, _mainCameraPriority + boostedCamera.CameraPriority);
                }
            }
            else if (_lastBoostedCamera != null)
            {
                SetCameraPriority(_lastBoostedCamera, _mainCameraPriority + _lastBoostedCamera.CameraPriority);
            }
            _pendingRecalculation = false;
        }

        private void SetCameraPriority(CameraZone cameraZone, int priority)
        {
            string cameraName = cameraZone.CameraName;
            if (cameraZone.Camera != CameraNames.None)
            {
                cameraName = cameraZone.Camera.ToString();
            }
            GetCamera(cameraName).Priority = priority;
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
                camera.LookAt = _cameraTarget;
                camera.Follow = _playerRoot.transform;
            }
        }

        public async UniTask ShowCutscene(Transform cameraOffset, Transform targetObject)
        {
            _cutsceneCamera.LookAt = targetObject;
            _cutsceneCamera.Follow = cameraOffset;
            Debug.Log($"Starting Camera Blend to cutscene");
            _cutsceneCamera.Priority = 1000;
            //_brain.IsBlending
            await UniTask.WaitWhile(IsBlendingToCutscene);

            Debug.Log($"Finished Camera Blend to cutscene");
        }


        public async UniTask CancelCutscene()
        {
            Debug.Log($"Starting Camera Blend from cutscene");
            _cutsceneCamera.Priority = 0;
            await UniTask.WaitWhile(IsBlendingFromCutscene);
            Debug.Log($"Finished Camera Blend from cutscene");
        }


        private bool IsBlendingToCutscene()
        {
            var activeCamera = _brain.ActiveVirtualCamera;
            var activeBlend = _brain.ActiveBlend;
            return (activeBlend != null && !activeBlend.IsComplete)
                || _brain.IsBlending
                || _cutsceneCameraInterface != activeCamera;
        }


        private bool IsBlendingFromCutscene()
        {
            var activeCamera = _brain.ActiveVirtualCamera;
            var activeBlend = _brain.ActiveBlend;
            return (activeBlend != null && !activeBlend.IsComplete)
                || _brain.IsBlending
                || _cutsceneCameraInterface == activeCamera;
        }

    }
}