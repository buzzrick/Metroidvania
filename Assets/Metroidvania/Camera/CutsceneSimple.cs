using Buzzrick.UnityLibs.Attributes;
using Cysharp.Threading.Tasks;
using Metroidvania.Cameras;
using Metroidvania.Characters.Player;
using Metroidvania.MessageBus;
using System.Threading;
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
        private MessageBusVoid _skipBus;

        [Inject]
        private void Initialise(CameraController cameraController,
            PlayerMovementInputLimiter inputLimiter,
            [Inject(Id = "Skip")]MessageBusVoid skipBus)
        {
            _cameraController = cameraController;
            _inputLimiter = inputLimiter;
            _skipBus = skipBus;
        }

        public async UniTask RunCutscene()
        {
            CancellationTokenSource skipToken = new CancellationTokenSource(); 
            void SkipCutscene()
            {
                skipToken.Cancel();
            }

            _skipBus.OnEvent += SkipCutscene;
            _inputLimiter.RegisterLimiter(this);
            //Debug.Log($"Cutscene - zoom camera in");
            await _cameraController.ShowCutscene(_cameraPosition, _lookatTransform, skipToken.Token);
            //Debug.Log($"Cutscene - Starting delay");
            await UniTask.Delay((int)(_lookatDuration * 1000), cancellationToken: skipToken.Token);
            //Debug.Log($"Cutscene - zoom camera back out");
            await _cameraController.CancelCutscene();
            //Debug.Log($"Cutscene - complete");
            _skipBus.OnEvent -= SkipCutscene;
            _inputLimiter.UnregisterLimiter(this);
        }
    }
}