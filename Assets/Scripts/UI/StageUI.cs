using System.Collections;
using TMPro;
using UnityEngine;

public class StageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_StageText;

    private GameManagerNet m_Manager;

    private IEnumerator Start()
    {
        while (GameManagerNet.Instance == null)
        {
            yield return null;
        }

        m_Manager = GameManagerNet.Instance;
        m_Manager.OnStageChangedEvent += UpdateStage;

        yield return null;
        UpdateStage(m_Manager.CurrentStage);
    }

    private void OnDestroy()
    {
        if (m_Manager != null)
        {
            m_Manager.OnStageChangedEvent -= UpdateStage;
        }
    }

    private void UpdateStage(int stage)
    {
        if (m_StageText != null)
        {
            m_StageText.text = $"Stage: {stage}";
        }
    }
}