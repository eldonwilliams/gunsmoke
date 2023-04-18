using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenSpacedUIElement : MonoBehaviour
{
    static GameObject _billboardPrefab;
    static Transform _damageIndicatorCanvas;

    public static void SpawnDamageIndicator(Vector3 position, string damageText) {
        if (_billboardPrefab == null)
            _billboardPrefab = Resources.Load<GameObject>("Prefab/DamageIndicator");
        
        if (_damageIndicatorCanvas == null)
            _damageIndicatorCanvas = GameObject.FindWithTag("DamageIndicatorCanvas").transform;

        GameObject billboardInstance = Instantiate(
            _billboardPrefab,
            position,
            Quaternion.identity,
            _damageIndicatorCanvas);
        
        billboardInstance.GetComponent<TMP_Text>().text = damageText;
        billboardInstance.GetComponent<DamageIndicatorScreenSpacedUIElement>().TargetPoint = position;
    }

    [SerializeField, Tooltip("The point this UI Element tracks in world coordinates.")]
    public Vector3 TargetPoint;

    protected Camera _camera;

    protected void Start()
    {
        _camera = Camera.main;
    }

    void LateUpdate()
    {
        transform.position = TargetPoint;

        // transform.LookAt(TargetPoint + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
        transform.LookAt(TargetPoint + _camera.transform.forward, _camera.transform.up);
    }
}
