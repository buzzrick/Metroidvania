using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ListButton : MonoBehaviour
{
    public TMPro.TextMeshProUGUI ButtonText;
    private Button _button;

    public event Action<string> OnButtonClick;

    public void SetButtonText(string text)
    {
        _button = GetComponent<Button>();
        ButtonText.text = text;
        _button.onClick.AddListener(HandleButtonClick);
    }

    private void HandleButtonClick()
    {
        OnButtonClick?.Invoke(ButtonText.text);
    }
}
