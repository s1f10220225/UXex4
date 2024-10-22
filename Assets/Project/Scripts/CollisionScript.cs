using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    public GameObject obj;
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Stage"))
        {
            Burst();
        }
    }

    void Burst()
    {
        Destroy(obj);
    }
}
