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
    ///  The amount of health this entity has
    /// </summary>
    protected float _health;

    /// <summary>
    ///  When entity hurt
    /// </summary>
    /// <param name="damageAmount">The amount of damage taken</param>
    public delegate void HurtEvent(float damageAmount);

    /// <summary>
    ///  When entity dies
    /// </summary>
    public delegate void DeathEvent();

    /// <summary>
    ///  When this entity takes damage
    /// </summary>
    public event HurtEvent OnHurt;

    /// <summary>
    ///  When this entity dies
    /// </summary>
    public event DeathEvent OnDeath;

    protected virtual void Awake() {
        _health = getInitialHealth();
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
        return _health <= 0;
    }

    public float GetHealth()
    {
        return _health;
    }

    public void DealDamage(float amount)
    {
        if (IsDead()) return;
        _health = Mathf.Clamp(_health - amount, 0, float.MaxValue);
        if (IsDead()) {
            OnDeath?.Invoke();
        } else {
            OnHurt?.Invoke(amount);
        }
    }

    public void Heal(float amount, bool clamp = true)
    {
        _health = Mathf.Clamp(_health + amount, 0, clamp ? getInitialHealth() : float.MaxValue);
    }

    public bool IsDamageDeadly(float amount)
    {
        return _health - amount <= 0;
    }
}
