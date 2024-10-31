using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionFull : MonoBehaviour
{
    public GameObject obj;
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("item"))
        {
            // Burst(col);
        }
    }

    // void Burst(col)
    // {
    //     Destroy(col);
    // }
}
