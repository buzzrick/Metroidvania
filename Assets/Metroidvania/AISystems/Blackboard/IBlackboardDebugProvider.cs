using System.Collections.Generic;

namespace Metroidvania.AISystems.Blackboard
{
    public interface IBlackboardDebugProvider
    {
        IEnumerable<(string key, string value)> GetBlackboardDebugEntries();
    }
}
