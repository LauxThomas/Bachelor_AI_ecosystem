using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject background;
    public GameObject organism;
    public Vector3 worldBorder = new Vector3(200f,200f,200f);
    // Start is called before the first frame update
    void Start()
    {
        background.GetComponent<Transform>().localScale = worldBorder;
        organism.GetComponent<Transform>().localScale = worldBorder / 4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
