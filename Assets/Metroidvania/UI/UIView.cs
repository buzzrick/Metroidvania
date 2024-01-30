using Cysharp.Threading.Tasks;
using Metroidvania.MultiScene;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania.UI
{

    public class UIView : MonoBehaviour, IView
    {
        private Dictionary<string, List<Action>> callbacks = new Dictionary<string, List<Action>>();

        public UniTask CleanupSelf()
        {
            return UniTask.CompletedTask;
        }

        public void RegisterListener(string messageID, Action callback)
        {
            if (callbacks.TryGetValue(messageID, out var actions))
            {
                actions.Add(callback);
            }
            else
            {
                callbacks[messageID] = new List<Action>() { callback };
            }
        }

        public void UnregisterListener(string messageID, Action callback)
        {
            if (callbacks.TryGetValue(messageID, out var actions))
            {
                if (actions.Contains(callback))
                {
                    actions.Remove(callback);
                }
            }

        }

        internal UniTask StartCore()
        {
            return UniTask.CompletedTask;
        }

        public void TriggerMessage(string messageID)
        {
            if (callbacks.TryGetValue(messageID, out var actions))
            {
                foreach (var action in actions)
                {
                    action.Invoke();
                }
            }
        }
    }
}