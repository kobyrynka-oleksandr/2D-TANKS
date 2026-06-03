using UnityEngine;

public class BonusUIManager : MonoBehaviour
{
    public static BonusUIManager Instance { get; private set; }

    [SerializeField] private GameObject _healIcon;
    [SerializeField] private GameObject _speedIcon;
    [SerializeField] private GameObject _doubleDamageIcon;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        HideAll();
    }

    public void ShowHeal()
    {
        SetIconState(_healIcon, true);
    }

    public void HideHeal()
    {
        SetIconState(_healIcon, false);
    }

    public void ShowSpeed()
    {
        SetIconState(_speedIcon, true);
    }

    public void HideSpeed()
    {
        SetIconState(_speedIcon, false);
    }

    public void ShowDoubleDamage()
    {
        SetIconState(_doubleDamageIcon, true);
    }

    public void HideDoubleDamage()
    {
        SetIconState(_doubleDamageIcon, false);
    }

    public void HideAll()
    {
        HideHeal();
        HideSpeed();
        HideDoubleDamage();
    }

    private void InitializeSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void SetIconState(GameObject icon, bool isActive)
    {
        if (icon != null)
        {
            icon.SetActive(isActive);
        }
    }
}