using Cysharp.Threading.Tasks;
using Metroidvania.ResourceTypes;
using System;
using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects
{
    public class ResourceNodeInteractable : MonoBehaviour, IPlayerInteractable, IResourceNode
    {
        public ResourceTypeSO ResourceType;
        public int MaxResourceCount = 5;
        public float ResetTime = 60f;
        private int _currentResourceCount;
        private Vector3 _startingScale;
        private float _resetTimer;

        private void Awake()
        {
            _currentResourceCount = MaxResourceCount;
            _startingScale = transform.localScale;
        }

        public async UniTask<bool> InteractAsync()
        {
            if (_currentResourceCount > 0)
            {
                ScaleOverTime(GetScaleForResourceCount(_currentResourceCount), GetScaleForResourceCount(_currentResourceCount - 1), 0.5f, 0.25f).Forget(); //  don't await this
                _currentResourceCount--;
                _resetTimer = ResetTime;
                return true;
            }
            return false;
        }

        private float GetScaleForResourceCount(int resourceCount)
        {
            if (resourceCount == 0)
                return 0f;
            float scale = 1f - (((float)(MaxResourceCount - resourceCount) / MaxResourceCount) * 0.5f);
            return scale;
        }

        private async UniTask ScaleOverTime(float startingScale, float endingScale, float delay, float time)
        {
            // scale this gameobject from startingScale to endingScale over time seconds 
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            float currentTime = 0.0f;
            do
            {
                transform.localScale = Vector3.Lerp(_startingScale * startingScale, _startingScale * endingScale, currentTime / time);
                currentTime += Time.deltaTime;
                await UniTask.Yield();
            } while (currentTime <= time);
        }



        private void Update()
        {
            if (_resetTimer > 0f)
            {
                _resetTimer -= Time.deltaTime;
                if (_resetTimer <= 0f)
                {
                    _currentResourceCount = MaxResourceCount;
                    ScaleOverTime(GetScaleForResourceCount(0), GetScaleForResourceCount(MaxResourceCount), 0f, 0.25f).Forget(); //  don't await this
                }
            }
        }

        public ResourceTypeSO GetResourceType() => ResourceType;
    }
}