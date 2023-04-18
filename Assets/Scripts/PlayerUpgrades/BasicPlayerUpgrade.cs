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

    static public BasicPlayerUpgrade[] GetBasicPlayerUpgrades()
    {
        return Resources.LoadAll<BasicPlayerUpgrade>("Config/PlayerUpgrades");
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
    ///  What upgrades are required for this one to become purchasable
    /// </summary>
    public List<BasicPlayerUpgrade> Requirements
    {
        get => _requirements;
        private set
        {
            reconcileDependencies(value);
            _requirements = value;
        }
    }

    /// <summary>
    ///  What upgrades does this unlock?
    /// </summary>
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

    /// <summary>
    ///  Is the purchase unlocked
    /// </summary>
    /// <returns></returns>
    public bool isUnlocked()
    {
        return false;
    }

    /// <summary>
    ///  Can this upgrade be purchased?
    /// </summary>
    /// <returns></returns>
    public bool canPurchase()
    {
        if (isUnlocked()) return false;
        foreach (BasicPlayerUpgrade cur in Requirements)
        {
            if (!cur.isUnlocked()) return false;
        }
        return true;
    }

    /// <summary>
    ///  Updates the requirement and dependents list when requirements change,
    /// </summary>
    /// <param name="newDependencies">either the current requirements or a new set (if setter)</param>
    public void reconcileDependencies(List<BasicPlayerUpgrade> newDependencies)
    {
        // If no change, don't calculate changes
        if (newDependencies == Requirements) return;
        
        // Remove ourself as a dependent from all requirements
        // null-check first
        foreach (BasicPlayerUpgrade item in _requirements) 
            if (item != null)
                item.Dependents.Remove(this);

        // Add back with the new dependencies
        foreach (BasicPlayerUpgrade item in newDependencies) {
            if (item == this)
                return;
            item.Dependents.Add(this);
        }
    }
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

        // If requirements change, we must reconcile the dependents variable for the other upgrades
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(requirements, new GUIContent("Requirements", "The required upgrades for this one to be purchaseable"));
        if (EditorGUI.EndChangeCheck())
            targetedUpgrade.reconcileDependencies(getSerializedList<BasicPlayerUpgrade>(requirements));

        // Disable dependents so it is not changed, it should be read only
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(dependents, new GUIContent("Dependents", "The upgrades that require this one to be unlocked"));
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.LabelField("UI Config");

        EditorGUILayout.PropertyField(image, new GUIContent("Image", "The image associated with the upgrade"));
        EditorGUILayout.PropertyField(nameProperty, new GUIContent("Name", "The name of the upgrade, as it shows up in the purchase ui"));

        // Add a button to manually reconcile dependencies, incase something messes up
        if (GUILayout.Button("Reconcile Dependencies"))
            targetedUpgrade.reconcileDependencies(getSerializedList<BasicPlayerUpgrade>(requirements));

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    ///  Makes a list from a serializedproperty that is a list.
    ///  The Type must be a object reference, but may add more overloads as needed
    /// </summary>
    /// <param name="array">The SerializedProperty containing the array</param>
    /// <typeparam name="T">The type the serializedproperty contains</typeparam>
    /// <returns>A List object from the SerializedProperty</returns>
    private List<T> getSerializedList<T>(SerializedProperty array) where T : Object {
        List<T> serializedArrayValue = new List<T>();
        for (int i = 0; i < requirements.arraySize; i++)
        {
            var current = (T)requirements.GetArrayElementAtIndex(i).objectReferenceValue;
            if (current != null) serializedArrayValue.Add(current);
        }
        return serializedArrayValue;
    }
}