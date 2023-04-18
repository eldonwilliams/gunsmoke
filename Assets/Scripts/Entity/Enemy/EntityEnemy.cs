using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  An entity that represents an entity
/// </summary>
public class EntityEnemy : DamageableEntity
{
    /// <summary>
    ///  The EnemyDescription object for this enemy
    /// </summary>
    [SerializeField, Tooltip("The EnemyDescription object for this enemy")]
    private EnemyDescription Description;

    /// <summary>
    ///  The color that is flashed when this enemy takes damage
    /// </summary>
    [SerializeField, Tooltip("The color that is flashed when this enemy takes damage")]
    private Color DamageColor = Color.red;

    /// <summary>
    ///  The length of the damage and death animations
    /// </summary>
    [SerializeField, Tooltip("The length of the damage and death animations")]
    private float AnimationLength = 0.25f;

    /// <summary>
    ///  Modifier for gravity
    /// </summary>
    [SerializeField, Tooltip("The modifier for gravity")]
    private float GravityModifier = -9.81f;

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

    /// <summary>
    ///  Reference to the player's transform
    /// </summary>
    private Transform Player;

    /// <summary>
    ///  The enumerator handling damage color right now
    /// </summary>
    private IEnumerator _damageColorEnumerator;

    /// <summary>
    ///  The time of the last successful attack
    ///  Used for debounce
    /// </summary>
    private float _lastAttack = 0;

    /// <summary>
    ///  the max distance the enemy can attack
    /// </summary>
    private float _attackRange = 1.5f;

    /// <summary>
    ///  A reference to the SaveData
    /// </summary>
    private SaveData _save;

    void Start()
    {   
        // Make controller, minMoveDistance to mix grounded bug
        controller = gameObject.AddComponent<CharacterController>();
        controller.minMoveDistance = 0;

        /*
            Make a new material for this entity, for the color effect
        */
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        Material defaultMaterial = renderer.material;
        _material = new Material(defaultMaterial);
        renderer.material = _material;
        _initialColor = _material.color;

        Player = EntityPlayer.GetCharacter();

        _save = SaveDataManager.GetSaveData();

        /*
            Setup events for the effects
        */
        OnHurt += (_damage) =>
        {
            if (_damageColorEnumerator != null)
                StopCoroutine(_damageColorEnumerator);
            _damageColorEnumerator = damageColorEffect();
            StartCoroutine(_damageColorEnumerator);
        };

        OnDeath += () => StartCoroutine(deathShrinkEffect());
        OnDeath += () => {
            _save.alienDNA++;
        };
    }

    void Update()
    {
        if (IsDead()) return;
        Vector3 moveDirection = (Player.position - transform.position).normalized;
        controller.Move(moveDirection * Description.Speed * Time.deltaTime);
        controller.Move(new Vector3(0, GravityModifier * Time.deltaTime));

        if (_lastAttack + 1 / Description.Damage.HitFrequency <= Time.time) TryAttack();
    }

    private void TryAttack() {
        if (Vector3.Distance(transform.position, Player.position) > _attackRange) return;
        _lastAttack = Time.time;
        EntityPlayer.GetPlayer().DealDamage(Description.Damage.PerHit);
    }

    protected override float getInitialHealth()
    {
        return Description.Health;
    }

    /// <summary>
    ///  Manages the color red flash when damaged
    /// </summary>
    /// <returns>The corourtine ig, im half asleep man</returns>
    private IEnumerator damageColorEffect()
    {
        float timeRemaining = AnimationLength / 2;
        _material.color = _initialColor;

        while (timeRemaining > 0)
        {
            _material.color = Color.Lerp(_initialColor, DamageColor, 1 - (timeRemaining / AnimationLength / 2));
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        _material.color = DamageColor;

        timeRemaining = AnimationLength / 2;

        while (timeRemaining > 0)
        {
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
    private IEnumerator deathShrinkEffect()
    {
        float timeRemaining = AnimationLength;

        while (timeRemaining > 0)
        {
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
