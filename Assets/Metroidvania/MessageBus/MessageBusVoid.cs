using System;
using UnityEngine;

namespace Metroidvania.MessageBus
{
    [CreateAssetMenu(fileName = "New Message Bus", menuName = "Metroidvania/MessageBus/Void")]
    public class MessageBusVoid : ScriptableObject
    {
        public event Action OnEvent;

        public void RaiseEvent()
        {
            OnEvent?.Invoke();
        }
    }
}