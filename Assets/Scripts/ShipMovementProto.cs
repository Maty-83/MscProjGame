using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
/// <summary>
/// Specifies control types for controla axes. 
/// Direct means a clamped -1/+1 accumulation (Sensitivity here adjusts curve)
/// Direct Damped will slow a given axis to zero without input
/// Accumulation will "overflow" inputs and execute until the total time of input is reached (Time in physics frames)
/// Accumulation Damped also slows to 0 once accumulation of inputs is at 0
/// Throttle will convert accumulation into a throttle type, where we change velocity until a given percent of max speed. It essentially damps to the throttle level*max velocity.
/// </summary>
public enum AxisControlType
{
    Direct,
    DirectDamped,
    Accumulation,
    AccumulationDamped,
    Throttle

}
public class ShipMovementProto : MonoBehaviour
{
    [Header("Pitch")]
    public float pitchAccelPerSec = 1.0f;//Pitch acceleration per second
    public float maxPitchVel = 1.0f;//Max Pitch speed, Rad/Sec
    public AxisControlType pitchControlType = AxisControlType.Direct;
    public float pitchSensitivity = 1.0f;

    [Header("Yaw")]
    public float yawAccelPerSec = 0.5f;
    public float maxYawVel = 1.0f;//Max Yaw speed, rad/sec
    public AxisControlType yawControlType = AxisControlType.Direct;
    public float yawSensitivity = 1.0f;

    [Header("Roll")]
    public float rollAccelPerSec = 1.0f;
    public float maxRollVel = 1.0f;//Max Roll Speed, rad/sec
    public AxisControlType rollControlType = AxisControlType.Direct;
    public float rollSensitivity = 1.0f;

    [Header("Thrust")]
    public float forwardAccelPerSec = 40f;
    public float maxVel = 200; //FA-off movement: Max speed limit. FA-on movement: Max forward velocity.
    public float reverseAccelPerSec = 20f;
    public float maxReverseVel = 100; //FA-on: Max reverse velocity.
    public AxisControlType thrustControlType=AxisControlType.Direct;
    public float thrustSensitivity = 1.0f;

    [Header("Lateral")]
    public float lateralAccelPerSec = 10f;
    public float maxLateralVel = 100; //FA-on movement: Max Lateral velocity
    public AxisControlType lateralControlType= AxisControlType.Direct;
    public float lateralSensitivity = 1.0f;

    [Header("Vertical")]
    public float verticalAccelPerSec = 10f;
    public float maxVerticalVel = 100; //FA-on movement: Max vertical velocity
    public AxisControlType verticalControlType = AxisControlType.Direct;
    public float verticalSensitivity = 1.0f;

    [Header("Control Settings")]
    public float maxAccumulation = 100; //Governs between which values pitch/yaw/roll over frames will be clamped
    public float accumulationToFullThrottle = 100;//Determines much accumulated input is needed to reach full throttle.
    [Header("Debug")]
    public bool logInputs = false;
    public bool logAppliedRotAcc = false;
    public bool logAppliedLinAcc = false;
    public bool logRotationVel = false;


    private float pitchOverFrames = 0;
    private float yawOverFrames = 0;
    private float rollOverFrames = 0;

    private float thrustOverFrames = 0;
    private float verticalOverFrames = 0;
    private float lateralOverFrames = 0;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputActionMap currentMap;
    private InputAction forwardAxis;
    private InputAction lateralAxis;
    private InputAction verticalAxis;
    private InputAction pitchAxis;
    private InputAction yawAxis;
    private InputAction rollAxis;

    /// <summary>
    /// Gets the percent of used thrust last physics frame in each axis (Order: Thrust, Vertical, Lateral)
    /// </summary>
    public Vector3 LastFrameInputPercentLinear { get; private set; }= new Vector3();
    /// <summary>
    /// Gets percent of used rotation accel last physics frame in each axis (Order: Roll, Yaw, Pitch)
    /// </summary>
    public Vector3 LastFrameInputPercentRotation { get; private set; } = new Vector3();
    private enum RotationAxis
    {
        Roll,
        Yaw,
        Pitch
    }
    
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
    
