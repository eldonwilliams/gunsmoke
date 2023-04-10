using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  The description of how a enemy attacks, with a per hit damage and a hit frequency. DPS = PerHit * HitFrequency
/// DamageDescription is an abstract way to represent damage for basic enemy types.
/// Bosses may have different attack strategies and cannot be represented with a basic PerHit & HitFrequency metric
/// </summary>
[System.Serializable]
public struct DamageDescription
{
    public DamageDescription(float perHit, float hitFrequency)
    {
        PerHit = perHit;
        HitFrequency = hitFrequency;
    }

    /// <summary>
    ///  The damage inflicted per hit
    /// </summary>
    [Tooltip("The damage inflicted per hit")]
    public float PerHit;

    /// <summary>
    ///  How many times a hit will occur per second
    /// </summary>
    [Tooltip("How many times a hit will occur per second")]
    public float HitFrequency;

    /// <summary>
    ///  Calculates the damage per second
    /// </summary>
    /// <returns>The damage per second</returns>
    public float CalcDPS() => PerHit * HitFrequency;
}

/// <summary>
///  A scriptable object that describes the data about a enemy type or instance
/// </summary>
[CreateAssetMenu(fileName = "Enemy", menuName = "Gunsmoke/CreateEnemyType")]
public class EnemyDescription : ScriptableObject
{
    /// <summary>
    ///  The initial health value; max health
    /// </summary>
    [Tooltip("The initial health value; max health")]
    public float Health = 50;

    /// <summary>
    ///  The speed this enemy moves at
    /// </summary>
    [Tooltip("The speed this enemy moves at")]
    public float Speed = 0.75f;

    /// <summary>
    ///  A description for the damage this enemy does, -1 specifies a special attack strategy
    /// </summary>
    [Tooltip("A description for the damage this enemy does, put -1 for special attack strategies")]
    public DamageDescription Damage = new DamageDescription(10, 1);
}
