using System.Collections.Generic;
using UnityEngine;

namespace Assets.Metroidvania.AISystems.Blackboard
{
    public class BlackboardManager : MonoBehaviour
    {
        public static BlackboardManager Instance { get; private set; } = null;

        Dictionary<MonoBehaviour, object> IndividualBlackboards = new();
        Dictionary<int, object> SharedBlackboards = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"Trying to create second BlackboardManager on {gameObject.name}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public Blackboard<T> GetIndividualBlackboard<T>(MonoBehaviour requestor) where T : BlackboardKeyBase, new()
        {
            if (!IndividualBlackboards.ContainsKey(requestor))
                IndividualBlackboards[requestor] = new Blackboard<T>();

            return IndividualBlackboards[requestor] as Blackboard<T>;
        }

        public Blackboard<T> GetSharedBlackboard<T>(int uniqueID) where T : BlackboardKeyBase, new()
        {
            if (!SharedBlackboards.ContainsKey(uniqueID))
                SharedBlackboards[uniqueID] = new Blackboard<T>();

            return SharedBlackboards[uniqueID] as Blackboard<T>;
        }
    }
}