    /// <summary>
    /// Returns bounded acceleration to a target velocity, from a given velocity
    /// </summary>
    /// <param name="targetVelocity">Velocity to reach</param>
    /// <param name="currentVelocity">Current velocity</param>
    /// <param name="maxAcc">Maximum velocity change permitted</param>
    /// <param name="maxAccNegative">Maximum negative velocity change permitted; if negative, uses maxAcc instead.</param>
    /// <returns>Bounded acceleration to the targeted velocity.</returns>
    private float GetAccelToTarget(float targetVelocity, float currentVelocity, float maxAcc, float maxAccNegative = -1)
    {
        float velDiff=targetVelocity-currentVelocity;
        if (velDiff == 0) return 0;//No need for anything fancy, if the difference is zero, just don't do anything.

        if (maxAccNegative < 0) maxAccNegative = maxAcc;
        if (velDiff > 0)
        {
            //we accelerate with maxAcc
            return Mathf.Min(velDiff, maxAcc);
        }
        else
        {
            //we accelerate with maxAccNegative
            return -Mathf.Min(-velDiff, maxAccNegative);
        }
    }

    private void SetLastFrameRotAccel(float pitchAcc, float yawAcc, float rollAcc)
    {
        //Back-calculate max per-frame accel
        float MPA = pitchAccelPerSec * Time.fixedDeltaTime;
        float MYA = yawAccelPerSec * Time.fixedDeltaTime;
        float MRA = rollAccelPerSec * Time.fixedDeltaTime;

        LastFrameInputPercentRotation = new Vector3(rollAcc / MRA, yawAcc / MYA, pitchAcc / MPA);
    }

