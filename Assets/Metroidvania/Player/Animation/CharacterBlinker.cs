using UnityEngine;

namespace Metroidvania.Player.Animation
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

        private void Awake()
        {
            _colorOriginal = ObjectToBlink.material.color;

            if (CharacterDriver == null)
            {
                CharacterDriver = GetComponentInParent<ICharacterMovementDriver>();
            }
            CharacterDriver.RegisterCharacterBlinker(this);
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

                if (ObjectToBlink.material.color == _colorOriginal)
                {
                    ObjectToBlink.material.color = color;
                }
                else
                {
                    ObjectToBlink.material.color = _colorOriginal;
                }
                _blinkCounter++;
            }
            else
            {
                ObjectToBlink.material.color = _colorOriginal;
                _blinkCounter = 0;
                CancelInvoke();
            }
        }
    }
}
