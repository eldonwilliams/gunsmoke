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
}