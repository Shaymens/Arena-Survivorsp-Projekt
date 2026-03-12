using UnityEngine;


// Boss — silniejszy wróg, specjalne ataki, więcej HP. Dziedziczy po Enemy.

public class BossEnemy : Enemy
{
    [Header("Boss Settings")]
    public float chargeSpeed = 6f;
    public float chargeCooldown = 5f;
    public float chargeDuration = 0.5f;

    private float lastChargeTime = -999f;
    private bool isCharging = false;
    private float chargeEndTime;

    protected override void Awake()
    {
        base.Awake();
        entityName = "Boss";
        maxHealth = 200f;
        currentHealth = maxHealth;
        attackDamage = 20f;
        moveSpeed = 1.5f;
        scoreValue = 100;
        detectionRange = 12f;
    }

    protected override void Update()
    {
        if (playerTransform == null) return;

        if (isCharging)
        {
            if (Time.time >= chargeEndTime)
            {
                isCharging = false;
                moveSpeed = 1.5f;
            }
            return;
        }

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (dist <= detectionRange && Time.time >= lastChargeTime + chargeCooldown)
        {
            StartCharge();
            return;
        }

        base.Update();

        // Flip sprite w zależności od kierunku
        Vector2 dir = (playerTransform.position - transform.position).normalized;
        Transform spriteTransform = transform.Find("Sprite");
        if (spriteTransform != null)
        {
            if (dir.x < 0)
                spriteTransform.localScale = new Vector3(1, 1, 1);   // patrzy w prawo
            else if (dir.x > 0)
                spriteTransform.localScale = new Vector3(-1, 1, 1);  // patrzy w lewo
        }
    }

    private void StartCharge()
    {
        Debug.Log("[Boss] Szarżuje!");
        isCharging = true;
        lastChargeTime = Time.time;
        chargeEndTime = Time.time + chargeDuration;
        moveSpeed = chargeSpeed;

        if (playerTransform != null)
        {
            Vector2 dir = (playerTransform.position - transform.position).normalized;
            Move(dir);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null && CanAttack())
        {
            lastAttackTime = Time.time;
            player.TakeDamage(attackDamage);
            Debug.Log($"[Boss] Zadano {attackDamage} obrażeń graczowi!");
        }
    }

    protected override void OnDeath()
    {
        Debug.Log($"[Enemy] {entityName} pokonany!");
        Player player = FindFirstObjectByType<Player>();
        player?.AddScore(scoreValue);

        // Poinformuj GameManager że wróg zginął
        GameManager.Instance?.OnEnemyDefeated();

        EnemySpawner spawner = GetComponent<EnemySpawner>();
        if (spawner != null)
            spawner.TryDropPickup(transform.position);

        Destroy(gameObject, 0.1f);
    }
}
