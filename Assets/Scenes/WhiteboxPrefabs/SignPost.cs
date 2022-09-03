using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SignPost : MonoBehaviour
{
    [SerializeField] private string _signText;


    [SerializeField, HideInInspector] private TextMeshProUGUI _text;
    private void Reset()
    {
        _text = transform.Find("Flat/Canvas/Text").GetComponent<TextMeshProUGUI>();
    }

    private void Awake()
    {
        _text.text = _signText;
    }

    private void OnValidate()
    {
        _text.text = _signText;
    }
}
