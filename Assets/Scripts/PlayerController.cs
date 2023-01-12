using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private BoxCollider2D _bc2d;
    private SpriteRenderer _sr;
    private Animator _animator;
    private LayerMask _unJumpable;
    private float _extraHeight = .1f; // extra height added to GROUND raycast detection distance.
    private float _extraLength = .5f; // extra length added to WALLS raycast detection distance.

    private bool _wallJumpEnabled;
    
    private float _coyoteTimeCounter;  // coyote jump timer.
    private float _coyoteTimer = 0.2f; // available time for jumping after exiting edges.
    
    [SerializeField] private float playerSpeed = 6f;
    [SerializeField] float fallMultiplier = 4f;
    [SerializeField] float lowJumptMultiplier = 2f;
    [SerializeField] float jumpForce = 8f;
    
    private float _idleTimeCounter; // sit timer.
    private float _timeToSitTimer = 4f; // time before sitting animation starts to play.
    
    
    
    private static readonly int TimeToRun = Animator.StringToHash("timeToRun");
    private static readonly int TimeToSit = Animator.StringToHash("timeToSit");
    private static readonly int TimeToJump = Animator.StringToHash("timeToJump");
    private static readonly int IsFalling = Animator.StringToHash("isFalling");
    private static readonly int HaveFallen = Animator.StringToHash("haveFallen");


    // Start is called before the first frame update
    private void Start()
    {
        Cursor.visible = false;
        _rb = GetComponent<Rigidbody2D>();
        _bc2d = GetComponent<BoxCollider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _unJumpable = LayerMask.GetMask("Platform"); // player can't horizontally jump from out of this layer's objects.
    }

    // Update is called once per frame
    private void Update()
    {
        CheckWalk();
        CheckJump();
        if (IsGrounded()) return;
        Fall();
        
    }

    private void CheckWalk() // manages walking speed.
    {
        IdleStateMachine();
        RunStateMachine();
        var x     = Input.GetAxis("Horizontal");
        var y     = Input.GetAxis("Vertical");
        var dir = new Vector2(x, y);
        
        _rb.velocity = new Vector2(dir.x * playerSpeed, _rb.velocity.y);
        if (_rb.velocity.x < 0 )
        {
            _sr.flipX = true;
            if (CheckWalls(-1) && _rb.velocity.x != 0)
            {
                Debug.Log("in");
                _wallJumpEnabled = true;
                //animator
            }
        }
        else if (_rb.velocity.x > 0)
        {
            _sr.flipX = false;
            if (CheckWalls(1) && _rb.velocity.x != 0)
            {
                Debug.Log("in");
                _wallJumpEnabled = true;
                //animator
            }

        }
        
    }

    private void CheckJump()
    {
        if (IsGrounded()) { _coyoteTimeCounter = _coyoteTimer; }
        else { _coyoteTimeCounter -= Time.deltaTime; }
        if ((Input.GetButtonDown("Jump")))
        {
            _animator.SetTrigger(TimeToJump);
            Jump();
            _coyoteTimeCounter = 0f;
        }
    }
    
    private void Jump() // set jump force onto player.
    {
        _rb.velocity  = new Vector2(_rb.velocity.x, 0);
        _rb.velocity += Vector2.up * jumpForce;
        _wallJumpEnabled = false;
    }

    private void Fall() // increases falling speed depending on player input and current velocity.
    {
        if (_rb.velocity.y < -1)
        {
            _animator.SetTrigger(IsFalling);
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            OnLanding(); // raycasts down until x distance to ground, then plays LandStateMachine.
        }
        else if (_rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumptMultiplier - 1) * Time.deltaTime;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D groundBoxCastInfo =  Physics2D.BoxCast(_bc2d.bounds.center, _bc2d.bounds.size, 0f, Vector2.down, _extraHeight, _unJumpable);
        return groundBoxCastInfo.collider != null;
    }

    private void OnLanding()
    {
        Debug.Log("Landable");
        Debug.DrawRay(_bc2d.bounds.center, Vector3.down, Color.blue);
        if (Physics2D.Raycast(_bc2d.bounds.center, Vector2.down, 1.5f, _unJumpable))
        {
            Debug.Log("Land");
            _animator.SetTrigger(HaveFallen); // WORK HERE ON LANDING ANIMATION!!!!
        }
    }

    private bool CheckWalls(int flip)
    {
        RaycastHit2D wallCastInfo = Physics2D.Raycast(_bc2d.bounds.center, new Vector2(flip, 0), _extraLength, _unJumpable);
        Debug.DrawRay(_bc2d.bounds.center, new Vector2(flip, 0), Color.blue);
        //Debug.Log(wallCastInfo.collider != null);
        return wallCastInfo.collider != null;
        
    }

    private void RunStateMachine() // manages run animation.
    {
        if (Input.GetAxis("Horizontal") != 0 && _rb.velocity.y == 0)
        {
            _animator.SetBool(TimeToRun, true);
            return;
        }
        _animator.SetBool(TimeToRun, false);
        
    }
    private void IdleStateMachine() // manages idle/sit animation.
    {
        _idleTimeCounter += Time.deltaTime;
        if (Input.anyKeyDown)
        {
            _idleTimeCounter = 0;
            _animator.SetBool(TimeToSit, false);
        }
        else if (_idleTimeCounter > _timeToSitTimer)
        {
            _animator.SetBool(TimeToSit, true);
        }
        
    }

}
