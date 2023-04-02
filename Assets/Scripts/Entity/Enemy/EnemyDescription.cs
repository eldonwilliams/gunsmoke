using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    ///  The damage this enemy can deal
    /// </summary>
    [Tooltip("The damage this enemy can deal")]
    public float Damage = 10;
}
