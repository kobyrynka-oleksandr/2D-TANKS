using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class IpInputValidator : MonoBehaviour
{
    private TMP_InputField _inputField;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
        _inputField.onValidateInput += ValidateIpCharacter;
    }

    private void OnDestroy()
    {
        if (_inputField != null)
        {
            _inputField.onValidateInput -= ValidateIpCharacter;
        }
    }

    private char ValidateIpCharacter(
        string text,
        int charIndex,
        char addedCharacter)
    {
        if ((addedCharacter >= '0' && addedCharacter <= '9')
            || addedCharacter == '.')
        {
            return addedCharacter;
        }

        return '\0';
    }
}