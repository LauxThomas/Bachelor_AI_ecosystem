using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovementController : MonoBehaviour
{
    private float _portalTimer = 0.1f;
    private float _targetTimer = 0f;
    public float speed = 3;
    private Transform _transform;
    private Vector3 target = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        _transform = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
//        _transform.position += Vector3.right * speed;
//        _transform.position += Vector3.up * speed;
        target = calculateRandomPosition();
        _transform.position =
            Vector3.MoveTowards(_transform.position, target, speed);
        decrementPortalTimer();
    }

    private Vector3 calculateRandomPosition()
    {
        if (_targetTimer > 0f && Vector3.Distance(transform.position, target) >= 25)
        {
            _targetTimer -= Time.fixedDeltaTime;
        }
        else
        {
            _targetTimer = Random.Range(2, 6);
            target = new Vector3(Random.Range(-1000, 1000), Random.Range(-1000, 1000), 0);
        }

        return target;
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
        String otherName = other.name.ToLower();
        handleEnemyContact(other, otherName);

        if (_portalTimer <= 0)
        {
            _portalTimer = 0.1f;
            float x = _transform.position.x;
            float y = _transform.position.y;
            float z = _transform.position.z;

            if (otherName.Contains("bottom") || otherName.Contains("upper"))
            {
                _transform.position = new Vector3(x, -y, z);
            }
            else if (otherName.Contains("left") || otherName.Contains("right"))
            {
                _transform.position = new Vector3(-x, y, z);
            }
        }
    }

    private void handleEnemyContact(Collider2D other, string otherName)
    {
        if ((otherName.Contains("t01") && this.name.ToLower().Contains("t02")) ||
            (otherName.Contains("t02") && this.name.ToLower().Contains("t01")))
        {
            if (Random.Range(0, 2) < 1)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}