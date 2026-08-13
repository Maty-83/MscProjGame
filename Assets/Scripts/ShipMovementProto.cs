using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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
    private void GetControlInputs(out float pitch, out float yaw, out float roll, out float forward, out float vertical, out float lateral, out bool resetMultiFrameInputs)
    {
        pitch = 0;
        yaw=0;
        roll = 0;
        forward = 0;
        vertical = 0;
        lateral = 0;
        //TODO: Swap this to input axes (Helps also modularity later)

        //Lateral
        if (Input.GetKey(KeyCode.Q)) { lateral++; }
        if (Input.GetKey(KeyCode.E)) { lateral--; }
        //Throttle axis (Forward/Back)
        if (Input.GetKey(KeyCode.W)) { forward++; }
        if (Input.GetKey(KeyCode.S)) { forward--; }
        //Vertical Axis (Up/Down)
        if (Input.GetKey(KeyCode.R)) { vertical++; }
        if (Input.GetKey(KeyCode.F)) { vertical--; }


        //Roll Axis
        if (Input.GetKey(KeyCode.A)) { roll++; }
        if (Input.GetKey(KeyCode.D)) { roll--; }

        if (Input.GetKey(KeyCode.I)) { pitch++; }
        if (Input.GetKey(KeyCode.K)) { pitch--; }

        if (Input.GetKey(KeyCode.L)) { yaw++; }
        if (Input.GetKey(KeyCode.J)) { yaw--; }

        resetMultiFrameInputs = Input.GetKey(KeyCode.Mouse2);
    }

    private void ApplyRotationVelChanges(bool dampIfNotAccelerating=false) 
    {
        //TODO: Apply effectors to ship. This is the distinction of the PerFrame input: If PerFrame is active, remove the motion.

        Vector3 localAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity); //P=Z, R=X, Y=Y

        Debug.Log(localAngularVelocity);

        float pitchThisFrame = Mathf.Clamp(pitchOverFrames, -1, 1);
        float pitchAcc = pitchThisFrame * pitchAccelPerSec * Time.fixedDeltaTime;
        if (pitchOnMouse)
        {
            pitchOverFrames -= pitchThisFrame;
        }
        else pitchOverFrames = 0;

        if (pitchThisFrame == 0 && dampIfNotAccelerating)
        {
            pitchAcc = Mathf.Clamp(-1 * Mathf.Sign(localAngularVelocity.z) * pitchAccelPerSec * Time.fixedDeltaTime, -Mathf.Abs(localAngularVelocity.z), Mathf.Abs(localAngularVelocity.z));
        }


        //Yaw
        float yawThisFrame = Mathf.Clamp(yawOverFrames, -1, 1);
        float yawAcc = yawThisFrame * yawAccelPerSec * Time.fixedDeltaTime;
        if (yawOnMouse)
        {
            yawOverFrames -= yawThisFrame;
        }
        else yawOverFrames = 0;

        if (yawThisFrame == 0 && dampIfNotAccelerating)
        {
            yawAcc = Mathf.Clamp(-1 * Mathf.Sign(localAngularVelocity.y) * yawAccelPerSec * Time.fixedDeltaTime, -Mathf.Abs(localAngularVelocity.y), Mathf.Abs(localAngularVelocity.y));
        }

        //Roll
        float rollThisFrame = Mathf.Clamp(rollOverFrames, -1, 1);
        float rollAcc = rollThisFrame * Time.fixedDeltaTime * rollAccelPerSec;
        if (rollOnMouse)
        {
            rollOverFrames -= rollThisFrame;
        }
        else rollOverFrames = 0;

        if (rollThisFrame == 0 && dampIfNotAccelerating)
        {
            rollAcc = Mathf.Clamp(-1 * Mathf.Sign(localAngularVelocity.x) * rollAccelPerSec * Time.fixedDeltaTime, -Mathf.Abs(localAngularVelocity.x), Mathf.Abs(localAngularVelocity.x));
        }

        //Assign the new velocity (Could be done easier if I didn't need to clamp it)

        rb.AddRelativeTorque(rollAcc, yawAcc, pitchAcc, ForceMode.VelocityChange);
    }

    private void ApplyLinearVelChanges(float forward, float lateral, float vertical, bool dampIfNotAccelerating=false)
    {
        
        var velocityInDirs = Quaternion.Inverse(rb.rotation) * rb.linearVelocity;

        float forwardAccel = 0f;
        if (forward > 0)
        {
            forwardAccel = forward * Time.fixedDeltaTime * forwardAccelPerSec;
        }
        else if (forward < 0)
        {
            forwardAccel = forward * Time.fixedDeltaTime * reverseAccelPerSec;
        }
        if(forward==0&& dampIfNotAccelerating)
        {
            //For now, we just use a simple damping as a proof of concept.
            if (velocityInDirs.x > 0)
            {
                forwardAccel = -Mathf.Min(velocityInDirs.x, Time.fixedDeltaTime * reverseAccelPerSec);
            }
            if (velocityInDirs.x < 0)//Gotta handle this separate
            {
                forwardAccel = -Mathf.Max(velocityInDirs.x, -Time.fixedDeltaTime * forwardAccelPerSec);//Confusing, NEEDS TESTING. Should work.
            }
        }
        //Side accel (Z, left-handed)
        float sideAccel = lateral * Time.fixedDeltaTime * lateralAccelPerSec;
        if (lateral == 0 && dampIfNotAccelerating)
        {
            sideAccel = Mathf.Clamp(-1 * Mathf.Sign(velocityInDirs.z) * Time.fixedDeltaTime * lateralAccelPerSec, -Mathf.Abs(velocityInDirs.z), Mathf.Abs(velocityInDirs.z));
        }

        //Vertical accel
        float vertAccel = vertical * Time.fixedDeltaTime * verticalAccelPerSec;
        if (vertical == 0 && dampIfNotAccelerating)
        {
            vertAccel = Mathf.Clamp(-1 * Mathf.Sign(velocityInDirs.y) * Time.fixedDeltaTime * verticalAccelPerSec, -Mathf.Abs(velocityInDirs.y), Mathf.Abs(velocityInDirs.y));
        }

        //TODO: Check if this works as intended based on the velocity limits. Also, implement FA-on.

        //Rotate our velocity system to local space

        //Compute new velocity
        velocityInDirs = new Vector3(
            Mathf.Clamp(velocityInDirs.x + forwardAccel, -maxReverseVel, maxVel),
            Mathf.Clamp(velocityInDirs.y + vertAccel, -maxVerticalVel, maxVerticalVel),
            Mathf.Clamp(velocityInDirs.z + sideAccel, -maxLateralVel, maxLateralVel)
            );
        //Assign newly calculated velocity vector
        rb.linearVelocity = rb.rotation * velocityInDirs;
    }
    void FixedUpdate()
    {
        //The inputs sections should be replaced by a function call in the future, same for "Over time" values
        float pitch = 0, yaw = 0, roll = 0; //Rotations
        float forward = 0, vertical = 0, lateral = 0; //Translations
        bool resetMultiFrameInputs = false;

        GetControlInputs(out pitch, out yaw, out roll, out forward, out vertical, out lateral, out resetMultiFrameInputs);

        if (resetMultiFrameInputs)
        {
            pitchOverFrames = 0;
            yawOverFrames = 0;
            rollOverFrames = 0;
        }
        else
        {
            pitchOverFrames += pitch;
            yawOverFrames += yaw;
            rollOverFrames += roll;
        }

        ApplyRotationVelChanges();

        //Now the linear velocity!

        ApplyLinearVelChanges(forward, lateral, vertical);
    }
}
