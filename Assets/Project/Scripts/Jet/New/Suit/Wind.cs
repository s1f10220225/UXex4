using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Wind : MonoBehaviour
{
    [SerializeField] private float windStrength = 5f;
    [SerializeField] private bool useObjectUp = true;
    [SerializeField] private Vector3 customWindDirection = Vector3.forward;

    public float WindStrength => windStrength;

    public Vector3 WindDirection
    {
        get
        {
            if (useObjectUp)
            {
                return transform.up;
            }
            else
            {
                return customWindDirection.normalized;
            }
        }
    }
}
