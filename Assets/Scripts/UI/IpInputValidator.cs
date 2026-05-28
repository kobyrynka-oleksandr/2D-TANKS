using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class IpInputValidator : MonoBehaviour
{
    private TMP_InputField m_InputField;

    private void Awake()
    {
        m_InputField = GetComponent<TMP_InputField>();
        m_InputField.onValidateInput += ValidateIpChar;
    }

    private char ValidateIpChar(string text, int charIndex, char addedChar)
    {
        if ((addedChar >= '0' && addedChar <= '9') || addedChar == '.')
        {
            return addedChar;
        }

        return '\0';
    }

    private void OnDestroy()
    {
        if (m_InputField != null)
        {
            m_InputField.onValidateInput -= ValidateIpChar;
        }
    }
}