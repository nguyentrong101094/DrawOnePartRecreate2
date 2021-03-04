using UnityEngine;
public class MathModulo
{
    public static float Modulo(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }
}
