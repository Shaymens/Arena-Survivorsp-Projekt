using UnityEngine;


// Klasa bazowa dla wszystkich bytów w grze (gracze, wrogowie, przedmioty).

public abstract class Entity : MonoBehaviour
{
    [Header("Entity Base Stats")]
    public string entityName = "Entity";
    public float maxHealth = 100f;
    protected float currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public float CurrentHealth => currentHealth;
    public bool IsAlive => currentHealth > 0;

    private bool isDead = false;

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnDamageTaken(amount);

        if (!IsAlive)
        {
            isDead = true;
            OnDeath();
        }
    }

    public virtual void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    protected virtual void OnDamageTaken(float amount) { }
    protected abstract void OnDeath();


}
