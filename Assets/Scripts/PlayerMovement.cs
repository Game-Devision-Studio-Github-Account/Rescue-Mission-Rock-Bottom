using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static GameObject PlayerGO;
    public Rigidbody2D rb;
    public CircleCollider2D cc;
    ScriptAnimator sa;
    public Animator anim;
    public SpriteRenderer sprite;

    public enum State {
        //State when attached to ground or walls.
        Ground,
        //State when in free-fall.
        Air,
        //State when charging a jump.
        JumpCharge,
        //State when ground pounding.
        GroundPound,
        //State when attacking
        Attack
    }

    public State state;

    [Header("Input")]

    //The keys used to input left and right.
    public KeyCode leftInput = KeyCode.LeftArrow;
    public KeyCode rightInput = KeyCode.RightArrow;

    //Stores which inputs are ACTUALLY left and right, in case the slime is upside down.
    KeyCode currentLeftInput;
    KeyCode currentRightInput;
    //The keys used to input Up and Down.
    public KeyCode upInput = KeyCode.UpArrow;
    public KeyCode downInput = KeyCode.DownArrow;
    //The Vector2 of the directional inputs. Set in Update.
    Vector2 directionalInput;  
    Vector2 trueDirectionalInput;
    //The key needed to input Jump.
    public KeyCode jumpInput = KeyCode.Z;
    //The key needed to input a Ground Pound
    public KeyCode groundPoundInput = KeyCode.C;
    //The key needed to input an Attack
    public KeyCode attackInput = KeyCode.X;

    [Header("Locomotion")]
    //Movement speed on a surface.
    public float speed = 5f;
    //Movement speed in the air.
    public float airSpeed = 3f;
    //Which layer is the ground?
    public LayerMask groundMask;
    //The amount of objects in the groundMask layer(s) that the slime is colliding with.
    int groundCount = 0;
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
    bool jump = false;
    //The time, in sections, to manually reset the jump variable if the ground has not been left.
    public float jumpResetTime = 0.1f;
    float jumpResetTimer;
    [Header("Attacking")]
    public float attackDuration = 0.1f;
    float attackTimer;
    public float attackCooldown = 0.1f;
    public float attackCooldownTimer;
    bool canAttack;

    [Header("Ground Pound")]
    public float groundPoundGravityScale = 2f;

    [Header("Visuals")]
    [Range(0.0f, 1.0f)]
    //Lerp speed for rotation. Too fast looks scuffed and jittery, too slow looks sluggish.
    public float rotationLerp = 0.2f;
    public TrailRenderer slimeTrail;

    [Header("Animation")]
    public string idleAnim = "Idle";
    public string jumpChargeAnim = "JumpCharge";
    public string groundPoundAnim = "GroundPound";
    public string attackAnim = "Attack";


    void Awake() {
        PlayerGO = gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        //Checks for a Rigidbody2D on the GameObject itself if none is assigned in the inspector.
        if (rb == null) {
            rb = GetComponent<Rigidbody2D>();
        }

        if (cc == null) {
            cc = GetComponent<CircleCollider2D>();
        }

        if (anim == null) {
            anim = GetComponent<Animator>();

            if (anim == null) {
                Debug.LogError("Assign an Animator to PlayerMovement.");
            }
        }

        sa = new ScriptAnimator(anim);

        if (sprite == null) {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        //Defaults the state to Air since OnCollisionExit can't run on frame 1.
        state = State.Air;

        attackCooldownTimer = 0f;
    }

    // Update is called once per frame. Used to handle inputs.
    void Update()
    {
        //Checks if neither the left or right keys are being pressed.
        if (!(Input.GetKey(leftInput) || Input.GetKey(rightInput))) {
            AdjustHorizontalInput();
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

        attackCooldownTimer -= Time.deltaTime;
        canAttack = (attackCooldownTimer <= 0);

        if (state == State.Ground && Input.GetKey(attackInput) && canAttack) {
            Attack();
        }
    }

    void AdjustHorizontalInput() {
        currentLeftInput = leftInput;
        currentRightInput = rightInput;

        //Checks if the player is upside-down and on a surface.
        if (state == State.Ground && UpsideDown()) {
            currentLeftInput = rightInput;
            currentRightInput = leftInput;
        }
    }

    //Creates a Vector2 from the players movement inputs.
    //I miss the new input system already.
    void SetInputVector() {
        directionalInput = Vector2.zero;
        trueDirectionalInput = Vector2.zero;

        if (Input.GetKey(currentLeftInput)) directionalInput += Vector2.left;
        if (Input.GetKey(currentRightInput)) directionalInput += Vector2.right;
        if (Input.GetKey(upInput)) directionalInput += Vector2.up;
        if (Input.GetKey(downInput)) directionalInput += Vector2.down;

        if (Input.GetKey(leftInput)) trueDirectionalInput += Vector2.left;
        if (Input.GetKey(rightInput)) trueDirectionalInput += Vector2.right;
        if (Input.GetKey(upInput)) trueDirectionalInput += Vector2.up;
        if (Input.GetKey(downInput)) trueDirectionalInput += Vector2.down;
    }
    
    //Runs 50 times per frame, no matter what. Eliminates the need for deltaTime.
    void FixedUpdate()
    { 
        if (jump) {
            jumpResetTimer -= Time.fixedDeltaTime;
            if (jumpResetTimer <= 0) jump = false;
        }

        if (state == State.Attack) {
            attackTimer -= Time.fixedDeltaTime;
            if (attackTimer <= 0) state = State.Ground;
        }
        
       

        if (groundCount == 0) {
            if (state == State.GroundPound) return;
            
            if (state == State.Ground || state == State.JumpCharge) {
                //RaycastHit2D gc = Physics2D.CircleCast(rb.position + (rb.velocity * Time.fixedDeltaTime), cc.radius + 0.01f + groundCastRadius, Vector2.zero, 0, groundMask);
                RaycastHit2D gc = Physics2D.CircleCast(rb.position, cc.radius + 0.01f + groundCastRadius, Vector2.zero, 0, groundMask);

                RaycastHit2D gl = Physics2D.Raycast(rb.position, -transform.up, cc.radius + 0.01f + groundCastRadius, groundMask);

                if (gl) {
                    groundNormal = gl.normal;
                } else if (gc) {
                    groundNormal = gc.normal;
                }
                

                if (gc == false) {
                    state = State.Air;
                    if (slimeTrail != null) slimeTrail.emitting = false;
                } else {
                    slimeTrail.emitting = true;
                    slimeTrail.transform.position = gl.point;
                    
                    //rb.MovePosition(gl.point - (-groundNormal * cc.radius));
                    
                    //rb.velocity = rb.velocity.magnitude * (Vector2.Perpendicular(groundNormal).normalized * ((Mathf.Abs(Vector2.Angle(rb.velocity, Vector2.Perpendicular(groundNormal))) < 90) ? 1 : -1));
                    rb.velocity = rb.velocity.magnitude * Vector2.Perpendicular(groundNormal).normalized * ((Mathf.Abs(Vector2.Angle(rb.velocity, Vector2.Perpendicular(groundNormal))) < 90) ? 1 : -1);

                    Magnetize();

                    RotateFloor();

                    state = State.Ground;
                }

                

                return;
            } else {
                state = State.Air;
                if (slimeTrail != null) slimeTrail.emitting = false;
            }

            /*if (groundCount == 0) {
                jump = false;
                state = State.Air;
                
                if (slimeTrail != null) slimeTrail.emitting = false;

            }*/
            
            
        } else {


            bool a = false;

            if (state != State.Ground) {
                a = true;
            }

            if (state != State.JumpCharge && state != State.Attack && !jump) {
                state = State.Ground;

                if (a) {
                    AdjustHorizontalInput();
                }
            }
            
        }

        switch(state) {
            case State.Ground:
            default:
                sa.SetState(idleAnim);

                if (directionalInput.x != 0) {
                    sprite.transform.localScale = new Vector2(
                        directionalInput.x > 0 ? 1 : -1,
                        sprite.transform.localScale.y
                    );
                }
                

                Magnetize();

                //Sets gravity to 0 so the slime doesn't fall away from walls.
                rb.gravityScale = 0;

                //Moves left or right along a given surface based on input.
                if (directionalInput.x != 0) {
                    rb.AddForce(Vector2.Perpendicular(groundNormal) * -directionalInput.x * speed);
                }

                //Rotates so it appears to be "on the ground."
                RotateFloor();

                break;
            case State.Air:
                sa.SetState(idleAnim);
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
                sa.SetState(jumpChargeAnim);
                jumpChargeAmount += (Time.fixedDeltaTime / jumpChargeTime);
                jumpChargeAmount = Mathf.Min (jumpChargeAmount, 1.0f);

                //Rotates so it appears to be "on the ground."
                RotateFloor();

                break;
            case State.GroundPound:
                sa.SetState(groundPoundAnim);
                rb.gravityScale = groundPoundGravityScale;
                break;
            case State.Attack:
                sa.SetState(attackAnim);
                rb.velocity = Vector2.zero;
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

                if (slimeTrail != null) {
                    slimeTrail.emitting = true;
                    slimeTrail.transform.position = col.GetContact(0).point;
                }

                //Scuffed temp variable to track if transition to the ground state.
                bool a = false;

                if (state != State.Ground) {
                    a = true;
                }

                if (!(state != State.JumpCharge && state != State.Attack && !jump)) return;
                state = State.Ground;

                if (a) {
                    AdjustHorizontalInput();
                }
            }
        }
        

        
        
    }

    void OnCollisionEnter2D(Collision2D col) {

        if (groundMask == (groundMask | 1 << col.collider.gameObject.layer)) {
            groundCount += 1;

            jump = false;
        }
    }

    void OnCollisionExit2D(Collision2D col) {
        

        if (groundMask == (groundMask | 1 << col.collider.gameObject.layer)) {
            groundCount -= 1;
        }
    }

    //Lerps the rotation towards a specific value based on the rotationLerp variable.
    public void RotateTowards(Quaternion targetRotation) {
        rb.SetRotation(Quaternion.Lerp(transform.rotation, targetRotation, rotationLerp));
    }

    public void RotateTowards(float angle) {
        rb.SetRotation(Mathf.Lerp(transform.rotation.eulerAngles.z, angle, rotationLerp));
        //rb.SetRotation(angle);
    }

    public void RotateFloor() {
        if (groundNormal.normalized.y != -1) {
            RotateTowards(Quaternion.FromToRotation(Vector2.up, groundNormal));
        } else {
            RotateTowards(180);
        }
    }

    public bool UpsideDown() {
        return (Vector2.Dot(groundNormal, Vector2.down) > 0);
    }

    #region action_functions

    public void StartJumpCharge() {
        jumpChargeAmount = 0;
        
        state = State.JumpCharge;
    }

    public void Jump() {
        jump = true;
        jumpResetTimer = jumpResetTime;
        state = State.Air;

        Vector2 jumpDir = groundNormal;
        if (!directionalInput.Equals(Vector2.zero)) {
            jumpDir = trueDirectionalInput.normalized;

            //if (UpsideDown()) jumpDir.x = -jumpDir.x;
        }

        rb.AddForce(jumpDir * (jumpForceFloor + (jumpForceCurve.Evaluate(jumpChargeAmount) * jumpForceMultiplier)), ForceMode2D.Impulse);
    }

    public void GroundPound() {
        state = State.GroundPound;
    }

    public void Attack() {
        attackTimer = attackDuration;
        attackCooldownTimer = attackCooldown;
        state = State.Attack;
    }

    #endregion
}

//code from DDP. idk how it works. math is magic to me.
//https://discussions.unity.com/t/whats-the-most-efficient-way-to-rotate-a-vector2-of-a-certain-angle-around-the-axis-orthogonal-to-the-plane-they-describe/98886
public static class Vector2Extension {
	
	public static Vector2 Rotate(this Vector2 v, float degrees) {
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
		
		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}
}