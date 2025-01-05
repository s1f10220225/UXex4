using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingRotate : MonoBehaviour
{
    public float Rotation = 1f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, Rotation);
    }
}
