using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private float windowWidth = 16.0f;
    private float windowHeight = 9.0f;

    [Range(0.5f, 5.0f)] public float vehicleMaxSpeed = 2.5f;
//    [Range(0.5f, 5.0f)] public float viewRadius = 2.5f;
    [Range(10, 100)] public int vehicleHealth = 50;

    [Range(1, 25)] public int initialFoodCount = 10;
    [Range(0, 25)] public int healthDegen = 0;
    [Range(1, 25)] public int cloningRate = 10;
    
    [Range(10, 100)] public static int staticVehicleHealth = 50;
    [Range(0.5f, 5.0f)] public static float staticViewRadius = 1f;
    [Range(0.5f, 5.0f)] public static float staticVehicleMaxSpeed = 2.5f;
    [Range(0.5f, 5.0f)]public static float staticVehicleMaxForce = 2;
    private static List<float> dna;

    private void Awake()
    {
        dna = new List<float>();
//        dna.Add(1);
//        dna.Add(1);
        dna.Add(Random.Range(-5, 5));
        dna.Add(Random.Range(-5, 5));
    }

    public static List<float> getDNA()
    {
        return dna;
    }

    public float getWindowWidth()
    {
        return windowWidth;
    }
    
    public float getWindowHeight()
    {
        return windowHeight;
    }
}