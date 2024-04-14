using UnityEngine;

namespace Metroidvania.UI
{
    public class BillboardCanvas : MonoBehaviour
    {
        private Canvas _canvas;
        private Camera _mainCamera;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _mainCamera = Camera.main;
            _canvas.worldCamera = _mainCamera;
        }

        private void Update()
        {
            // rotate the _canvas to face the camera
            Quaternion cameraRotation = _mainCamera.transform.rotation;
            _canvas.transform.LookAt(_canvas.transform.position + cameraRotation * Vector3.forward, cameraRotation * Vector3.up);
        }
    }
}