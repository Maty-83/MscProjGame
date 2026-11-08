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
            if (vel.sqrMagnitude <0.00001f) //edge-case catch for when stationary or near stationary.
            {
                vel = rb.transform.right;//The entire ship faces right, so we adjust for that. Should really fix it, but I'm too deep in.
            }
            transform.rotation = Quaternion.LookRotation(vel);
        }
        else
        {
            Debug.Log("DirectionIndicator.cs: Missing Rigidbody reference");
        }
    }
}
