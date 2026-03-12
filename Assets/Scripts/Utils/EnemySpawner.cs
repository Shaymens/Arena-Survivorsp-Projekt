using UnityEngine;


// Pomocniczy komponent do spawnu przedmiotów (Pickup) po śmierci wrogów.
// Dodaj do prefabu Enemy/BossEnemy.

public class EnemySpawner : MonoBehaviour
{
    public GameObject pickupPrefab;
    [Range(0f, 1f)]
    public float dropChance = 0.3f;

    // Wywołaj po śmierci wroga, aby losowo upuścić przedmiot.
    public void TryDropPickup(Vector3 position)
    {
        if (pickupPrefab == null) return;
        if (Random.value <= dropChance)
        {
            Instantiate(pickupPrefab, position, Quaternion.identity);
            Debug.Log("[EnemySpawner] Upuszczono przedmiot!");
        }
    }
}
