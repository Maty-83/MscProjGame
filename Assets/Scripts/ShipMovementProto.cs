using Unity.Hierarchy;
using UnityEngine;

public class ShipMovementProto : MonoBehaviour
{
    public float pitchAccelPerSec = 1.0f;//Pitch acceleration per second
    public float maxPitchVel = 1.0f;//Max Pitch speed, Rad/Sec
    public bool pitchOnMouse = false; //If pitch is on mouse, we collect overflow inputs and apply until overflow is exhausted

    public float yawAccelPerSec = 0.5f;
    public float maxYawVel = 1.0f;//Max Yaw speed, rad/sec
    public bool yawOnMouse = false; //If yaw is on mouse, we collect overflow inputs and apply until overflow is exhausted

    public float rollAccelPerSec = 1.0f;
    public float maxRollVel = 1.0f;//Max Roll Speed, rad/sec
    public bool rollOnMouse = false; //If roll is on mouse, we collect overflow inputs and apply until overflow is exhausted

    public float forwardAccelPerSec = 40f;
    public float maxVel = 200; //FA-off movement: Max speed limit. FA-on movement: Max forward velocity.

    public float reverseAccelPerSec = 20f;
    public float maxReverseVel = 100; //FA-on: Max reverse velocity.

    public float lateralAccelPerSec = 10f;
    public float maxLateralVel = 100; //FA-on movement: Max Lateral velocity

    public float verticalAccelPerSec = 10f;
    public float maxVerticalVel = 100; //FA-on movement: Max vertical velocity

    
    public bool perFrameInput=false;
    public float mouseDeltaMult = 1;

    private float pitchOverFrames = 0;
    private float yawOverFrames = 0;
    private float rollOverFrames = 0;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        float pitch = 0, yaw = 0, roll = 0; //Rotations
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
        if (Input.GetKey(KeyCode.A)) { roll++; }
        if (Input.GetKey(KeyCode.D)) { roll--; }




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
        //TEMP: Pitch and roll:
        if (Input.GetKey(KeyCode.I)) { pitch++; }
        if (Input.GetKey(KeyCode.K)) { pitch--; }
        if (Input.GetKey(KeyCode.L)) { yaw++; }
        if (Input.GetKey(KeyCode.J)) { yaw--; }

        pitchOverFrames += pitch;
        yawOverFrames += yaw;
        rollOverFrames += roll;

        //TODO: Apply effectors to ship. This is the distinction of the PerFrame input: If PerFrame is active, remove the motion.

        /* How this is SUPPOSED to work
         * Calculate changes in local coordinate space
         * Add said changes to the GLOBAL rotation.
         * Then, transform rotation into LOCAL space again and clamp speeds (Or if they're over, at least dampen them by some amount)
         * THEN TRANSFORM IT BACK AND SET AS A RESULT
         */

        
        float pitchThisFrame = Mathf.Clamp(pitchOverFrames, -1, 1);
        float pitchAcc = pitchThisFrame * pitchAccelPerSec * Time.fixedDeltaTime;
        if (pitchOnMouse)
        {
            pitchOverFrames -= pitchThisFrame;
        }
        else pitchOverFrames = 0;
        //Yaw
        float yawThisFrame= Mathf.Clamp(yawOverFrames, -1, 1);
        float yawAcc = yawThisFrame * yawAccelPerSec * Time.fixedDeltaTime;
        if (yawOnMouse)
        {
            yawOverFrames -= yawThisFrame;
        }
        else yawOverFrames = 0;
        //Roll
        //TODO: After we get this fully going, we should actually do a roll over frames too.
        float rollThisFrame = Mathf.Clamp(rollOverFrames, -1, 1);
        float rollAcc = rollThisFrame * Time.fixedDeltaTime * rollAccelPerSec;
        if (rollOnMouse)
        {
            rollOverFrames-=rollThisFrame;
        }
        else rollOverFrames = 0;

        //Assign the new velocity (Could be done easier if I didn't need to clamp it)

        //Calculate roll delta
        var rollDelta = rb.angularVelocity.x * Time.fixedDeltaTime;

        rb.AddRelativeTorque(rollAcc, yawAcc, pitchAcc, ForceMode.VelocityChange);

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