    /// <summary>
    /// Returns rotation acceleration for a given axis from globally stored control values
    /// </summary>
    /// <param name="axis">Axis of acceleration</param>
    /// <param name="localAngularVelocity">Vector of locally transformed angular velocity</param>
    /// <param name="dampIfNotAccelerating">Determines if rotation damping takes place</param>
    /// <returns>Direct acceleration value for a given rotation axis</returns>
    private float ReturnRotAcceleration(RotationAxis axis, Vector3 localAngularVelocity, AxisControlType controlType)
    {
        float axisAngularVelocity = 0;
        float inputOverFrames = 0;
        float accelPerSec = 0;
        float axisSpeedLimit = 1;
        switch (axis)
        {
            case RotationAxis.Roll:
                axisAngularVelocity = localAngularVelocity.x;
                inputOverFrames = rollOverFrames;
                accelPerSec = rollAccelPerSec;
                axisSpeedLimit = maxRollVel;
                break;
            case RotationAxis.Yaw:
                axisAngularVelocity = localAngularVelocity.y;
                inputOverFrames = yawOverFrames;
                accelPerSec = yawAccelPerSec;
                axisSpeedLimit = maxYawVel;
                break;
            case RotationAxis.Pitch:
                axisAngularVelocity = localAngularVelocity.z;
                inputOverFrames = pitchOverFrames;
                accelPerSec = pitchAccelPerSec;
                axisSpeedLimit = maxPitchVel;
                break;
            default:
                Debug.Log("[ERROR]: unknown rotation axis");
                return 0;
        }
        float inputThisFrame = Mathf.Clamp(inputOverFrames, -1, 1);

        //Throttle Control uses a completely different mechanism than everything else.
        float accel = 0;
        if (controlType == AxisControlType.Throttle)
        {
            float targetVelocity = Mathf.Clamp(inputOverFrames / accumulationToFullThrottle, -1, 1)*axisSpeedLimit;
            accel=GetAccelToTarget(targetVelocity, axisAngularVelocity, accelPerSec*Time.fixedDeltaTime);
        }
        else
        {
            accel = inputThisFrame * accelPerSec * Time.fixedDeltaTime;

            //Damp setting:
            if (inputThisFrame == 0 && (controlType == AxisControlType.DirectDamped || controlType == AxisControlType.AccumulationDamped))
            {
                accel = Mathf.Clamp(-1 * Mathf.Sign(axisAngularVelocity) * accelPerSec * Time.fixedDeltaTime, -Mathf.Abs(axisAngularVelocity), Mathf.Abs(axisAngularVelocity));
            }

            //If we have relative mouse enabled, reduce the input over frames by how much we're "using up" this frame (If we're at max rotation velocity, the input disappears slowly)
            switch (axis)
            {
                case RotationAxis.Roll:
                    if (controlType == AxisControlType.Direct || controlType == AxisControlType.DirectDamped) rollOverFrames = 0;
                    if (controlType == AxisControlType.Accumulation || controlType == AxisControlType.AccumulationDamped) rollOverFrames -= inputThisFrame;
                    break;
                case RotationAxis.Yaw:
                    if (controlType == AxisControlType.Direct || controlType == AxisControlType.DirectDamped) yawOverFrames = 0;
                    if (controlType == AxisControlType.Accumulation || controlType == AxisControlType.AccumulationDamped) yawOverFrames -= inputThisFrame;
                    break;
                case RotationAxis.Pitch:
                    if (controlType == AxisControlType.Direct || controlType == AxisControlType.DirectDamped) pitchOverFrames = 0;
                    if (controlType == AxisControlType.Accumulation || controlType == AxisControlType.AccumulationDamped) pitchOverFrames -= inputThisFrame;
                    break;
                default:
                    return 0;
            }
            //Maximum velocity limiter
            if (Mathf.Abs(axisAngularVelocity + accel) > axisSpeedLimit)
            {
                if (Mathf.Abs(axisAngularVelocity) > axisSpeedLimit)
                {
                    //We now need to counteract the difference between the speed limit and max velocity and damp it down.
                    accel = -1 * Mathf.Sign(axisAngularVelocity) * Mathf.Min(accelPerSec * Time.fixedDeltaTime, Mathf.Abs(axisAngularVelocity) - axisSpeedLimit);
                }
                else
                {
                    //We just need to reduce the permitted accel so with axisAngularVelocity it equals the limit
                    accel = Mathf.Sign(accel) * (axisSpeedLimit - Mathf.Abs(axisAngularVelocity));
                    float total = axisAngularVelocity + accel;
                }
            }
        }
        return accel;
    }
    /// <summary>
    /// Applies rotational velocity changes depending on the inputs last recorded.
    /// </summary>
    /// <param name="dampIfNotAccelerating">Determines if acceleration damping is active</param>
    private void ApplyRotationVelChanges() 
    {
        Vector3 localAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity); //P=Z, R=X, Y=Y

        //Pitch
        float pitchAcc=ReturnRotAcceleration(RotationAxis.Pitch, localAngularVelocity, pitchControlType);

        //Yaw
        float yawAcc=ReturnRotAcceleration(RotationAxis.Yaw, localAngularVelocity, yawControlType);

        //Roll
        float rollAcc=ReturnRotAcceleration(RotationAxis.Roll, localAngularVelocity, rollControlType);

