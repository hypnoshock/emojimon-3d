using UnityEngine;

public class Easing
{
    public static float easeOutElastic(float x)
    {
        var c4 = (2 * Mathf.PI) / 3;

        return x == 0
            ? 0
            : x == 1
                ? 1
                : Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1f;
    }
}
