using Unity.Hierarchy;
using UnityEngine;

public class ShipMovementProto : MonoBehaviour
{
    public float maxPitchVel = 2;//Max Pitch speed, Rad/Sec
    public float maxYawVel = 1;//Max Yaw speed, rad/sec
    public float maxRollVel = 2;//Max Roll Speed, rad/sec
    public float maxVel = 200; //FA-off movement: Max speed limit. FA-on movement: Max forward velocity.
    public float maxReverseVel = 100; //FA-on: Max reverse velocity.
    public float maxHorizontalVel = 100; //FA-on movement: Max horizontal velocity
    public float MaxVerticalVel = 100; //FA-on movement: Max vertical velocity
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool perFrameInput=false;
    public float mouseDeltaMult = 1;

    private float pitchOverFrames = 0;
    private float yawOverFrames = 0;
    private Rigidbody rb;
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //The inputs sections should be replaced by a function call in the future, same for "Over time" values
        float pitch=0, yaw=0, roll = 0; //Rotations
        float thrust = 0, up = 0, side = 0; //Translations
        //gather all inputs
        //Strafe axis (L, R)
        if (Input.GetKey(KeyCode.Q)) { side--; }
        if (Input.GetKey(KeyCode.E)) { side++; }
        //Throttle axis (Forward/Back)
        if (Input.GetKey(KeyCode.W)) { thrust++; }
        if (Input.GetKey(KeyCode.S)) { thrust--; }
        //Vertical Axis (Up/Down)
        if (Input.GetKey(KeyCode.R)) { up++; }
        if (Input.GetKey(KeyCode.F)) { up--; }
        //Roll Axis
        if (Input.GetKey(KeyCode.A)) { roll--; }
        if (Input.GetKey(KeyCode.D)) { roll++; }

        //TODO for the movement script: Input.GetAxis, as accumulation
        //TODO for movement script: Middle mouse to reset accumulation.
        bool resetMouseInputs = Input.GetKeyDown(KeyCode.Mouse2);
        if (resetMouseInputs)
        {
            pitchOverFrames = 0;
            yawOverFrames = 0;
        }
        pitch = Input.GetAxis("MouseX");
        yaw = Input.GetAxis("MouseY");

        pitchOverFrames += pitch;
        yawOverFrames += yaw;

        //TODO: Apply effectors to ship. This is the distinction of the PerFrame input: If PerFrame is active, remove the motion.
        
        

    }
}
