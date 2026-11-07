using UnityEditor.UI;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rb != null)
        {
            var vel = rb.linearVelocity;
            if (vel.magnitude == 0) //edge-case catch for when stationary.
            {
                vel = Vector3.forward;
            }
            transform.rotation = Quaternion.LookRotation(vel);
        }
        else
        {
            Debug.Log("DirectionIndicator.cs: Missing Rigidbody reference");
        }
    }
}
