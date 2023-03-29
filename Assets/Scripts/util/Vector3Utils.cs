using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Utils {
    // It returns a Vector3 with the absolute value of each component.
    public static Vector3 Abs(Vector3 of) {
        return new Vector3(
            Mathf.Abs(of.x),
            Mathf.Abs(of.y),
            Mathf.Abs(of.z)
        );
    }

    // Removes the y component of a Vector3 and normalizes it
    public static Vector3 ProjectHorizontally(Vector3 a) {
        a.y = 0;
        return a.normalized;
    }

    // Finds the midpoint of two Vector3s
    public static Vector3 Midpoint(Vector3 a, Vector3 b) {
        return Vector3.Lerp(a, b, 0.5f);
    }
}