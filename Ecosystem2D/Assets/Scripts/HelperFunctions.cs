using UnityEngine;
 
public static class HelperFunctions
{
    public static Vector2 rsVector2(this Vector3 _v)
    {
        return new Vector2(_v.x, _v.y);
    }
    public static float remap (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}