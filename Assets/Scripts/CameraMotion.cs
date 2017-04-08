using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour 
{

    public Vector3 target;
    public float distance;
    public float height;
    public float speed;


    private void Update()
    {
        var pos = target;
        var angle = 2f + Time.time * speed;
        pos += distance * new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        pos.y = height;
        transform.position = pos;
        transform.LookAt(target);
    }

}