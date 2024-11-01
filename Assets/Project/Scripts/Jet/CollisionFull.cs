using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionFull : MonoBehaviour
{
    public static CollisionFull Instance { get; private set; }
    public GameObject obj;
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("l_hand"))
        {
            Burst(col);
        }
    }


    void Burst(Collision col)
    {
        Destroy(col.gameObject);
    }
}
