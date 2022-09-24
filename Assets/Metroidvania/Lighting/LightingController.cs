using Cysharp.Threading.Tasks;
using Metroidvania.GameCore;
using Metroidvania.MultiScene;
using System;
using System.Collections.Generic;
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

        public List<LightSetup> LightSettings = new();

        public void EnableLightSetup(string setupName)
        {
            bool isLightFound = false;
            foreach (var light in LightSettings)
            {
                bool isActiveSetup = light.name == setupName;
                if (light.LightingRoot != null)
                {
                    light.LightingRoot.SetActive(isActiveSetup);
                }
                isLightFound &= isActiveSetup;
            }

            if (!isLightFound && LightSettings.Count > 0)
            {
                LightSettings[0].LightingRoot?.SetActive(true);
            }
        }

        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }

        public UniTask StartCore()
        {
            EnableLightSetup("");
            return UniTask.CompletedTask;
        }
    }
}