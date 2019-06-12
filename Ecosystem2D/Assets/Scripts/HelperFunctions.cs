using UnityEngine;
 
public class HelperFunctions
{

    public static float remap ( float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    
    public static float randomBinominal()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }

}