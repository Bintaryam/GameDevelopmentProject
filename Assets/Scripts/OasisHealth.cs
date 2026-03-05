using UnityEngine;
using UnityEngine.Events;

public class OasisHealth : MonoBehaviour
{
    public float maxHealth = 100f;

    public UnityEvent OnOasisDied;
    public UnityEvent<float, float> OnHealthChanged;

    private float currentHealth;
    private bool isDead;

    void Start()
    {
        ResetHealth();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"[OasisHealth] Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            isDead = true;
            OnOasisDied?.Invoke();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
