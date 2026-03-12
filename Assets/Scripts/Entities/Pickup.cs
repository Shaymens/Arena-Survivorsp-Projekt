using UnityEngine;


// Przedmiot do podniesienia — apteczka lub bonus punktowy. Dziedziczy po Entity.

public class Pickup : Entity
{
    public enum PickupType { Health, ScoreBonus }

    [Header("Pickup Settings")]
    public PickupType pickupType = PickupType.Health;
    public float healAmount = 30f;
    public int bonusScore = 25;

    protected override void Awake()
    {
        base.Awake();
        entityName = "Pickup";
        maxHealth = 1f;
        currentHealth = 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) return;

        switch (pickupType)
        {
            case PickupType.Health:
                player.Heal(healAmount);
                Debug.Log($"[Pickup] Gracz uleczony o {healAmount} HP");
                break;
            case PickupType.ScoreBonus:
                player.AddScore(bonusScore);
                Debug.Log($"[Pickup] Gracz otrzymał {bonusScore} punktów bonusowych");
                break;
        }

        Destroy(gameObject);
    }

    // Pickup nie może być atakowany — ignorujemy obrażenia
    public override void TakeDamage(float amount) { }

    protected override void OnDeath() { }
}
