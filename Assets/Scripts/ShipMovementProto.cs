using UnityEngine;

public class ShipMovementProto : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //gather all inputs
        //Strafe axis (L, R)
        if (Input.GetKey(KeyCode.Q)) { }
        if (Input.GetKey(KeyCode.E)) { }
        //Throttle axis (Forward/Back)
        if (Input.GetKey(KeyCode.W)) { }
        if (Input.GetKey(KeyCode.S)) { }
        //Vertical Axis (Up/Down)
        if (Input.GetKey(KeyCode.R)) { }
        if (Input.GetKey(KeyCode.F)) { }
        //Roll Axis
        if (Input.GetKey(KeyCode.A)) { }
        if (Input.GetKey(KeyCode.D)) { }

        //TODO for the movement script: Input.GetAxis, as accumulation
        //TODO for movement script: Middle mouse to reset accumulation.

    }
}
