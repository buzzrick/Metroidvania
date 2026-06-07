using Metroidvania.AISystems.Blackboard;

namespace Metroidvania.Characters.NPC
{
    public static class NPCBlackboardKeys
    {
        public static readonly BlackboardKey CharacterController = new BlackboardKey { Name = "CharacterController" };
        public static readonly BlackboardKey PlayerDetector      = new BlackboardKey { Name = "PlayerDetector" };
        public static readonly BlackboardKey Transform           = new BlackboardKey { Name = "Transform" };
        public static readonly BlackboardKey Inputs              = new BlackboardKey { Name = "Inputs" };
        public static readonly BlackboardKey StartPosition       = new BlackboardKey { Name = "StartPosition" };
        public static readonly BlackboardKey WanderTarget        = new BlackboardKey { Name = "WanderTarget" };
        public static readonly BlackboardKey WanderVelocity      = new BlackboardKey { Name = "WanderVelocity" };
        public static readonly BlackboardKey WanderTimer         = new BlackboardKey { Name = "WanderTimer" };
        public static readonly BlackboardKey IdleTimer           = new BlackboardKey { Name = "IdleTimer" };
        public static readonly BlackboardKey ShouldFlee          = new BlackboardKey { Name = "ShouldFlee" };
    }
}
