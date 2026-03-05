using UnityEngine;

public class GameManager : MonoBehaviour
{
    public OasisHealth oasisHealth;
    public WaveManager waveManager;

    private bool gameOver;

    void Start()
    {
        if (oasisHealth != null)
            oasisHealth.OnOasisDied.AddListener(LoseGame);

        if (waveManager != null)
            waveManager.OnAllWavesComplete.AddListener(WinGame);

        Debug.Log("[GameManager] Game started");
    }

    private void LoseGame()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("[GameManager] LOSE — Oasis destroyed!");
    }

    private void WinGame()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("[GameManager] WIN — All waves defeated!");
    }
}
