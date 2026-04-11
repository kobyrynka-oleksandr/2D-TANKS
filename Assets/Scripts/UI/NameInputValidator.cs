using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class NameInputValidator : MonoBehaviour
{
    [SerializeField] private Button m_SaveButton;

    private TMP_InputField m_InputField;

    private void Awake()
    {
        m_InputField = GetComponent<TMP_InputField>();
        m_InputField.onValueChanged.AddListener(OnValueChanged);

        m_SaveButton.interactable = false;
    }

    private void OnValueChanged(string value)
    {
        string filtered = Filter(value);
        if (filtered != value)
            m_InputField.SetTextWithoutNotify(filtered);

        m_SaveButton.interactable = filtered.Length == m_InputField.characterLimit;
    }

    private string Filter(string input)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (char c in input)
        {
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                sb.Append(char.ToUpper(c));
        }
        return sb.ToString();
    }
}