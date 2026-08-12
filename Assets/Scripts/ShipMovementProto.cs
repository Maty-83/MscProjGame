using Unity.Hierarchy;
using UnityEngine;

public class ShipMovementProto : MonoBehaviour
{
    public float pitchAccelPerSec = 0.5f;//Pitch acceleration per second
    public float maxPitchVel = 0.5f;//Max Pitch speed, Rad/Sec
    public float yawAccelPerSec = 0.5f;
    public float maxYawVel = 0.25f;//Max Yaw speed, rad/sec
    public float rollAccelPerSec = 0.5f;
    public float maxRollVel = 0.5f;//Max Roll Speed, rad/sec
    public float forwardAccelPerSec = 40f;
    public float maxVel = 200; //FA-off movement: Max speed limit. FA-on movement: Max forward velocity.
    public float reverseAccelPerSec = 20f;
    public float maxReverseVel = 100; //FA-on: Max reverse velocity.
    public float lateralAccelPerSec = 10f;
    public float maxLateralVel = 100; //FA-on movement: Max Lateral velocity
    public float verticalAccelPerSec = 10f;
    public float maxVerticalVel = 100; //FA-on movement: Max vertical velocity
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
        if (Input.GetKey(KeyCode.Q)) { side++; }
        if (Input.GetKey(KeyCode.E)) { side--; }
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
        //pitch = Input.GetAxis("MouseX"); //Temp removal
        //yaw = Input.GetAxis("MouseY");

        pitchOverFrames += pitch;
        yawOverFrames += yaw;

        //TODO: Apply effectors to ship. This is the distinction of the PerFrame input: If PerFrame is active, remove the motion.

        //Need to calculate Pitch change for a given dir. Basically the same process should exist for all other options.
        float pitchThisFrame = Mathf.Clamp(pitchOverFrames, -maxPitchVel, maxPitchVel);
        float pitchVel = Mathf.Clamp(rb.angularVelocity.x + pitchThisFrame*pitchAccelPerSec*Time.fixedDeltaTime, -maxPitchVel, maxPitchVel);
        pitchOverFrames -= pitchThisFrame;
        //Yaw
        float yawThisFrame= Mathf.Clamp(yawOverFrames, -maxYawVel, maxYawVel);
        float yawVel = Mathf.Clamp(rb.angularVelocity.y + yawThisFrame*yawAccelPerSec*Time.fixedDeltaTime, -maxYawVel, maxYawVel);
        yawOverFrames -= yawThisFrame;
        //Roll
        //TODO: After we get this fully going, we should actually do a roll over frames too.
        float rollVel= Mathf.Clamp(rb.angularVelocity.z + Time.fixedDeltaTime * rollAccelPerSec * roll, -maxRollVel, maxRollVel);
        //Assign the new velocity (Could be done easier if I didn't need to clamp it)
        rb.angularVelocity = new Vector3(rollVel, pitchVel, yawVel);

        //Now the linear velocity!

        float forwardAccel = 0f;
        if (thrust > 0)
        {
            forwardAccel = thrust * Time.fixedDeltaTime * forwardAccelPerSec;
        }
        else
        {
            forwardAccel = thrust * Time.fixedDeltaTime * reverseAccelPerSec;
        }
        //Side accel (Z, left-handed)
        float sideAccel = side * Time.fixedDeltaTime * lateralAccelPerSec;
        //Vertical accel
        float vertAccel = up * Time.fixedDeltaTime * verticalAccelPerSec;

        //TODO: Check if this works as intended based on the velocity limits. Also, implement FA-off.

        //Rotate our velocity system to local space
        var velocityInDirs = Quaternion.Inverse(rb.rotation) * rb.linearVelocity;
        //Compute new velocity
        velocityInDirs=new Vector3(
            Mathf.Clamp(velocityInDirs.x+forwardAccel,-maxReverseVel,maxVel),
            Mathf.Clamp(velocityInDirs.y+vertAccel,-maxVerticalVel,maxVerticalVel),
            Mathf.Clamp(velocityInDirs.z+sideAccel,-maxLateralVel, maxLateralVel)
            );
        //Assign newly calculated velocity vector
        rb.linearVelocity = rb.rotation * velocityInDirs;
    }
}
