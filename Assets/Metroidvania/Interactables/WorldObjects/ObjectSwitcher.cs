#nullable enable
using Cysharp.Threading.Tasks;
using Metroidvania.Characters.Player.Animation;
using UnityEngine;

namespace Metroidvania.Interactables.WorldObjects
{

    [RequireComponent(typeof(Collider))]
    public class ObjectSwitcher : MonoBehaviour, IPlayerInteractable
    {
        public ObjectSwitchableBase[] ObjectsToSwitch = default!;
        private int _switchableCount;
        private UniTask[] _animations = default!;
        public AudioClip? InteractionAudioClip;
        public AudioSource? _audioSource;

        public InteractionActionType GetInteractionType() => InteractionActionType.Interact;

        private void Awake()
        {
            this.EnsureCorrectInteractableLayer();
            _switchableCount = ObjectsToSwitch.Length;
            _animations = new UniTask[_switchableCount];
            _audioSource = GetComponent<AudioSource>();
        }

        public async UniTask<bool> Interact(InteractionActionType interactionActionType)
        {
            bool switcherFound = false;
            if (interactionActionType == GetInteractionType())
            {
                for (int i = 0; i < _switchableCount; i++)
                {
                    var switcher = ObjectsToSwitch[i];
                    if (switcher.IsSwitchAvailable)
                    {
                        switcherFound = true;
                        _animations[i] = switcher.ToggleSwitchState();
                    }
                    else
                    {
                        _animations[i] = UniTask.CompletedTask;
                    }
                }
                if (InteractionAudioClip  != null)
                {
                    _audioSource?.PlayOneShot(InteractionAudioClip);
                }
                await UniTask.WhenAll(_animations);
            }

            return switcherFound;
        }

        public virtual bool IsInteractionEnabled
        {
            get
            {
                if (ObjectsToSwitch.Length == 0)
                    return false;

                foreach (var switcher in ObjectsToSwitch)
                {
                    if (!switcher.IsSwitchAvailable)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}