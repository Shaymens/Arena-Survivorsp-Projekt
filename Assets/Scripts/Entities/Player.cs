using UnityEngine;

/// <summary>
/// Gracz — sterowany przez użytkownika, zbiera punkty i przedmioty.
/// </summary>
public class Player : Character
{
    [Header("Player Settings")]
    public int score = 0;
    public string playerName = "Player1";
    [Header("Effects")]
    public GameObject damageNumberPrefab;
    private Camera mainCamera;
    [Header("Level System")]
    public int currentLevel = 1;
    public int experiencePoints = 0;
    public int experienceToNextLevel = 100;

    protected override void Awake()
    {
        base.Awake();
        entityName = "Player";
        mainCamera = Camera.main;
        // Pobierz Animator z obiektu Sprite
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleMovement();
        HandleAttackInput();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 direction = new Vector2(x, y);
        Move(direction);

        if (animator != null)
        {
            bool isMoving = direction.magnitude > 0;
            animator.SetBool("isMoving", isMoving);

            if (isMoving)
            {
                animator.SetFloat("moveX", x);
                animator.SetFloat("moveY", y);
            }
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && CanAttack())
        {
            lastAttackTime = Time.time;

            // Sprawdź kierunek myszy
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            bool mouseOnRight = mousePos.x > transform.position.x;

            // Ustaw odpowiednią animację
            animator.SetBool("attackRight", mouseOnRight);
            animator.SetTrigger("attack");

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach (var hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                    ShowDamageNumber(enemy.transform.position, attackDamage);
                    Debug.Log($"[Player] Uderzono {enemy.entityName}!");
                }
            }

            StartCoroutine(ShowAttackEffect());
        }
    }

    private System.Collections.IEnumerator ShowAttackEffect()
    {
        GameObject effect = new GameObject("AttackEffect");
        effect.transform.position = transform.position;
        SpriteRenderer sr = effect.AddComponent<SpriteRenderer>();
        sr.sprite = GetComponentInChildren<SpriteRenderer>().sprite;
        sr.color = new Color(1f, 1f, 0f, 0.5f);
        effect.transform.localScale = new Vector3(attackRange * 2, attackRange * 2, 1);

        yield return new WaitForSeconds(0.1f);
        Destroy(effect);
    }

    public void AddScore(int points)
    {
        score += points;
        AddExperience(points);
        UIManager.Instance?.UpdateScoreUI(score);
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);
        UIManager.Instance?.UpdateHealthUI(currentHealth, maxHealth);
    }

    protected override void OnDamageTaken(float amount)
    {
        Debug.Log($"[Player] Otrzymano {amount} obrażeń. HP: {currentHealth}/{maxHealth}");
        UIManager.Instance?.UpdateHealthUI(currentHealth, maxHealth);
    }

    protected override void OnDeath()
    {
        Debug.Log("[Player] OnDeath wywołane!");
        DataManager.Instance?.SaveScore(new ScoreEntry(playerName, score));
        GameManager.Instance?.GameOver();
    }

    protected override void OnAttack(Entity target)
    {
        Debug.Log($"[Player] Zaatakowano: {target.entityName}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void ShowDamageNumber(Vector3 position, float damage)
    {
        if (damageNumberPrefab == null) return;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(position + Vector3.up);
        GameObject obj = Instantiate(damageNumberPrefab, screenPos, Quaternion.identity);
        obj.transform.SetParent(FindFirstObjectByType<Canvas>().transform, true);
        obj.GetComponent<DamageNumber>()?.Setup(damage);
    }

    public void AddExperience(int amount)
    {
        experiencePoints += amount;
        UIManager.Instance?.UpdateExpUI(experiencePoints, experienceToNextLevel, currentLevel);

        if (experiencePoints >= experienceToNextLevel)
            LevelUp();
    }

    private void LevelUp()
    {
        currentLevel++;
        experiencePoints = 0;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);

        // Ulepszenia statystyk
        maxHealth += 10f;
        currentHealth = maxHealth; // pełne HP po levelupie
        attackDamage += 5f;
        moveSpeed += 0.5f;

        Debug.Log($"[Player] Level UP! Poziom: {currentLevel}");
        UIManager.Instance?.ShowLevelUp(currentLevel);
        UIManager.Instance?.UpdateHealthUI(currentHealth, maxHealth);
    }

}
