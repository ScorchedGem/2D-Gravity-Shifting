using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{    
    // A cool curve we can use to change the rotation speed based on how close to the new surface it is (0 - basically touching, 1 - super far away)
    public AnimationCurve GravCorrectionSpeed;
    private Vector3 orientationNormal;
    private float floorDistance;

    // This is the multiplying speed we use to rotate faster
    public float MaxCorrectionSpeed = 20f;
    // This value is the distance our speed calc's cap at, any further than this value from the surface and it's the same speed
    public float gravCorrectionDist = 10f;

    public float moveSpeed = 5f;
    public float jumpForce = 6f;

    // So we know if the rotations need to be redone
    private bool floorChanged = false;
    private bool isGrounded = false;
    // What it says on the box
    private Transform playerTransform;
    // Sow we can ignore the player and enemy colliders in grounded tests
    public LayerMask worldLayers;

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the player transform
        playerTransform = GetComponent<Transform>();
        // Get a initial reference to the scene gravity settings
        orientationNormal = Physics2D.gravity.normalized;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Change global gravity
            Physics2D.gravity = new Vector3(-9.81f, 0, 0);
            // Save locally for references just incase
            orientationNormal = Physics2D.gravity.normalized;
            // Tell the component it's no longer grounded and needs to do rotations
            floorChanged = true;
        }

        // Check if they're on the floor (to help with falling off platforms etc.
        CheckGrounded();

        // If we were rotating but now we've hit the floor, stop rotating to ground and just set it
        if (floorChanged) { floorChanged = !isGrounded; Debug.Log("grounded"); }

        // Get movement input and convert it to the players local rotation
        Vector2 moveDelta = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 calcLocalVel = transform.TransformDirection(moveDelta * moveSpeed * Time.deltaTime);

        // Jump if we're on the ground
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Just a quick set incase something later on needs to ask
            isGrounded = false;
            // Add force in the global gravity direction we saved before
            calcLocalVel += (Vector2)(-orientationNormal) * jumpForce;
        }

        // Apply velocities calculated from inputs
        GetComponent<Rigidbody2D>().velocity += calcLocalVel;

        // If the gravity changed, do rotations to correct for orientation
        if (!floorChanged) floorDistance = 0;
        CorrectRotation();
    }

    // So you know what's going on with this stuff
    // Forward = blue
    // Left = red
    // Up = green

    private Quaternion LookRotation(Vector3 forward, Vector3 up)
    {
        // We want to rotate the obj to look toward the camera, and to rotate to face the floor at the same time
        Quaternion zUp = Quaternion.LookRotation(forward, up);
        return zUp;
    }

    private void CorrectRotation()
    {
        // Get new floor and front rotation
        Quaternion desiredRot = LookRotation(Camera.main.transform.forward, -orientationNormal);

        // Calculate the speed we want to rotate by how much time we have to do it
        float strength = Time.deltaTime * (MaxCorrectionSpeed * GravCorrectionSpeed.Evaluate(Mathf.Clamp(floorDistance, 0, gravCorrectionDist) / gravCorrectionDist));

        // Apply the new Slerp'd rotation
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, desiredRot, strength);
    }

    private bool CheckGrounded()
    {
        // Uhh... Check if its on the ground
        float playerWidth = 0.55f;
        Debug.DrawLine(transform.position, transform.position + orientationNormal * playerWidth);
        return isGrounded = Physics2D.Raycast((Vector2)transform.position, orientationNormal, playerWidth, worldLayers);
    }
}
