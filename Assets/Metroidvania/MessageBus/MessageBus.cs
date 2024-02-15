using System;
using UnityEngine;

namespace Metroidvania.MessageBus
{
    public abstract class MessageBusBase<T> : ScriptableObject
    {
        public event Action<T> OnEvent;

        public void RaiseEvent(T message)
        {
            OnEvent?.Invoke(message);
        }
    }
}