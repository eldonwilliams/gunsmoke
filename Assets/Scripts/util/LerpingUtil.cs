using UnityEngine;

public class LerpingUtil {

    public static void lerpVar(float from, float to, float seconds, out float next)  {
        next = Mathf.Lerp(from, to, seconds * Time.deltaTime);
    }
}