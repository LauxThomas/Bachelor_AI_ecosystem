using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private float _portalTimer = 0.1f;
    public float speed = 3;
    private Transform _transform;

    // Start is called before the first frame update
    void Start()
    {
        _transform = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _transform.position += Vector3.right * speed;
        _transform.position += Vector3.up * speed;
        decrementPortalTimer();
    }

    private void decrementPortalTimer()
    {
        if (_portalTimer > 0f)
        {
            _portalTimer -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_portalTimer <= 0)
        {
            _portalTimer = 0.1f;
            float x = _transform.position.x;
            float y = _transform.position.y;
            float z = _transform.position.z;
            String otherName = other.name.ToLower();

            if (otherName.Contains("bottom") || otherName.Contains("upper"))
            {
                _transform.position = new Vector3(x, -y, z);
                Debug.Log("upperBottom");
            }
            else if (otherName.Contains("left") || otherName.Contains("right"))
            {
                _transform.position = new Vector3(-x, y, z);
                Debug.Log("leftRight");
            }
        }
    }
}