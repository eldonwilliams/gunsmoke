using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RandomRangeFloat  {
    /// <summary>
    ///  Minimum value of range
    /// </summary>
    [Tooltip("Minimum value of range")]
    public float min;
    /// <summary>
    ///  Maximum value of range
    /// </summary>
    [Tooltip("Maximum value of range")]
    public float max;

    /// <summary>
    ///  Gets a value between min and max, returning 0 if min > max
    /// </summary>
    /// <returns>a value</returns>
    public float generateValue() {
        if (min > max) return 0;
        return Random.Range(min, max);
    }
}

[System.Serializable]
public struct RandomRangeInt  {
    /// <summary>
    ///  Minimum value of range
    /// </summary>
    [Tooltip("Minimum value of range")]
    public int min;
    /// <summary>
    ///  Maximum value of range
    /// </summary>
    [Tooltip("Maximum value of range")]
    public int max;

    /// <summary>
    ///  Gets a value between min and max, returning 0 if min > max
    /// </summary>
    /// <returns>a value</returns>
    public int generateValue() {
        if (min > max) return 0;
        return Random.Range(min, max);
    }
}