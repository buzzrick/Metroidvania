using System.Collections.Generic;

namespace Metroidvania.AISystems.Blackboard
{
    public class BlackboardKey : BlackboardKeyBase, System.IEquatable<BlackboardKey>
    {
        public string Name;

        public override bool Equals(object obj)
        {
            return Equals(obj as BlackboardKey);
        }

        public bool Equals(BlackboardKey other)
        {
            return other is not null &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(Name);
        }

        public static bool operator ==(BlackboardKey left, BlackboardKey right)
        {
            return EqualityComparer<BlackboardKey>.Default.Equals(left, right);
        }

        public static bool operator !=(BlackboardKey left, BlackboardKey right)
        {
            return !(left == right);
        }

        public override string ToString() => $"Key: {Name}";
    }
}