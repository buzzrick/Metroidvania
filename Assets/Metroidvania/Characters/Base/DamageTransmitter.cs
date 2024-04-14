using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania.Characters
{
    [RequireComponent(typeof(Collider))]
    public class DamageTransmitter : MonoBehaviour
    {
        public float Damage = 1;
        public float DamageInterval = 1;
        private Dictionary<DamageReceiver, Coroutine> _damageReceivers = new Dictionary<DamageReceiver, Coroutine>();


        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out DamageReceiver damageReceiver))
            {
                _damageReceivers.Add(damageReceiver, StartCoroutine(DealDamage(damageReceiver)));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out DamageReceiver damageReceiver))
            {
                if (_damageReceivers.ContainsKey(damageReceiver))
                {
                    StopCoroutine(_damageReceivers[damageReceiver]);
                    _damageReceivers.Remove(damageReceiver);
                }
            }
        }


        private IEnumerator DealDamage(DamageReceiver damageReceiver)
        {
            while (true)
            {
                damageReceiver.TakeDamage(Damage);
                yield return new WaitForSeconds(DamageInterval);
            }
        }
    }
}