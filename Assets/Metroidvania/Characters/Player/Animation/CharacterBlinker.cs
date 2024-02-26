using System.Linq;
using UnityEngine;

namespace Metroidvania.Characters.Player.Animation
{
    public class CharacterBlinker : MonoBehaviour
    {
        public Renderer ObjectToBlink;
        public ICharacterMovementDriver CharacterDriver;

        //Blinking
        Color _colorOriginal;
        Color color;
        float _R = 2500.0f;
        float _G = 2500.0f;
        float _B = 2500.0f;

        bool _randomColor;
        int _blinkCounter;
        int _stopBlink;

        /// <summary>
        /// Turn on specific handling for Synty characters
        /// </summary>
        private bool _isSynty;
        private const string SyntyColorProperty = "_Texture_Base_Primary";
        private readonly static int SyntyColorPropertyID = Shader.PropertyToID(SyntyColorProperty);
        private MaterialPropertyBlock _syntyProperties;

        private void Awake()
        {
            _syntyProperties = new MaterialPropertyBlock();
            _isSynty = CheckForSyntyCharacter();
            _colorOriginal = GetCurrentColor();

            if (CharacterDriver == null)
            {
                CharacterDriver = GetComponentInParent<ICharacterMovementDriver>();
            }
            CharacterDriver?.RegisterCharacterBlinker(this);
        }


        public void Blink(int times, float speed, float red, float green, float blue)
        {
            CancelInvoke();
            _randomColor = false;
            _R = red;
            _G = green;
            _B = blue;
            _stopBlink = times;
            InvokeRepeating("BlinkInvoke", speed, speed);
        }

        public void Blink(int times, float speed)
        {
            CancelInvoke();
            _randomColor = true;
            _stopBlink = times;
            InvokeRepeating("BlinkInvoke", speed, speed);
        }

        private bool CheckForSyntyCharacter()
        {
            string[] propertyNames = ObjectToBlink.sharedMaterial.GetTexturePropertyNames();
            //  use this to list the available property names
            //Debug.Log($"Found Property Names : {string.Join(",", propertyNames)}");

            if (propertyNames.Contains(SyntyColorProperty))
            {
                ObjectToBlink.GetPropertyBlock(_syntyProperties);
                Debug.Log($"Character is Synty");
                return true;
            }
            return false;   
        }


        public void BlinkInvoke()
        {
            if (_blinkCounter < _stopBlink)
            {
                if (_randomColor)
                {
                    color = new Color((float)UnityEngine.Random.Range(1, 5), (float)UnityEngine.Random.Range(1, 5), (float)UnityEngine.Random.Range(1, 5), 1.0f);
                }
                else
                {
                    color = new Color(_R, _G, _B, 1.0f);
                }


                if (GetCurrentColor() == _colorOriginal)
                {
                    SetColor(color);
                }
                else
                {
                    SetColor(_colorOriginal);
                }
                _blinkCounter++;
            }
            else
            {
                SetColor(_colorOriginal);
                _blinkCounter = 0;
                CancelInvoke();
            }
        }


        private Color GetCurrentColor()
        {
            if (_isSynty)
            {
                return _syntyProperties.GetColor(SyntyColorPropertyID);
            }
            else
            {
                return ObjectToBlink.material.color;
            }
        }

        private void SetColor(Color color)
        {
            if (_isSynty)
            {
                _syntyProperties.SetColor(SyntyColorPropertyID, color);
                ObjectToBlink.SetPropertyBlock(_syntyProperties);
            }
            else
            {
                ObjectToBlink.material.color = color;
            }
        }
    }
}
