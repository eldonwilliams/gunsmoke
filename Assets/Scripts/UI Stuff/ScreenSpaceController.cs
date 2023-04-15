using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceController : MonoBehaviour
{
    [SerializeField, Tooltip("A reference to the prefab for the billboard")]
    private GameObject _billboardPrefab;

    [SerializeField, Tooltip("A reference to the canvas for damage indicators")]
    private GameObject _damageIndicatorCanvas;

    /// <summary>
    ///  A reference to the prefab for the billboard
    /// </summary>
    public GameObject BillboardPrefab
    { get => _billboardPrefab; }

    /// <summary>
    ///  A reference to the canvas used for rendering damage indicators
    /// </summary>
    public GameObject DamageIndicatorCanvas
    { get => _damageIndicatorCanvas; }
}
