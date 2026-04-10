using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        TargetHealth player = GameObject.FindWithTag("Player")?.GetComponent<TargetHealth>();
        TargetHealth flag = GameObject.FindWithTag("Flag")?.GetComponent<TargetHealth>();

        if (player) player.OnDeath += OnPlayerDied;
        if (flag) flag.OnDeath += OnFlagDestroyed;
    }

    private void OnPlayerDied()
    {
        Debug.Log("Game Over — гравець загинув");
    }

    private void OnFlagDestroyed()
    {
        Debug.Log("Game Over — флаг знищено");
    }
}