using Metroidvania.Characters.Player.Animation;
using UnityEngine;

namespace Metroidvania.MessageBus
{

    [CreateAssetMenu(fileName = "New Message Bus", menuName = "Metroidvania/MessageBus/ToolLevel")]
    public class MessageBusTool : MessageBusBase<ToolLevel> {
    }
    public struct ToolLevel
    {
        public PlayerAnimationTool Tool;
        public int Level;
    }

}
