using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeftHandHUDRender : MonoBehaviour
{
    /// <summary>
    ///  A reference to the health text display
    /// </summary> 
    [SerializeField, Tooltip("A reference to the health text display")]
    private Transform _healthDisplay;

    /// <summary>
    ///  A reference to the damage text display
    /// </summary>
    [SerializeField, Tooltip("A reference to the damage text display")]
    private Transform _damageDisplay;

    /// <summary>
    ///  A reference to the main player
    /// </summary>
    private EntityPlayer _player;
    
    void Start()
    {
        _player = EntityPlayer.GetPlayer();

        UpdateDisplays();
        _player.OnHealthUpdate += (float _) => UpdateDisplays();
    }

    private void UpdateDisplays() {
        _healthDisplay.GetComponentInChildren<TMP_Text>().text = _player.GetHealth().ToString();
        _healthDisplay.Find("Fill").LeanScaleX(
            _player.GetHealth() / _player.GetMaxHealth(),
            0.2f).setEaseOutQuad();
        // TODO: Add damageDisplay to UpdateDisplays method
        _damageDisplay.GetComponentInChildren<TMP_Text>().text = "10";
    }
}
