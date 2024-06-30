using Cysharp.Threading.Tasks;
using Metroidvania.ResourceTypes;
using System;
using Metroidvania.Characters.Player.Animation;
using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects
{
    public class ResourceNodeInteractable : MonoBehaviour, IResourceNode
    {
        public ResourceTypeSO ResourceType;
        public int MaxResourceCount = 5;
        public float ResetTime = 60f;
        private int _currentResourceCount;
        private Vector3 _startingScale;
        private float _resetTimer;

        //  todo: detect when there is no more resources to harvest
        public bool IsInteractionEnabled
        {
            get
            {
                if (ResourceType != null)
                    return _currentResourceCount > 0;
                return false;
            }
        }

        private void Awake()
        {
            _currentResourceCount = MaxResourceCount;
            _startingScale = transform.localScale;
            this.EnsureCorrectInteractableLayer();
        }

        public InteractionActionType GetInteractionType() =>
            IsInteractionEnabled ? ResourceType.InteractionAction : InteractionActionType.None;

        public (ResourceTypeSO resourceType, int amount) GetResource()
        {
            return (ResourceType, MaxResourceCount);
        }

        public UniTask<bool> Interact(InteractionActionType interactionActionType)
        {
            if (interactionActionType != GetInteractionType())
                return new(false);
            
            if (_currentResourceCount > 0)
            {
                ScaleOverTime(GetScaleForResourceCount(_currentResourceCount), GetScaleForResourceCount(_currentResourceCount - 1), 0.5f, 0.25f).Forget(); //  don't await this
                _currentResourceCount--;
                //  reset the respawn timer
                if (_resetTimer <= 0f)
                {
                    _resetTimer = ResetTime;
                }
                return new (true);
            }
            return new (false);
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

        private void Reset()
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerPickup");
        }
    }
}