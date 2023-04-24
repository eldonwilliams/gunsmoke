using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgradeUIRenderer : MonoBehaviour
{
    /// <summary>
    ///  A list of all the player upgrades to be rendered
    /// </summary>
    private BasicPlayerUpgrade[] _playerUpgrades;

    /// <summary>
    ///  UI Line prefab
    /// </summary>
    private GameObject _uiline;

    void Start() {
        _playerUpgrades = BasicPlayerUpgrade.GetBasicPlayerUpgrades();
    }

    /// <summary>
    ///  Adds a line renderer that tracks the position of both game objects until either is destroyed
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    private void connectTwoUpgrades(GameObject a, GameObject b) {

    }
}
