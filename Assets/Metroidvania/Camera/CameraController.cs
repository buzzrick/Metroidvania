using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Metroidvania.Camera
{
    public class CameraController : MonoBehaviour
    {

        private CinemachineVirtualCameraBase _mainCamera;

        private int _mainCameraPriority = 0;
        private CinemachineVirtualCameraBase[] _cameras;
        private List<CameraZone> _boostedCameras = new();
        private Dictionary<string, int> _defaultCameraPriorities = new();

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
    }
}