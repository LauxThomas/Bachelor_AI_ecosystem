using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleConfig : MonoBehaviour
{
    public float maxFOV = 180;
    public float maxAcceleration;
    public float maxVelocity;

    //Wander Variables:
    public float wanderJitter;
    public float wanderRadius;
    public float wanderDistance;
    public float wanderPriority;

    //helper functions:

    public void wrapAround(ref Vector3 vector, float xmin, float xmax, float ymin, float ymax)
    {
        if (vector.x > xmax)
        {
            vector.x = xmin;
        }
        else if (vector.x < xmin)
        {
            vector.x = xmax;
        }
        if (vector.y > ymax)
        {
            vector.y = ymin;
        }
        else if (vector.y < ymin)
        {
            vector.y = ymax;
        }

        vector.z = vector.z;
    }


    public float randomBinominal()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }
}