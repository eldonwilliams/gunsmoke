using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /// <summary>
    ///  The health this enemy has initially
    /// </summary>
    [SerializeField, Tooltip("The health this enemy has initially")]
    private float InitialHealth = 50;
    
    /// <summary>
    ///  The speed this enemy can move
    /// </summary>
    [SerializeField, Tooltip("The speed this enemy can move")]
    private float Speed = 0.5f;

    /// <summary>
    ///  A reference to the Player's transform
    /// </summary>
    [SerializeField, Tooltip("A reference to the Player's transform")]
    private Transform Player;

    /// <summary>
    ///  The amount of health currently possessed
    /// </summary>
    private float health;

    /// <summary>
    ///  A reference to the CharacterController component made at Start
    /// </summary>
    private CharacterController controller;

    void Awake() {
        health = InitialHealth;
    }

    void Start() {
        controller = gameObject.AddComponent<CharacterController>();
        controller.minMoveDistance = 0;
    }

    void Update()
    {
        Vector3 moveDirection = (Player.position - transform.position).normalized;
        controller.Move(moveDirection * Speed * Time.deltaTime);
    }

    /// <summary>
    ///  Gets the current amount of health the Enemy has
    /// </summary>
    /// <returns>Current health</returns>
    public float getHealth() {
        return health;
    }

    /// <summary>
    ///  Damages the enemy by a specified amount, killing the enemy if it is >health
    /// </summary>
    /// <param name="damageAmt">The amount of damage to deal</param>
    public void damage(float damageAmt) {
        health -= damageAmt;
    }
}
