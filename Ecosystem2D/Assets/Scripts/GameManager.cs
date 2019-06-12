﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float windowWidth = 16.0f;
    private float windowHeight = 9.0f;

    [Range(0.5f, 5.0f)] public float vehicleMaxSpeed = 2.5f;
    [Range(0.5f, 5.0f)] public float viewRadius = 2.5f;
    [Range(10, 100)] public int vehicleHealth = 50;

    [Range(1, 25)] public int initialFoodCount = 10;
    [Range(1, 25)] public int healthModifier = 10;

    public float getWindowWidth()
    {
        return windowWidth;
    }
    
    public float getWindowHeight()
    {
        return windowHeight;
    }
}