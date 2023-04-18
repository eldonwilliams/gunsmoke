using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeftHandHUDRender : MonoBehaviour
{
    /// <summary>
    ///  A reference to the single instance of LeftHandHUDRender that should exist
    /// </summary>
    static LeftHandHUDRender Instance;

    /// <summary>
    ///  Get the instance of LeftHandHUDRender
    /// </summary>
    /// <returns></returns>
    public static LeftHandHUDRender GetInstance()
    {
        if (Instance != null) return Instance;
        Instance = UnityUtil.GetRootComponent<LeftHandHUDRender>();
        return Instance;
    }

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

    private SaveData _save;

    /// <summary>
    ///  Is the HUD shown?
    /// </summary>
    public bool Hidden
    {
        get => _hidden;
        set
        {
            if (value == true) Hide();
            else Show();
            _hidden = value;
        }
    }

    private bool _hidden = false;

    void Start()
    {
        _player = EntityPlayer.GetPlayer();
        _save = SaveDataManager.GetSaveData();

        UpdateDisplays();
        _player.OnHealthUpdate += (float _) => UpdateDisplays();
    }

    public void Hide()
    {
        LeanTween.moveX(GetComponent<RectTransform>(), -GetComponent<RectTransform>().rect.width, 0.75f).setEaseOutQuad();
    }

    public void Show()
    {
        LeanTween.moveX(GetComponent<RectTransform>(), 0, 0.75f).setEaseOutQuad();
    }

    private void UpdateDisplays()
    {
        _healthDisplay.GetComponentInChildren<TMP_Text>().text = _player.GetHealth().ToString();
        _healthDisplay.Find("Fill").LeanScaleX(
            _player.GetHealth() / _player.GetMaxHealth(),
            0.2f).setEaseOutQuad();
        // TODO: Add damageDisplay to UpdateDisplays method
        _damageDisplay.GetComponentInChildren<TMP_Text>().text = _save.alienDNA.ToString();
    }
}
