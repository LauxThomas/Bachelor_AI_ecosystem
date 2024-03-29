﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public MyScriptableObjectClass myScriptableObject;
    
    public TMP_Text worldSizeText;
    public Slider worldSizeSlider;

    public TMP_Text landmassConnectionText;
    public Slider landmassConnectionSlider;

    public TMP_Text grassPercentageSliderText;
    public Slider grassPercentageSlider;

    public TMP_Text numberOfInitialBibitsSliderText;
    public Slider numberOfInitialBibitsSlider;

    public TMP_Text minimumNumberOfBibitsSliderText;
    public Slider minimumNumberOfBibitsSlider;

    private Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;

    public void Start()
    {
        worldSizeText.text = "WORLDSIZE: " + worldSizeSlider.value;
        landmassConnectionText.text = "LANDMASSCONNECTION: " + landmassConnectionSlider.value;
        grassPercentageSliderText.text = "GRASSPERCENTAGE: " + grassPercentageSlider.value + "%";
        numberOfInitialBibitsSliderText.text = "#INITIAL BIBITS: " + numberOfInitialBibitsSlider.value;
        minimumNumberOfBibitsSliderText.text = "#MINIMUM BIBITS: " + minimumNumberOfBibitsSlider.value;


        //create resolutionsarray
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<String> options = new List<string>();
        int currentResIndex = 0;
        foreach (var res in resolutions)
        {
            String o = res.width + " x " + res.width;
            options.Add(o);
        }

        resolutionDropdown.AddOptions(options);
    }

    public void SetWorldSize()
    {
        worldSizeText.text = "WORLDSIZE: " + worldSizeSlider.value;
        myScriptableObject.cameraSize = (int) worldSizeSlider.value;
    }

    public void SetLandmassConnection()
    {
        landmassConnectionText.text = "LANDMASSCONNECTION: " + landmassConnectionSlider.value;
        myScriptableObject.landMassConnection = (int) landmassConnectionSlider.value;
    }

    public void SetGrassPercentage()
    {
        grassPercentageSliderText.text = "GRASSPERCENTAGE: " + grassPercentageSlider.value + "%";
        myScriptableObject.grassPercentage = (int) grassPercentageSlider.value;
    }

    public void SetNumberOfInitialNumberOfBibits()
    {
        numberOfInitialBibitsSliderText.text = "#INITIAL BIBITS: " + numberOfInitialBibitsSlider.value;
        myScriptableObject.initialBibits = (int) numberOfInitialBibitsSlider.value;
    }

    public void SetNumberOfMinimalBibits()
    {
        minimumNumberOfBibitsSliderText.text = "#MINIMUM BIBITS: " + minimumNumberOfBibitsSlider.value;
        myScriptableObject.minimumBibits = (int) minimumNumberOfBibitsSlider.value;
    }

    public void setQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void setResolution(int resIndex)
    {
        Resolution res = resolutions[resIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}