using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /// <summary>
    ///  The EnemyDescription applied to this enemy
    /// </summary>
    [SerializeField, Tooltip("The EnemyDescription applied to this enemy")]
    private EnemyDescription Description;

    /// <summary>
    ///  The color that is flashed when this enemy takes damage
    /// </summary>
    [SerializeField, Tooltip("The color that is flashed when this enemy takes damage")]
    private Color DamageColor = Color.red;

    /// <summary>
    ///  A reference to the Player's transform
    /// </summary>
    [SerializeField, Tooltip("A reference to the Player's transform")]
    private Transform Player;

    /// <summary>
    ///  The length of the damage and death animations
    /// </summary>
    [SerializeField, Tooltip("The length of the damage and death animations")]
    private float AnimationLength = 0.25f;

    /// <summary>
    ///  The amount of health currently possessed
    /// </summary>
    private float _health;

    /// <summary>
    ///  Is this enemy dead?
    /// </summary>
    private bool _dead = false;

    /// <summary>
    ///  The initial color of the enemy controller
    /// </summary>
    private Color _initialColor;

    /// <summary>
    ///  This enemies copy of the default material
    /// </summary>
    private Material _material;

    /// <summary>
    ///  A reference to the CharacterController component made at Start
    /// </summary>
    private CharacterController controller;

    void Awake() {
        _health = Description.Health;
    }

    void Start() {
        controller = gameObject.AddComponent<CharacterController>();
        controller.minMoveDistance = 0;

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        Material defaultMaterial = renderer.material;
        _material = new Material(defaultMaterial);
        renderer.material = _material;
        _initialColor = _material.color;
    }

    void Update()
    {
        if (_dead) return;
        Vector3 moveDirection = (Player.position - transform.position).normalized;
        controller.Move(moveDirection * Description.Speed * Time.deltaTime);
    }

    /// <summary>
    ///  Gets the current amount of health the Enemy has
    /// </summary>
    /// <returns>Current health</returns>
    public float getHealth() {
        return _health;
    }

    /// <summary>
    ///  Is this enemy dead
    /// </summary>
    /// <returns>is it?</returns>
    public bool isDead() {
        return _dead;
    }

    private IEnumerator _damageColorEnumerator;

    /// <summary>
    ///  Damages the enemy by a specified amount, killing the enemy if it is >health
    /// </summary>
    /// <param name="damageAmt">The amount of damage to deal</param>
    public void damage(float damageAmt) {
        if (_dead) return;
        _health -= damageAmt;
        if (_damageColorEnumerator != null)
            StopCoroutine(_damageColorEnumerator);
        if (_health <= 0) {
            _dead = true;
            _health = 0;
            StartCoroutine(deathShrinkEffect());
        } else {
            _damageColorEnumerator = damageColorEffect();
            StartCoroutine(_damageColorEnumerator);
        }
    }

    /// <summary>
    ///  Manages the color red flash when damaged
    /// </summary>
    /// <returns>The corourtine ig, im half asleep man</returns>
    private IEnumerator damageColorEffect() {
        float timeRemaining = AnimationLength / 2;
        _material.color = _initialColor;

        while (timeRemaining > 0) {
            _material.color = Color.Lerp(_initialColor, DamageColor, 1 - (timeRemaining / AnimationLength / 2));
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        _material.color = DamageColor;

        timeRemaining = AnimationLength / 2;

        while (timeRemaining > 0) {
            _material.color = Color.Lerp(DamageColor, _initialColor, 1 - (timeRemaining / AnimationLength / 2));
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        _material.color = _initialColor;
    }

    /// <summary>
    ///  Manages the shrink effect when an enemy dies
    /// </summary>
    /// <returns></returns>
    private IEnumerator deathShrinkEffect() {
        float timeRemaining = AnimationLength;

        while (timeRemaining > 0) {
            //transform.localPosition = Vector3.Lerp(Vector3.one, new Vector3(1, 0, 1), 1 - (timeRemaining / time));
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, 1 - (timeRemaining / AnimationLength));
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Destroy(_material, AnimationLength);
        Destroy(transform.gameObject, AnimationLength);
    }
}
