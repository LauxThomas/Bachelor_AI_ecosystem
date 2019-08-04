using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class MyScriptableObjectClass : ScriptableObject {
    public int cameraSize = 30;
    public int landMassConnection = 30;
    public int grassPercentage = 60;
    public int initialBibits = 100;
    public int minimumBibits = 50;
}