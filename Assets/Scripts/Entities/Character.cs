using UnityEngine;


// Klasa pośrednia dla postaci (gracze i wrogowie) — rozszerza Entity o ruch i atak.

public abstract class Character : Entity
{
    [Header("Character Stats")]
    public float moveSpeed = 3f;
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    protected float lastAttackTime;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Move(Vector2 direction)
    {
        if (rb != null)
            rb.linearVelocity = direction.normalized * moveSpeed;
    }

    public virtual bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public virtual void Attack(Entity target)
    {
        if (!CanAttack()) return;
        lastAttackTime = Time.time;
        float distance = Vector2.Distance(transform.position, target.transform.position);
        if (distance <= attackRange)
        {
            target.TakeDamage(attackDamage);
            OnAttack(target);
        }
    }

    protected virtual void OnAttack(Entity target) { }
}
