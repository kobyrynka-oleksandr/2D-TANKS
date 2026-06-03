using System.Collections;
using TMPro;
using UnityEngine;

public class StageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stageText;

    private GameManagerNet _manager;

    private IEnumerator Start()
    {
        while (GameManagerNet.Instance == null)
        {
            yield return null;
        }

        _manager = GameManagerNet.Instance;

        _manager.StageChangedEvent += UpdateStage;

        yield return null;

        UpdateStage(_manager.CurrentStage);
    }

    private void OnDestroy()
    {
        if (_manager != null)
        {
            _manager.StageChangedEvent -= UpdateStage;
        }
    }

    private void UpdateStage(int stage)
    {
        if (_stageText != null)
        {
            _stageText.text = $"Stage: {stage}";
        }
    }
}