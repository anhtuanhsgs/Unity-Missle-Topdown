using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Common.Misc;
using Assets.Scripts.Steering;


public class MovingAgent : MonoBehaviour {

    //Moving attributes
    public float maxSpeed;
    public float minSpeed;
    public float velocity;
    public float accelerationRate;
    public float moveForce;

    //Limit speed will be updated regularly
    protected float limitSpeed;

    //Turning attributes
    public float targetAngle;
    public float turnRate;
    protected bool turningRight;

    //Reference to components of the gameObjet
    protected Transform _transform;
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;
    protected SpriteRenderer _renderer;

    protected float boundingRadius;

    protected SteeringBehaviors steeringBehavior;

    public LayerMask obstDef;
    public float obsFeelerLen;

    bool firing = false;

    //Other agents references
    public List<MovingAgent> neighbors;

    // Use this for initialization
    protected virtual void Start () {
        _transform = gameObject.GetComponent<Transform> ();

        _rigidbody = gameObject.GetComponent<Rigidbody2D> ();
        if (_rigidbody == null) Debug.LogError ("Missing rigidbody");

        _renderer = gameObject.GetComponent<SpriteRenderer> ();
        if (_renderer == null) Debug.LogError ("Missing renderer");

        steeringBehavior = new SteeringBehaviors (this);
    }

    // Update is called once per frame
    protected virtual void Update () {
        UpdateActualSpeedAndAngle ();
    }

    protected virtual void UpdateActualSpeedAndAngle () {
        UpdateActualSpeed ();
        UpdateActualAngle ();
    }

    protected virtual void UpdateActualSpeed () {
        /*
        if (velocity > limitSpeed) {
            velocity = limitSpeed;
        }
        else if (velocity < limitSpeed) {
            velocity += accelerationRate * Time.deltaTime;
            //TODO: change to using force
        }

        //Change on rigidbody
        _rigidbody.velocity = VectorHeading () * velocity;
        */
        /////////////

        //TODO: __If the speed is to high, add no moving force, explosion force is still valid
        //Using 2 separate functions, 2 separate variables to store ( Need to change directly affecting function in steering behavior)

        _rigidbody.AddForce (VectorHeading () * moveForce);

        velocity = _rigidbody.velocity.magnitude;
   
        //Clamp velocity
        if (velocity > limitSpeed) {
            velocity = limitSpeed;
            _rigidbody.velocity = _rigidbody.velocity.normalized * limitSpeed;
        }
    }

    protected virtual void UpdateActualAngle () {
        float deltaAngle = Utils.DeltaAngle (DegreeHeading (), targetAngle);

        //Change on transform
        float changeValue = turnRate * Time.deltaTime;

        if (Mathf.Abs (changeValue) > Mathf.Abs (deltaAngle)) {
            SetAngle (targetAngle);
        }
        else {
            if (deltaAngle < 0) {
                turningRight = true;
                ChangeAngle (-changeValue);
            }
            else if (deltaAngle > 0) {
                turningRight = false;
                ChangeAngle (changeValue);
            }
        }
    }

    public virtual void GetHit (float damage) {

    }

    public void SetNeighbors (List<MovingAgent> newNeighbors) {
        neighbors = newNeighbors;
    }

    public void AddForce (Vector2 force) {
        _rigidbody.AddForce (force);
    }

    public bool IsVisible () {
        return _renderer.isVisible;
    }

    public Renderer GetRenderer () {
        return _renderer;
    }

    public float MaxSpeed {
        set { maxSpeed = value; }
        get { return maxSpeed; }
    }

    public float MinSpeed {
        set { minSpeed = value; }
        get { return minSpeed; }
    }

    public float Velocity {
        set { velocity = value; }
        get { return velocity; }
    }

    public float AccelerationRate {
        set { accelerationRate = value; }
        get { return accelerationRate; }
    }

    public bool Firing {
        set { firing = value; }
        get { return firing; }
    }

    public float TurnRate {
        set { turnRate = value; }
        get { return turnRate; }
    }

    public void ChangeAngle (float value) {
        float newAngle = DegreeHeading () + value;
        SetAngle (newAngle);
    }

    public void SetAngle (float angle) {
        /*        Vector3 newEulerAngles = transform.rotation.eulerAngles;
                newEulerAngles.z = angle;
                Quaternion newRotation = _transform.rotation;
                newRotation.eulerAngles = newEulerAngles;
                _transform.rotation = newRotation;*/
        _transform.eulerAngles = new Vector3 (0, 0, angle);
    }

    public Vector2 Position {
        get { return _transform.position; }
    }

    public float DegreeHeading () {
        return transform.rotation.eulerAngles.z;
    }

    public Vector2 VectorHeading () {
        return transform.right;
    }

    public float BoundingRadius {
        get { return boundingRadius; }
    }

    public float GetBoundingRadius () {
        return boundingRadius;
    }

    public Vector2 FuturePosition (float lookAheadTime) {
        return Position + VectorHeading () * Velocity * lookAheadTime;
    }

    public void SetTargetAngle (float newTargetAng) {
        targetAngle = newTargetAng;
    }

    public float TargetAngle () {
        return targetAngle;
    }
}