        SetLastFrameRotAccel(pitchAcc, yawAcc, rollAcc);
        if(logAppliedRotAcc) Debug.Log($"Applied rotation input: P:{pitchAcc}, Y:{yawAcc}, R: {rollAcc}");
        if(logRotationVel) Debug.Log($"Current rotation rate: {localAngularVelocity}");
        //Add torque
        rb.AddRelativeTorque(rollAcc, yawAcc, pitchAcc, ForceMode.VelocityChange);
    }
    private void SetLastFrameLinAccel(float thrustAcc, float vertAcc, float latAcc)
    {
        LastFrameInputPercentLinear = new Vector3(thrustAcc, vertAcc, latAcc);
    }
    private void ApplyLinearVelChanges()
    {
        //Rotate our velocity system to local space
        var velocityInDirs = Quaternion.Inverse(rb.rotation) * rb.linearVelocity;

        //Forward/Back axis
        float thrustAccel = 0f;
        //"Throttle" control type.
        float thrustThisFrame = 0;
        if (thrustControlType == AxisControlType.Throttle) 
        {
            float targetPercent = Mathf.Clamp(thrustOverFrames / accumulationToFullThrottle,-1,1);
            float target;
            if(targetPercent < 0)
            {
                target = targetPercent * maxReverseVel;
            }
            else
            {
                target = targetPercent * maxVel;
            }
            thrustAccel = GetAccelToTarget(target, velocityInDirs.x, forwardAccelPerSec*Time.fixedDeltaTime, reverseAccelPerSec*Time.fixedDeltaTime);
            thrustThisFrame = targetPercent;
        }
        else {
            thrustThisFrame = Mathf.Clamp(thrustOverFrames, -1, 1);
            //Update "over frames" value
            if (thrustControlType == AxisControlType.Direct || thrustControlType == AxisControlType.DirectDamped)
            {
                thrustOverFrames = 0;
            }
            if (thrustControlType == AxisControlType.Accumulation || thrustControlType == AxisControlType.AccumulationDamped)
            {
                thrustOverFrames -= thrustThisFrame;
            }
            //Actual acceleration calculation
            if (thrustThisFrame > 0)
            {
                thrustAccel = thrustThisFrame * Time.fixedDeltaTime * forwardAccelPerSec;
            }
            else if (thrustThisFrame < 0)
            {
                thrustAccel = thrustThisFrame * Time.fixedDeltaTime * reverseAccelPerSec;
            }
            if (thrustThisFrame == 0 && (thrustControlType == AxisControlType.DirectDamped || thrustControlType == AxisControlType.AccumulationDamped))
            {
                //For now, we just use a simple damping as a proof of concept.
                if (velocityInDirs.x > 0)
                {
                    thrustAccel = -Mathf.Min(velocityInDirs.x, Time.fixedDeltaTime * reverseAccelPerSec);
                }
                if (velocityInDirs.x < 0)//Gotta handle this separate due to forward and reverse being as they are.
                {
                    thrustAccel = -Mathf.Max(velocityInDirs.x, -Time.fixedDeltaTime * forwardAccelPerSec);
                }
            }
        }

        //Side accel (Z, left-handed)
        float latAccel=0;
        float lateralThisFrame = 0;
        //"Throttle" control type
        if (lateralControlType == AxisControlType.Throttle)
        {
            float targetPercent = Mathf.Clamp(lateralOverFrames / accumulationToFullThrottle, -1, 1);
            latAccel = GetAccelToTarget(targetPercent * maxLateralVel, velocityInDirs.z, lateralAccelPerSec * Time.fixedDeltaTime);
            lateralThisFrame = targetPercent;
        }
        else
        {
            lateralThisFrame = Mathf.Clamp(lateralOverFrames, -1, 1);
            //Update "over frames" value
            if (lateralControlType == AxisControlType.Direct || lateralControlType == AxisControlType.DirectDamped)
            {
                lateralOverFrames = 0;
            }
            if (lateralControlType == AxisControlType.Accumulation || lateralControlType == AxisControlType.AccumulationDamped)
            {
                lateralOverFrames -= lateralThisFrame;
            }
            //Actual acceleration calculation.
            latAccel = lateralThisFrame * Time.fixedDeltaTime * lateralAccelPerSec;
            if (lateralThisFrame == 0 && (lateralControlType == AxisControlType.DirectDamped || lateralControlType == AxisControlType.AccumulationDamped))
            {
                latAccel = Mathf.Clamp(-1 * Mathf.Sign(velocityInDirs.z) * Time.fixedDeltaTime * lateralAccelPerSec, -Mathf.Abs(velocityInDirs.z), Mathf.Abs(velocityInDirs.z));
            }
        }

        //Vertical accel
        float vertAccel = 0;
        float verticalThisFrame = 0;
        //Throttle control
        if (verticalControlType == AxisControlType.Throttle)
        {
            float targetPercent = Mathf.Clamp(verticalOverFrames / accumulationToFullThrottle, -1, 1);
            vertAccel = GetAccelToTarget(targetPercent * maxVerticalVel, velocityInDirs.y, verticalAccelPerSec * Time.fixedDeltaTime);
            verticalThisFrame = targetPercent;
        }
        else
        {
            verticalThisFrame = Mathf.Clamp(verticalOverFrames, -1, 1);
            //Update "over frames" value
            if (verticalControlType == AxisControlType.Direct || verticalControlType == AxisControlType.DirectDamped)
            {
                verticalOverFrames = 0;
            }
            if (verticalControlType == AxisControlType.Accumulation || verticalControlType == AxisControlType.AccumulationDamped)
            {
                verticalOverFrames -= verticalThisFrame;
            }
            //Actual acceleration calculation
            vertAccel = verticalThisFrame * Time.fixedDeltaTime * verticalAccelPerSec;
            if (verticalThisFrame == 0 && (verticalControlType == AxisControlType.DirectDamped || verticalControlType == AxisControlType.AccumulationDamped))
            {
                vertAccel = Mathf.Clamp(-1 * Mathf.Sign(velocityInDirs.y) * Time.fixedDeltaTime * verticalAccelPerSec, -Mathf.Abs(velocityInDirs.y), Mathf.Abs(velocityInDirs.y));
            }
        }

        //Debug
        if (logAppliedLinAcc) Debug.Log($"Applied linear input: F: {thrustThisFrame}, L: {lateralThisFrame}, V: {verticalThisFrame}");

        //TODO: Remove the "Harsh" limiter here maybe? Think it over.
        //Compute new velocity
        velocityInDirs = new Vector3(
            Mathf.Clamp(velocityInDirs.x + thrustAccel, -maxReverseVel, maxVel),
            Mathf.Clamp(velocityInDirs.y + vertAccel, -maxVerticalVel, maxVerticalVel),
            Mathf.Clamp(velocityInDirs.z + latAccel, -maxLateralVel, maxLateralVel)
            );
        if (velocityInDirs.magnitude > maxVel)
        {
            velocityInDirs = (velocityInDirs / velocityInDirs.magnitude) * maxVel;//Normalize the vector to max speed. I'd rather do it this way as it's less confusing.
        }
        SetLastFrameLinAccel(thrustThisFrame, verticalThisFrame, lateralThisFrame);
        //SetLastFrameLinAccel(thrustAccel, vertAccel, latAccel);
        //Assign newly calculated velocity vector
        rb.linearVelocity = rb.rotation * velocityInDirs;
    }
    void FixedUpdate()
    {
        GetControlInputs(out float pitch, out float yaw, out float roll, out float thrust, out float vertical, out float lateral, out bool resetMultiFrameInputs);

        //TEMP: For testing damping
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            enableDamping = !enableDamping;
        }*/

        if (resetMultiFrameInputs)
        {
            pitchOverFrames = pitch*pitchSensitivity;
            yawOverFrames = yaw*yawSensitivity;
            rollOverFrames = roll*rollSensitivity;
            thrustOverFrames = thrust*thrustSensitivity;
            verticalOverFrames = vertical*verticalSensitivity;
            lateralOverFrames = lateral*lateralSensitivity;
        }
        else
        {
            pitchOverFrames = Mathf.Clamp(pitchOverFrames+pitch*pitchSensitivity, -maxAccumulation, maxAccumulation); ;
            yawOverFrames = Mathf.Clamp(yawOverFrames+yaw*yawSensitivity, -maxAccumulation, maxAccumulation);
            rollOverFrames = Mathf.Clamp(rollOverFrames+roll*rollSensitivity, -maxAccumulation, maxAccumulation);
            thrustOverFrames = Mathf.Clamp(thrustOverFrames + thrust*thrustSensitivity, -maxAccumulation, maxAccumulation);
            verticalOverFrames = Mathf.Clamp(verticalOverFrames+vertical*verticalSensitivity, -maxAccumulation, maxAccumulation);
            lateralOverFrames = Mathf.Clamp(lateralOverFrames+lateral*lateralSensitivity, -maxAccumulation, maxAccumulation);
        }

        ApplyRotationVelChanges();

        ApplyLinearVelChanges();
    }
}
