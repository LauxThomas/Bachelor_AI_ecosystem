using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class childCounter : MonoBehaviour
{
    public int childrenCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        childrenCount = transform.childCount;
    }
}
