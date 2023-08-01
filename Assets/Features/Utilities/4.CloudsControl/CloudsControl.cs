using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;


public class CloudsControl : MonoBehaviour
{
    bool canControl = false;
    float speed;
    Vector3 destination;

    void OnEnable()
    {
        canControl = true;
        AnimateClouds();
    }

    void AnimateClouds()
    {
        float x = Random.Range(-300, 300);
        float y = Random.Range(-300, 300);
        speed = Random.Range(30f, 100f);
        destination = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + 1000);
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (canControl)
        {
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, destination, step);
        }
    }
}
