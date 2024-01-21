using System;
using UnityEngine;

namespace Metroidvania.GameCore
{
    public class GameLifecycleManager : MonoBehaviour
    {

        public event Action OnGameQuit;
        public event Action<bool> OnGamePaused;


        private void OnApplicationPause(bool pause)
        {
            OnGamePaused?.Invoke(pause);
        }

        private void OnApplicationQuit()
        {
            OnGameQuit?.Invoke();
        }
    }
}