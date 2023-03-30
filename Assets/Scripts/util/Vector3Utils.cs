using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Utils {
    /// <summary>
    ///  Gets the absolute value of each component of the <c>Vector3</c>
    /// </summary>
    /// <param name="of">A <c>Vector3</c> that you want to get the absolute value of</param>
    /// <returns><c>Vector3</c> object where each component is >=0</returns>
    public static Vector3 Abs(Vector3 of) {
        return new Vector3(
            Mathf.Abs(of.x),
            Mathf.Abs(of.y),
            Mathf.Abs(of.z)
        );
    }

    /// <summary>
    ///  Projects a Vector3 on the horizontal plane.
    ///  Makes the y = 0 and magnitude = 1,
    ///  y can be set to anything else to project on y=yValue 
    /// </summary>
    /// <param name="a">The <c>Vector3</c> to project</param>
    /// <param name="yValue">The y component to set after Normalization.</param>
    /// <returns>The projected Vector3</returns>
    public static Vector3 ProjectHorizontally(Vector3 a, float yValue = 0) {
        a.y = 0;
        a.Normalize();
        a.y = yValue;
        return a;
    }

    /// <summary>
    ///  Finds the midpoint of two Vectors
    /// </summary>
    /// <param name="a">The first vector3</param>
    /// <param name="b">The second vector3</param>
    /// <returns>The midpoint of the two vectors</returns>
    public static Vector3 Midpoint(Vector3 a, Vector3 b) {
        return Vector3.Lerp(a, b, 0.5f);
    }
}