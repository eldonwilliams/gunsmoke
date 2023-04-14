using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenSpacedUIElement : MonoBehaviour
{

    public static void SpawnDamageIndicator(Vector3 position, string damageText) {
        ScreenSpaceController controller = UnityUtil.GetRootComponent<ScreenSpaceController>();

        GameObject billboardInstance = Instantiate(
            controller.BillboardPrefab,
            position,
            Quaternion.identity,
            controller.DamageIndicatorCanvas.transform);
        
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
