using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class NameInputValidator : MonoBehaviour
{
    [SerializeField] private Button _saveButton;

    private TMP_InputField _inputField;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();

        _inputField.onValueChanged.AddListener(OnValueChanged);

        _saveButton.interactable = false;
    }

    private void OnDestroy()
    {
        if (_inputField != null)
        {
            _inputField.onValueChanged.RemoveListener(OnValueChanged);
        }
    }

    private void OnValueChanged(string value)
    {
        string filtered = Filter(value);

        if (filtered != value)
        {
            _inputField.SetTextWithoutNotify(filtered);
        }

        _saveButton.interactable =
            filtered.Length ==
            _inputField.characterLimit;
    }

    private string Filter(string input)
    {
        StringBuilder builder = new();

        foreach (char character in input)
        {
            bool isLetter =
                (character >= 'A' && character <= 'Z')
                || (character >= 'a' && character <= 'z');

            if (isLetter)
            {
                builder.Append(char.ToUpper(character));
            }
        }

        return builder.ToString();
    }
}