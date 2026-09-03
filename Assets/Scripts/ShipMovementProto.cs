using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class ShipMovementProto : MonoBehaviour
{
    [Header("Pitch")]
    public float pitchAccelPerSec = 1.0f;//Pitch acceleration per second
    public float maxPitchVel = 1.0f;//Max Pitch speed, Rad/Sec
    [Header("Yaw")]
    public float yawAccelPerSec = 0.5f;
    public float maxYawVel = 1.0f;//Max Yaw speed, rad/sec
    [Header("Roll")]
    public float rollAccelPerSec = 1.0f;
    public float maxRollVel = 1.0f;//Max Roll Speed, rad/sec
    [Header("Thrust")]
    public float forwardAccelPerSec = 40f;
    public float maxVel = 200; //FA-off movement: Max speed limit. FA-on movement: Max forward velocity.
    public float reverseAccelPerSec = 20f;
    public float maxReverseVel = 100; //FA-on: Max reverse velocity.
    [Header("Lateral")]
    public float lateralAccelPerSec = 10f;
    public float maxLateralVel = 100; //FA-on movement: Max Lateral velocity
    [Header("Vertical")]
    public float verticalAccelPerSec = 10f;
    public float maxVerticalVel = 100; //FA-on movement: Max vertical velocity
    [Header("Control Settings")]
    public bool enableDamping = false; //If an input is not pressed, the ship starts decellerating to zero.
    //TODO: FA should be specifically per-axis
    public float mouseSensitivity = 1; //Multiplier on any mouse inputs.
    public bool enableRelativeMouse = true; //If enabled, causes the inputs linked to the mouse movement to decrease as they are processed.
    public float maxMouseExcess = 250; //Governs between which values pitch/yaw/roll over frames will be clamped
    [Header("Debug")]
    public bool logInputs = false;
    public bool logAppliedRotAcc = false;
    public bool logAppliedLinAcc = false;
    public bool logRotationVel = false;


    private float pitchOverFrames = 0;
    private float yawOverFrames = 0;
    private float rollOverFrames = 0;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputActionMap currentMap;
    private InputAction forwardAxis;
    private InputAction lateralAxis;
    private InputAction verticalAxis;
    private InputAction pitchAxis;
    private InputAction yawAxis;
    private InputAction rollAxis;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        playerInput= this.gameObject.GetComponent<PlayerInput>();
        currentMap = playerInput.currentActionMap;
        ReloadInputBindings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ReloadInputBindings()
    {
        forwardAxis = currentMap.FindAction("Thrust Forward",true);
        lateralAxis = currentMap.FindAction("Thrust Lateral",true);
        verticalAxis = currentMap.FindAction("Thrust Vertical",true);
        pitchAxis = currentMap.FindAction("Rotation Pitch",true);
        yawAxis = currentMap.FindAction("Rotation Yaw", true);
        rollAxis = currentMap.FindAction("Rotation Roll",true);
        Debug.Log("Reloading input bindings");
    }

    private void GetControlInputs(out float pitch, out float yaw, out float roll, out float forward, out float vertical, out float lateral, out bool resetMultiFrameInputs)
    {
        if (currentMap != playerInput.currentActionMap)
        {
            currentMap= playerInput.currentActionMap;
            ReloadInputBindings();
        }

        pitch = 0;
        yaw=0;
        roll = 0;
        forward = 0;
        vertical = 0;
        lateral = 0;
        resetMultiFrameInputs = false;
        //TODO: We can actually use InputAction.bindings to potentially retrieve if the binding is a mouse binding and behave accordingly!!!

        //Lateral (Left/Right)
        //if (Input.GetKey(KeyCode.Q)) { lateral++; }
        //if (Input.GetKey(KeyCode.E)) { lateral--; }
        lateral = lateralAxis.ReadValue<float>();

        //Throttle axis (Forward/Back)
        //if (Input.GetKey(KeyCode.W)) { forward++; }
        //if (Input.GetKey(KeyCode.S)) { forward--; }
        forward=forwardAxis.ReadValue<float>();

        //Vertical Axis (Up/Down)
        //if (Input.GetKey(KeyCode.R)) { vertical++; }
        //if (Input.GetKey(KeyCode.F)) { vertical--; }
        vertical=verticalAxis.ReadValue<float>();


        //Roll Axis
        //if (Input.GetKey(KeyCode.A)) { roll++; }
        //if (Input.GetKey(KeyCode.D)) { roll--; }
        roll=rollAxis.ReadValue<float>();

        //Pitch Axis
        //if (Input.GetKey(KeyCode.I)) { pitch++; }
        //if (Input.GetKey(KeyCode.K)) { pitch--; }
        pitch=pitchAxis.ReadValue<float>();
        

        //Yaw Axis
        //if (Input.GetKey(KeyCode.L)) { yaw++; }
        //if (Input.GetKey(KeyCode.J)) { yaw--; }
        yaw=yawAxis.ReadValue<float>();

        if (logInputs) Debug.Log($"Raw Input: F: {forward}, L: {lateral}, V: {vertical}, P:{pitch}, Y:{yaw}, R: {roll}");

        //resetMultiFrameInputs = Input.GetKey(KeyCode.Mouse2);
    }

    private void ApplyRotationVelChanges(bool dampIfNotAccelerating=false) 
    {
        //TODO: Apply effectors to ship. This is the distinction of the PerFrame input: If PerFrame is active, remove the motion.

        Vector3 localAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity); //P=Z, R=X, Y=Y

        float pitchThisFrame = Mathf.Clamp(pitchOverFrames, -1, 1);
        float pitchAcc = pitchThisFrame * pitchAccelPerSec * Time.fixedDeltaTime;

        if(enableRelativeMouse) pitchOverFrames -= pitchThisFrame;
        //pitchOverFrames = 0;

        if (pitchThisFrame == 0 && dampIfNotAccelerating)
        {
            pitchAcc = Mathf.Clamp(-1 * Mathf.Sign(localAngularVelocity.z) * pitchAccelPerSec * Time.fixedDeltaTime, -Mathf.Abs(localAngularVelocity.z), Mathf.Abs(localAngularVelocity.z));
        }


        //Yaw
        float yawThisFrame = Mathf.Clamp(yawOverFrames, -1, 1);
        float yawAcc = yawThisFrame * yawAccelPerSec * Time.fixedDeltaTime;

        if (enableRelativeMouse) yawOverFrames -= yawThisFrame;
        //yawOverFrames = 0;

        if (yawThisFrame == 0 && dampIfNotAccelerating)
        {
            yawAcc = Mathf.Clamp(-1 * Mathf.Sign(localAngularVelocity.y) * yawAccelPerSec * Time.fixedDeltaTime, -Mathf.Abs(localAngularVelocity.y), Mathf.Abs(localAngularVelocity.y));
        }

        //Roll
        
        float rollThisFrame = Mathf.Clamp(rollOverFrames, -1, 1);
        float rollAcc = rollThisFrame * Time.fixedDeltaTime * rollAccelPerSec;
        if (enableRelativeMouse) rollOverFrames -= rollThisFrame;
        //rollOverFrames = 0;

        if (rollThisFrame == 0 && dampIfNotAccelerating)
        {
            rollAcc = Mathf.Clamp(-1 * Mathf.Sign(localAngularVelocity.x) * rollAccelPerSec * Time.fixedDeltaTime, -Mathf.Abs(localAngularVelocity.x), Mathf.Abs(localAngularVelocity.x));
        }
        if(logAppliedRotAcc) Debug.Log($"Applied rotation input: P:{pitchThisFrame}, Y:{yawThisFrame}, R: {rollThisFrame}");
        if(logRotationVel) Debug.Log($"Current rotation rate: {localAngularVelocity}");
        //Add torque
        rb.AddRelativeTorque(rollAcc, yawAcc, pitchAcc, ForceMode.VelocityChange);
    }

    private void ApplyLinearVelChanges(float forward, float lateral, float vertical, bool dampIfNotAccelerating=false)
    {
        //Rotate our velocity system to local space
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
            if (velocityInDirs.x < 0)//Gotta handle this separate due to forward and reverse being as they are.
            {
                forwardAccel = -Mathf.Max(velocityInDirs.x, -Time.fixedDeltaTime * forwardAccelPerSec);
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

        //Debug
        if (logAppliedLinAcc) Debug.Log($"Applied linear input: F: {forward}, L: {lateral}, V: {vertical}");

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
        GetControlInputs(out float pitch, out float yaw, out float roll, out float forward, out float vertical, out float lateral, out bool resetMultiFrameInputs);

        //TEMP: For testing damping
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            enableDamping = !enableDamping;
        }

        if (resetMultiFrameInputs)
        {
            pitchOverFrames = 0;
            yawOverFrames = 0;
            rollOverFrames = 0;
        }
        else
        {
            pitchOverFrames = Mathf.Clamp(pitchOverFrames+pitch, -maxMouseExcess, maxMouseExcess); ;
            yawOverFrames = Mathf.Clamp(yawOverFrames+yaw, -maxMouseExcess, maxMouseExcess);
            rollOverFrames = Mathf.Clamp(rollOverFrames+roll, -maxMouseExcess, maxMouseExcess);
        }

        ApplyRotationVelChanges(enableDamping);

        ApplyLinearVelChanges(forward, lateral, vertical, enableDamping);
    }
}
