using Buzzrick.UnityLibs.Attributes;
using NaughtyAttributes;
using PsychoticLab;
using System;
using UnityEngine;

namespace Medroidvania.CharacterEditor
{
    public class EditorCharacterCreatorUI : MonoBehaviour
    {
        [SerializeField, RequiredField] private CharacterRandomizer Character = default!;
        private CharacterRandomizer.CharacterConfiguration _characterConfiguration;

        private void Start()
        {
            RenderJson();
        }

        [Button]
        public void RandomiseCharacter()
        {
            Character.Randomize();
            RenderJson();
        }

        [Button]
        public void ResetCharacter()
        {
            Character.SetDefaultConfiguration();
            RenderJson();
        }

        [Button]
        private void RenderJson()
        {
            try
            {
                _characterConfiguration = Character.GetConfiguration();
                CharacterJson = JsonUtility.ToJson(_characterConfiguration, true);

                Debug.Log($"Generated Character Json:\n{CharacterJson}");
            }
            catch (Exception ex)
            {   
                Debug.LogError($"Exception while rendering Character Json\n{ex.Message}");
            }
        }

        [Button]
        private void ApplyFromJson()
        {
            _characterConfiguration = JsonUtility.FromJson<CharacterRandomizer.CharacterConfiguration>(CharacterJson);
            Character.ApplyConfiguration(_characterConfiguration);
        }

        /// <summary>
        ///     at the bottom so that it renders at the end of the inspector window
        /// </summary>
        [SerializeField, ResizableTextArea] private string CharacterJson;
    }
}