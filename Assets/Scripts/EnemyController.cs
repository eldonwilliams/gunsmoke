using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float Speed = 0.5f;
    public float InitialHealth = 50.0f;

    [SerializeField]
    private Transform Player;
    private float _health;
    private CharacterController controller;

    void Awake() {
        _health = InitialHealth;
    }

    void Start() {
        controller = gameObject.AddComponent<CharacterController>();
        controller.minMoveDistance = 0;
    }

    void Update()
    {
        Vector3 moveDirection = Vector3.Lerp(transform.position, Player.position, Speed * Time.deltaTime);
        moveDirection.Normalize();
        Debug.Log(moveDirection);
        controller.Move(moveDirection);
    }

    float getHealth() {
        return _health;
    }

    void damage(float damageAmt) {
        _health -= damageAmt;
    }
}
