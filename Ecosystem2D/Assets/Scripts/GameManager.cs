using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float windowWidth = 9.0f;
    public float windowHeight = 5.0f;

    [Range(0.5f, 5.0f)] public float vehicleMaxSpeed = 2.5f;
    [Range(0.5f, 5.0f)] public float viewRadius = 2.5f;
    [Range(10, 100)] public int vehicleHealth = 50;

    [Range(1, 25)] public int initialFoodCount = 10;
    [Range(1, 25)] public int healthModifier = 10;
}