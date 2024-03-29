using Buzzrick.UnityLibs.Attributes;
using Cysharp.Threading.Tasks;
using Metroidvania.Cameras;
using Metroidvania.Characters.Player;
using UnityEngine;
using Zenject;

namespace Assets.Metroidvania.Camera
{
    public class CutsceneSimple : MonoBehaviour
    {
        [SerializeField, RequiredField] private Transform _lookatTransform;
        [SerializeField, RequiredField] private Transform _cameraPosition;
        [SerializeField] private float _lookatDuration = 2f;

        private CameraController _cameraController;
        private PlayerMovementInputLimiter _inputLimiter;

        [Inject]
        private void Initialise(CameraController cameraController,
            PlayerMovementInputLimiter inputLimiter)
        {
            _cameraController = cameraController;
            _inputLimiter = inputLimiter;
        }

        public async UniTask RunCutscene()
        {
            _inputLimiter.RegisterLimiter(this);
            Debug.Log($"Cutscene - zoom camera in");
            await _cameraController.ShowCutscene(_cameraPosition, _lookatTransform);
            Debug.Log($"Cutscene - Starting delay");
            await UniTask.Delay((int)(_lookatDuration * 1000));
            Debug.Log($"Cutscene - zoom camera back out");
            await _cameraController.CancelCutscene();
            Debug.Log($"Cutscene - complete");
            _inputLimiter.UnregisterLimiter(this);
        }
    }
}