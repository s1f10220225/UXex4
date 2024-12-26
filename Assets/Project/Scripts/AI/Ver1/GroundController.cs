using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public float rotationSpeed = 100f;

    void Update()
    {
        float horInput = Input.GetAxis("Horizontal");
        float verInput = Input.GetAxis("Vertical");
        
        Vector3 rotation = new Vector3(verInput, 0, -horInput) * rotationSpeed * Time.deltaTime;
        transform.Rotate(rotation);
    }
}