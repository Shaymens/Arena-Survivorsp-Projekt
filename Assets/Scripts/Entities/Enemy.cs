using UnityEngine;


// Wróg — porusza się w stronę gracza i atakuje. Prosta AI.
 
public class Enemy : Character
{
    [Header("Enemy Settings")]
    public int scoreValue = 10;
    public float detectionRange = 8f;

    protected Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();
        entityName = "Enemy";
        maxHealth = 50f;
        currentHealth = maxHealth;
        attackDamage = 8f;
        moveSpeed = 2f;

        // Wyłącz culling — wróg zawsze aktywny
        if (GetComponentInChildren<SpriteRenderer>() != null)
            GetComponentInChildren<SpriteRenderer>().forceRenderingOff = false;
    }

    protected virtual void Start()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
            playerTransform = player.transform;
    }

    protected virtual void Update()
    {
        if (playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (dist <= detectionRange)
        {
            // Poruszaj się w stronę gracza
            Vector2 dir = (playerTransform.position - transform.position).normalized;
            Move(dir);

            // Flip sprite w zależności od kierunku
            Transform spriteTransform = transform.Find("Sprite");
            if (spriteTransform != null)
            {
                if (dir.x < 0)
                    spriteTransform.localScale = new Vector3(1, 1, 1);   // patrzy w prawo
                else if (dir.x > 0)
                    spriteTransform.localScale = new Vector3(-1, 1, 1);  // patrzy w lewo
            }
        }
        else
        {
            Move(Vector2.zero);
        }
    }

    protected override void OnDamageTaken(float amount)
    {
        Debug.Log($"[Enemy] {entityName} otrzymał {amount} obrażeń. HP: {currentHealth}/{maxHealth}");
    }

    protected override void OnDeath()
    {
        Debug.Log($"[Enemy] {entityName} pokonany!");
        Debug.Log($"[Enemy] GameManager istnieje: {GameManager.Instance != null}");

        Player player = FindFirstObjectByType<Player>();
        player?.AddScore(scoreValue);

        GameManager.Instance?.OnEnemyDefeated();

        EnemySpawner spawner = GetComponent<EnemySpawner>();
        if (spawner != null)
            spawner.TryDropPickup(transform.position);

        Destroy(gameObject, 0.1f);
    }

    protected override void OnAttack(Entity target)
    {
        Debug.Log($"[Enemy] {entityName} atakuje {target.entityName}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null && CanAttack())
        {
            Attack(player);
        }
    }

    private void OnBecameInvisible()
    {
        // Nie usypiaj wroga gdy jest poza ekranem
        enabled = true;
    }
}
