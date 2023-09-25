using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public Rigidbody2D rb;
    public CircleCollider2D cc;

    public enum State {
        //State when attached to ground or walls.
        Ground,
        //State when in free-fall.
        Air,
        //State when charging a jump.
        JumpCharge,
        //State when ground pounding.
        GroundPound
    }

    public State state;

    [Header("Input")]

    //The keys used to input left and right.
    public KeyCode leftInput = KeyCode.A;
    public KeyCode rightInput = KeyCode.D;

    //Stores which inputs are ACTUALLY left and right, in case the slime is upside down.
    KeyCode currentLeftInput;
    KeyCode currentRightInput;
    //The keys used to input Up and Down.
    public KeyCode upInput = KeyCode.W;
    public KeyCode downInput = KeyCode.S;
    //The Vector2 of the directional inputs. Set in Update.
    Vector2 directionalInput;  
    //The key needed to input Jump.
    public KeyCode jumpInput = KeyCode.Space;
    //The key needed to input a Ground Pound
    public KeyCode groundPoundInput = KeyCode.LeftShift;

    [Header("Locomotion")]
    //Movement speed on a surface.
    public float speed = 5f;
    //Movement speed in the air.
    public float airSpeed = 3f;
    //Which layer is the ground?
    public LayerMask groundMask;
    //Normal of the ground.
    public Vector2 groundNormal;
    //How far out to detect ground. If less than.
    public float groundCastRadius = 0.5f;
    public float groundMagnetism = 1;

    [Header("Jumping")]
    //Curve that defines the jump force 
    public AnimationCurve jumpForceCurve;
    public float jumpForceMultiplier;
    public float jumpForceFloor;
    //The time, in seconds, it takes for a jump to charge.
    public float jumpChargeTime;
    float jumpChargeAmount;

    [Header("Ground Pound")]
    public float groundPoundGravityScale = 2f;

    [Header("Visuals")]
    [Range(0.0f, 1.0f)]
    //Lerp speed for rotation. Too fast looks scuffed and jittery, too slow looks sluggish.
    public float rotationLerp = 0.2f;
    public TrailRenderer slimeTrail;


    // Start is called before the first frame update
    void Start()
    {
        //Checks for a Rigidbody2D on the GameObject itself if none is assigned in the inspector.
        if (rb == null) {
            rb = GetComponent<Rigidbody2D>();
        }

        if (cc==null) {
            cc = GetComponent<CircleCollider2D>();
        }

        //Defaults the state to Air since OnCollisionExit can't run on frame 1.
        state = State.Air;
    }

    // Update is called once per frame. Used to handle inputs.
    void Update()
    {
        //Checks if neither the left or right keys are being pressed.
        if (!(Input.GetKey(leftInput) || Input.GetKey(rightInput))) {
            currentLeftInput = leftInput;
            currentRightInput = rightInput;

            //Checks if the player is upside-down and on a surface.
            if (state == State.Ground && UpsideDown()) {
                currentLeftInput = rightInput;
                currentRightInput = leftInput;
            }
        }

        //Sets directionalInput every frame, so I don't have to bother with setting it.
        SetInputVector();


        if (state == State.JumpCharge && !Input.GetKey(jumpInput)) {
            Jump();
        }

        if (state == State.Ground && Input.GetKey(jumpInput)) {
            StartJumpCharge();
        }

        if (state == State.Air && Input.GetKey(groundPoundInput)) {
            GroundPound();
        }
    }

    //Creates a Vector2 from the players movement inputs.
    //I miss the new input system already.
    void SetInputVector() {
        directionalInput = Vector2.zero;

        if (Input.GetKey(currentLeftInput)) directionalInput += Vector2.left;
        if (Input.GetKey(currentRightInput)) directionalInput += Vector2.right;
        if (Input.GetKey(upInput)) directionalInput += Vector2.up;
        if (Input.GetKey(downInput)) directionalInput += Vector2.down;
    }
    
    //Runs 50 times per frame, no matter what. Eliminates the need for deltaTime.
    void FixedUpdate()
    {
        if (slimeTrail != null) slimeTrail.emitting = (state == State.Ground);

        switch(state) {
            case State.Ground:
            default:
                Magnetize();

                //Sets gravity to 0 so the slime doesn't fall away from walls.
                rb.gravityScale = 0;

                //Moves left or right along a given surface based on input.
                if (directionalInput.x != 0) {
                    rb.AddForce(Vector2.Perpendicular(groundNormal) * -directionalInput.x * speed);
                }

                /*rb.AddForce(-groundNormal);*/

                //Rotates so it appears to be "on the ground."
                RotateTowards(Quaternion.FromToRotation(Vector2.up, groundNormal));

                break;
            case State.Air:
                Magnetize();

                //Sets gravity to 1 to allow the slime to fall.
                rb.gravityScale = 1;

                //Moves left or right universally based on input.
                if (directionalInput.x != 0) {
                    rb.AddForce(Vector2.right * directionalInput.x * airSpeed);
                }
                
                //Rotates back to the default.
                RotateTowards(Quaternion.identity);

                break;
            case State.JumpCharge:
                jumpChargeAmount += (Time.fixedDeltaTime / jumpChargeTime);
                jumpChargeAmount = Mathf.Min (jumpChargeAmount, 1.0f);

                //Rotates so it appears to be "on the ground."
                RotateTowards(Quaternion.FromToRotation(Vector2.up, groundNormal));

                break;
            case State.GroundPound:
                rb.gravityScale = groundPoundGravityScale;
                break;
        }

        

        
    }

    public void Magnetize() {
        RaycastHit2D ray = Physics2D.CircleCast(rb.position, cc.radius + 0.01f + groundCastRadius, Vector2.zero, 0, groundMask);

        Vector2 n;

        if (ray) {
            n = ray.normal;

            RaycastHit2D ray2 = Physics2D.CircleCast(rb.position, cc.radius + 0.01f, Vector2.zero, 0, groundMask);

            if (ray2) {
                n = ray2.normal;
            }

            rb.AddForce(-n * groundMagnetism);
        }

        
    }

    /*current idea as a note to myself: a larger collider that basically magnetizes the slime to
    a nearby wall*/
    void OnCollisionStay2D(Collision2D col) {
        

        for(int i = 0; i < col.contactCount; i++) {
            //checks if the other game object is in the groundMask LayerMask
            //dont ask me to explain. as far as I am aware this works via magic.
            if (groundMask == (groundMask | 1 << col.GetContact(i).collider.gameObject.layer)) {
                groundNormal = col.GetContact(0).normal;

                if (state == State.JumpCharge) return;
                state = State.Ground;
            }
        }
        

        
        
    }

    //MAKE GROUND DETECTION BETTER THAN THIS.
    void OnCollisionExit2D(Collision2D col) {
        state = State.Air;
    }

    //Lerps the rotation towards a specific value based on the rotationLerp variable.
    public void RotateTowards(Quaternion targetRotation) {
        rb.SetRotation(Quaternion.Lerp(transform.rotation, targetRotation, rotationLerp));
    }

    public bool UpsideDown() {
        return (Vector2.Dot(transform.up, Vector2.down) > 0);
    }

    #region action_functions

    public void StartJumpCharge() {
        jumpChargeAmount = 0;
        
        state = State.JumpCharge;
    }

    public void Jump() {
        state = State.Air;

        Vector2 jumpDir = groundNormal;
        if (!directionalInput.Equals(Vector2.zero)) {
            jumpDir = directionalInput.normalized;

            if (UpsideDown()) jumpDir.x = -jumpDir.x;
        }
        rb.AddForce(jumpDir * (jumpForceFloor + (jumpForceCurve.Evaluate(jumpChargeAmount) * jumpForceMultiplier)), ForceMode2D.Impulse);
        
    }

    public void GroundPound() {
        state = State.GroundPound;
    }

    #endregion
}
