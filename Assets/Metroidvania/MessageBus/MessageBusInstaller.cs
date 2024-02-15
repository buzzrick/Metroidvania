using Metroidvania.MessageBus;
using UnityEngine;
using Zenject;

public class MessageBusInstaller : MonoInstaller
{
    public MessageBusVoid[] MessageBusVoids;
    public MessageBusInt[] MessageBusInts;
    public MessageBusString[] MessageBusStrings;
    public MessageBusTool[] MessageBusTools;

    public override void InstallBindings()
    {
        foreach (var bus in MessageBusVoids)
        {
            Debug.Log($"Binding Bus:{bus.name}");
            Container.BindInstance(bus).WithId(bus.name);
        }
        BindAllBuses(MessageBusInts);
        BindAllBuses(MessageBusStrings);
        BindAllBuses(MessageBusTools);
    }

    private void BindAllBuses<T>(MessageBusBase<T>[] allBuses)
    {
        if (allBuses == null)
            return;
        foreach (var bus in allBuses)
        {
            Debug.Log($"Binding Bus:{bus.name}");
            //Container.Bind<MessageBusBase<T>>().FromInstance(bus).WithConcreteId(bus.name);
            Container.BindInstance(bus).WithId(bus.name);
        }
    }
}