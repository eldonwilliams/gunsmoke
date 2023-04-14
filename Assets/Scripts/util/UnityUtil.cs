using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnityUtil
{
    /// <summary>
    ///  Gets the reference to a behavior in the root object, finds the first instance.
    /// </summary>
    /// <typeparam name="T">The behavior type to look for</typeparam>
    /// <returns>The found behavior instance</returns>
    public static T GetRootComponent<T>() {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject rootObject in rootObjects)
        {
            T found = rootObject.GetComponentInChildren<T>();
            if (found != null) return found;
        }

        return default(T);
    }

    /// <summary>
    ///  Shortcut of GetRootObject<T>().gameObject
    /// </summary>
    /// <typeparam name="T">The unity behavior type to look for on root game objects</typeparam>
    /// <returns>The gameobject that has the behavior</returns>
    public static GameObject GetRootObject<T>() {
        return GetRootObject<T>().gameObject;
    }
}
