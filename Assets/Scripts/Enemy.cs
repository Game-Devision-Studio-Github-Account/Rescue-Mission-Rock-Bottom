using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteTools st;
    public CircleCollider2D cc;
    public SlopeRotation sr;
    public Hitbox hb;

    public enum State {
        Idle,
        Chase,
        Attack,
        Fall,
    }

    public State state;

    [Header("AI")]
    public bool checkEdges = true;
    public bool active = true;

    [Header("Movement")]
    public float speed;
    //public float height = 1;
    public LayerMask groundMask;
    Vector2 groundNormal;
    public bool flipDirection;
    public float maxAngle = 30f;
    public float groundDifference;

    

    [Header("Imminent Fall Detection")]
    public float aheadCheckDistance = 5f;
    public float aheadCheckHeight = 5f;

    [Header("Death")]
    public GameObject drop;


    // Start is called before the first frame update
    void Start()
    {
        if (rb == null) GetComponent<Rigidbody2D>();

        state = State.Chase;

        if (st == null) {
            st = GetComponentInChildren<SpriteTools>();
        }

        if (cc == null) {
            cc = GetComponent<CircleCollider2D>();
        }

        if (sr == null) {
            sr = GetComponent<SlopeRotation>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        st.flipped = !flipDirection;
    }

    void FixedUpdate() {
        if (!active) return;
        /*Vector2 playerPosition = PlayerMovement.PlayerGO.transform.position;
        Vector2 movementDirection = ((Vector3)playerPosition - (Vector3)transform.position).normalized;

        rb.MovePosition((Vector2)transform.position + (movementDirection * speed * 0.01f));*/

        if (!CheckGround()) {
            state = State.Fall;
        }

        rb.isKinematic = !(state == State.Fall);

        switch(state) {
            case State.Fall:
                if (CheckGround()) {
                    state = State.Idle;
                } 
                
                break;
            case State.Idle:
                sr.RotateFloor(groundNormal);
                float angle = Vector3.Angle(groundNormal, Vector3.up);

                bool shouldFlip = false;
                
                if (angle >= maxAngle) shouldFlip = true;
                if (!CheckGroundAhead() && checkEdges) shouldFlip = true;

                if (shouldFlip) flipDirection = !flipDirection;

                int flipMod = flipDirection ? -1 : 1;

                Vector3 movement = (Vector3)Vector2.Perpendicular(groundNormal) * 0.1f * (float)flipMod;
                
                RaycastHit2D[] col = new RaycastHit2D[0];

                ContactFilter2D cf2d = new ContactFilter2D();
                cf2d.useTriggers = false;
                cf2d.SetLayerMask(groundMask);
                cf2d.useLayerMask = true;


                rb.Cast(movement, cf2d, col, movement.magnitude);
                

                
                
                if (col.Length > 0) {
                    Debug.Log("STUCK.");
                } else {
                    rb.MovePosition(movement + transform.position + (Vector3.down * groundDifference));
                }

                //Debug.Log(col);
                
                
                break;
        }
        
    }

    public bool CheckGround(){
        float gcOffset = cc.radius - cc.offset.y;
        RaycastHit2D groundHit = Physics2D.Raycast(rb.position, Vector2.down, gcOffset + 0.1f, groundMask);
        groundNormal = groundHit.normal;

        groundDifference = groundHit.distance - gcOffset;

        //Debug.Log(a);
        //if(groundHit) transform.position -= (Vector3.up * a);
        //if(groundHit) rb.MovePosition(rb.position - (Vector2.up * a));

        //Debug.Log(gcOffset.ToString() + ' ' + a.ToString());// + ' ' + ((bool)groundHit).ToString());

        return groundHit;
    }

    public bool CheckGroundAhead(){
        int flipMod = flipDirection ? -1 : 1;
        Vector3 p = rb.position + (Vector2.Perpendicular(groundNormal) * (float)flipMod * aheadCheckDistance);
        RaycastHit2D groundHit = Physics2D.Raycast(p, Vector2.down, aheadCheckHeight, groundMask);
        return groundHit;
    }

    public void Activate() {
        active = true;
    }

    public void Die() {
        if (drop != null) {
            GameObject d = Instantiate(drop);
            d.transform.position = transform.position;
        }
        Destroy(gameObject);
    }
}
