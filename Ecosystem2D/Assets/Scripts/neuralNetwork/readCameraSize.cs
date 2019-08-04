using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class readCameraSize : MonoBehaviour
{
    public MyScriptableObjectClass myScriptableObject;
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Camera>().orthographicSize = myScriptableObject.cameraSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
