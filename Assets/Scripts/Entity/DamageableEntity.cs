using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageableEntity : IEntity
{

    /// <summary>
    ///  Is this entity dead
    /// </summary>
    /// <returns>is dead</returns>
    public bool IsDead();

    /// <summary>
    ///  How much health does this entity have
    /// </summary>
    /// <returns>the amount of health</returns>
    public float GetHealth();

    /// <summary>
    ///  How much health can this entity have?
    ///  Note: this is not a hard limit.
    /// </summary>
    /// <returns>Max health</returns>
    public float GetMaxHealth();

    /// <summary>
    ///  Is the given amount of damage able to cause death
    /// </summary>
    /// <param name="amount">the amount of damage</param>
    /// <returns>is deadly</returns>
    public bool IsDamageDeadly(float amount);

    /// <summary>
    ///  Heal the entity for an amount of hp
    /// </summary>
    /// <param name="amount">the amount</param>
    public void Heal(float amount, bool clamp);

    /// <summary>
    ///  Deal the entity some damage
    /// </summary>
    /// <param name="amount">the amount of damage</param>
    public void DealDamage(float amount);
}

/// <summary>
///  A damageable entity is a entity that has health and can receive damage and ultimately die.
/// </summary>
public abstract class DamageableEntity : Entity, IDamageableEntity
{
    /// <summary>
    ///  A private copy of the health this entity has
    /// </summary>
    private float _health;

    /// <summary>
    ///  The amount of health this entity has
    /// </summary>
    protected float Health
    {
        get => _health;
        set {
            _health = value;
            OnHealthUpdate?.Invoke(value);
        }
    }

    /// <summary>
    ///  When entity hurt
    /// </summary>
    /// <param name="damageAmount">The amount of damage taken</param>
    public delegate void HurtEvent(float damageAmount);

    /// <summary>
    ///  When entity health changes at all
    /// </summary>
    /// <param name="newHealth"></param>
    public delegate void HealthUpdateEvent(float newHealth);

    /// <summary>
    ///  When entity dies
    /// </summary>
    public delegate void DeathEvent();

    /// <summary>
    ///  When this entity takes damage
    /// </summary>
    public event HurtEvent OnHurt;

    /// <summary>
    ///  When entity health changes at all
    /// </summary>
    public event HealthUpdateEvent OnHealthUpdate;

    /// <summary>
    ///  When this entity dies
    /// </summary>
    public event DeathEvent OnDeath;

    protected virtual void Awake() {
        Health = getInitialHealth();
    }
    
    /// <summary>
    ///  Gets the initial health value
    /// </summary>
    /// <returns>The initial health value</returns>
    protected virtual float getInitialHealth() {
        return 0;
    }

    public bool IsDead()
    {
        return Health <= 0;
    }

    public float GetHealth()
    {
        return Health;
    }

    public float GetMaxHealth()
    {
        return getInitialHealth();
    }

    public void DealDamage(float amount)
    {
        if (IsDead()) return;
        Health = Mathf.Clamp(Health - amount, 0, float.MaxValue);
        if (IsDead()) {
            OnDeath?.Invoke();
        } else {
            OnHurt?.Invoke(amount);
        }
    }

    public void Heal(float amount, bool clamp = true)
    {
        Health = Mathf.Clamp(Health + amount, 0, clamp ? getInitialHealth() : float.MaxValue);
    }

    public bool IsDamageDeadly(float amount)
    {
        return Health - amount <= 0;
    }
}
