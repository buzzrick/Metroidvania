using Buzzrick.UnityLibs.Attributes;
using TMPro;
using UnityEngine;

namespace Metroidvania.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class VersionText : MonoBehaviour
    {
        [SerializeField, RequiredField] private TMP_Text _textField = default!;

        private void Start()
        {
            _textField.text = Application.version;
        }

        private void Reset()
        {
            _textField = GetComponent<TMP_Text>();
        }
    }
}