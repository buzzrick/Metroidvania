using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Metroidvania.Lighting
{
    public class LightingController : MonoBehaviour, IView, ICore
    {
        [Serializable]
        public class LightSetup
        {
            public string name;
            public GameObject LightingRoot;
        }

        public bool LogLightChanges = false;

        public List<LightSetup> LightSettings = new();
        private Dictionary<string, int> _lightStack = new();

        public void EnableLightSetup(string setupName, string sourceName)
        {
            IncreaseLightStack(setupName, sourceName);
            SetCorrectLightSetup(GetCurrentLightSetting());
        }
        public void DisableLightSetup(string setupName, string sourceName)
        {
            DecreaseLightStack(setupName, sourceName);
            SetCorrectLightSetup(GetCurrentLightSetting());
        }


        private void SetCorrectLightSetup(string setupName)
        {
            bool isLightFound = false;
            foreach (var light in LightSettings)
            {
                bool isActiveSetup = light.name == setupName;
                if (light.LightingRoot != null)
                {
                    if (LogLightChanges) Debug.Log($"Setting active light setup to {setupName}");
                    light.LightingRoot.SetActive(isActiveSetup);
                }
                isLightFound |= isActiveSetup;
            }

            if (!isLightFound && LightSettings.Count > 0)
            {
                LightSettings[0].LightingRoot?.SetActive(true);
            }
        }

        /// <summary>
        /// Increase the count of how many times a light setting has been enabled
        /// </summary>
        /// <param name="setupName"></param>
        /// <returns></returns>
        private void IncreaseLightStack(string setupName, string sourceName)
        {
            if (string.IsNullOrEmpty(setupName))
                return;
            _lightStack.TryGetValue(setupName, out int stackValue);
            stackValue++;
            _lightStack[setupName] = stackValue;

            if (LogLightChanges) Debug.Log($"Increased LightSetup {setupName} to {stackValue} due to {sourceName}");
        }

        /// <summary>
        /// Decrease the count of how many times a light setting has been enabled
        /// </summary>
        /// <param name="setupName"></param>
        /// <returns>Returns true if the light setup should still be active</returns>
        private bool DecreaseLightStack(string setupName, string sourceName)
        {
            if (string.IsNullOrEmpty(setupName))
                return false;

            _lightStack.TryGetValue(setupName, out int stackValue);
            if (stackValue > 0)
            {
                stackValue--;
                if (stackValue <= 0)
                {
                    _lightStack.Remove(setupName);
                }
                else
                {
                    _lightStack[setupName] = stackValue;
                }
            }
            if (LogLightChanges) Debug.Log($"Decreased LightSetup {setupName} to {stackValue} due to {sourceName}");
            return stackValue > 0;
        }

        private string GetCurrentLightSetting()
        {
            return _lightStack.FirstOrDefault().Key;
        }

        public UniTask CleanupSelf()
        {
            _lightStack.Clear();
            return UniTask.CompletedTask;
        }

        public UniTask StartCore()
        {
            EnableLightSetup("", "");
            return UniTask.CompletedTask;
        }

    }
}