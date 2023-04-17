using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

/// <summary>
///  The type of a player upgrade reward
/// </summary>
public enum PlayerUpgradeRewardType
{
    /// <summary>
    ///  Increases the max amount of health
    /// </summary>
    MAX_HEALTH,
    /// <summary>
    ///  Increases damage
    /// </summary>
    DAMAGE,
    /// <summary>
    ///  Increases amount of enemies pierced with bullet
    /// </summary>
    ENEMY_PIERCE,
}

/// <summary>
///  A player upgrade reward that has a integer value 
/// </summary>
[System.Serializable]
public struct PlayerUpgradeReward
{
    /// <summary>
    ///  The type of reward this is
    /// </summary>
    [Tooltip("The type of reward this is")]
    public PlayerUpgradeRewardType type;
    /// <summary>
    ///  The amount of reward this provides
    /// </summary>
    [Tooltip("The amount of reward this provides")]
    public float amount;
}

[CreateAssetMenu(fileName = "Upgrade", menuName = "Gunsmoke/CreateBasicPlayerUpgrade")]
public class BasicPlayerUpgrade : ScriptableObject
{
    static BasicPlayerUpgrade[] BasicPlayerUpgrades;

    static public BasicPlayerUpgrade[] GetBasicPlayerUpgrades() {
        
    }

    /// <summary>
    ///  The rewards this upgrade provides
    /// </summary>
    public PlayerUpgradeReward[] Rewards;

    /// <summary>
    ///  How much this upgrade costs
    /// </summary>
    public int Price;

    /// <summary>
    ///  What Upgrades is this on dependent on?
    /// </summary>
    public List<BasicPlayerUpgrade> Requirements
    {
        get => _requirements;
        private set
        {
            foreach (BasicPlayerUpgrade item in _requirements) item.Dependents.Remove(this);
            foreach (BasicPlayerUpgrade item in value) item.Dependents.Add(this);
            _requirements = value;
        }
    }

    /// <summary>
    ///  What upgrades does this unlock?
    /// </summary>
    //[System.NonSerialized
    public List<BasicPlayerUpgrade> Dependents = new List<BasicPlayerUpgrade>();

    /// <summary>
    ///  The image that will render for this upgrade
    /// </summary>
    public Sprite Image;

    /// <summary>
    ///  The name of this upgrade
    /// </summary>
    public string Name;

    /// <summary>
    ///  Private copy of requirements
    /// </summary>
    [SerializeField]
    private List<BasicPlayerUpgrade> _requirements = new List<BasicPlayerUpgrade>();

    public bool isUnlocked()
    {
        return false;
    }

    public bool canPurchase()
    {
        if (isUnlocked()) return false;
        foreach (BasicPlayerUpgrade cur in Requirements)
        {
            if (!cur.isUnlocked()) return false;
        }
        return true;
    }

#if UNITY_EDITOR
    public void updateDependents(List<BasicPlayerUpgrade> _for)
    {
        if (_for == Requirements) return;

        foreach (BasicPlayerUpgrade item in _requirements)
            if (item != null)
                item.Dependents.Remove(this);

        foreach (BasicPlayerUpgrade item in _for) item.Dependents.Add(this);
    }
#endif
}

[CustomEditor(typeof(BasicPlayerUpgrade))]
public class BasicPlayerUpgradeEditor : Editor
{
    SerializedProperty rewards;
    SerializedProperty price;
    SerializedProperty requirements;
    SerializedProperty dependents;
    SerializedProperty image;
    SerializedProperty nameProperty;
    BasicPlayerUpgrade targetedUpgrade;

    void OnEnable()
    {
        rewards = serializedObject.FindProperty("Rewards");
        price = serializedObject.FindProperty("Price");
        requirements = serializedObject.FindProperty("_requirements");
        dependents = serializedObject.FindProperty("Dependents");
        image = serializedObject.FindProperty("Image");
        nameProperty = serializedObject.FindProperty("Name");
        targetedUpgrade = (BasicPlayerUpgrade)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Information Config");

        EditorGUILayout.PropertyField(rewards, new GUIContent("Rewards", "The benefit the upgrade provides"));
        EditorGUILayout.PropertyField(price, new GUIContent("Price", "How much the upgrade costs"));

        EditorGUILayout.LabelField("Prerequisite Config");

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(requirements, new GUIContent("Requirements", "The required upgrades for this one to be purchaseable"));
        if (EditorGUI.EndChangeCheck())
        {
            List<BasicPlayerUpgrade> serializedRequirementsValue = new List<BasicPlayerUpgrade>();
            for (int i = 0; i < requirements.arraySize; i++)
            {
                var current = (BasicPlayerUpgrade)requirements.GetArrayElementAtIndex(i).objectReferenceValue;
                if (current != null) serializedRequirementsValue.Add(current);
            }
            targetedUpgrade.updateDependents(serializedRequirementsValue);
        }

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(dependents, new GUIContent("Dependents", "The upgrades that require this one to be unlocked"));
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("UI Config");

        EditorGUILayout.PropertyField(image, new GUIContent("Image", "The image associated with the upgrade"));
        EditorGUILayout.PropertyField(nameProperty, new GUIContent("Name", "The name of the upgrade, as it shows up in the purchase ui"));

        serializedObject.ApplyModifiedProperties();
    }
}