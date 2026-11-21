using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class KeyboardKey : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyLabelField;
    [SerializeField] private Image keyBackgroundImage;

    public event Action<string> OnKeyPressed;

    public string keyValue { get; set; }
    private Button _button;


    private void Awake()
    {
        keyValue = "a"; //set default
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Press);
    }

    public void Press()
    {
        OnKeyPressed?.Invoke(keyValue);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(Press);
    }
}
