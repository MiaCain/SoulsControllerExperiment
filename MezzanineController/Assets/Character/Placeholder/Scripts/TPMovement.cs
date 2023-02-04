using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TPMovement : MonoBehaviour
{
    public CharacterController ctrl;
    public Transform cam;

    [Header("Movement Values")]
    public float speed = 4f;
    public float runSpeed = 6f;
    bool isSlowWalking;
    public float curSpeed;
    public float rollSpeed;
    public float jumpRollSpeed;
    public float backRollSpeed;
    Vector3 rollDir;
    public float turnSmoothTime = 0.3f;
    float turnSmoothVelocity;
    public Vector3 faceDirection;

    [Header("Gravity Values")]
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float fallMultiplier = 2.5f;

    [Header("Grounded Data")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Animation Data")]
    public Animator anm;
    public FootIKSmooth footIK;

    Vector3 velocity;
    public bool isGrounded;

    private void Awake()
    {
        //Subscribe to Input Events
        InputScript.i.roll_Event.AddListener(RollStart);
        InputScript.i.runStart_Event.AddListener(RunStart);
        InputScript.i.runStop_Event.AddListener(RunStop);
        InputScript.i.jumpRoll_Event.AddListener(JumpRollStart);
        InputScript.i.backdodge_Event.AddListener(BackDodgeStart);
        //Attack scripts
        InputScript.i.unsheathe_Event.AddListener(Unsheathe);
    }

    void Update()
    {
        //check if standing
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0) { velocity.y = -2f;}

        //make sure anm bool are set correctly
        if (anm.GetBool("grounded") != isGrounded) { anm.SetBool("grounded", isGrounded); }
        if (InputScript.i.isRolling || InputScript.i.isJumprolling || InputScript.i.isBackrolling)
        { anm.SetBool("rolling", true); }
        else { anm.SetBool("rolling", false); }
        if(anm.GetBool("canMove") != InputScript.i.canMove) { anm.SetBool("canMove", InputScript.i.canMove); }
        if (anm.GetBool("canWalk") != InputScript.i.canWalk) { anm.SetBool("canWalk", InputScript.i.canWalk); }
        //if (anm.GetBool("still") != InputScript.i.isIdle) { InputScript.i.isIdle = anm.GetBool("still"); }
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //send data to walk function
        IsCharSlowWalking(horizontal, vertical);
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        velocity.y += gravity * Time.deltaTime;
        if (!isGrounded) { velocity.y += gravity * (fallMultiplier - 1) * Time.deltaTime; }
        ctrl.Move(velocity * Time.deltaTime);

        //Inputs
        if(Input.GetButtonDown("Jump") && isGrounded) { isGrounded = false;  velocity.y = Mathf.Sqrt(jumpHeight * 2 * -2f * gravity); }

        //Lockon lookat
        if (InputScript.i.isLockedOn && InputScript.i.canWalk && !InputScript.i.isRunning)
        {
            transform.LookAt(InputScript.i.LockedEnemy.transform, Vector3.up);
        }

        //walking and running input
        if (direction.magnitude >= 0.1f && InputScript.i.canWalk && InputScript.i.canMove && isGrounded) {

            //send walking to animator
            InputScript.i.isWalking = true;
            InputScript.i.isIdle = false;
            if (anm.GetBool("still")) { anm.SetBool("still", false); }

            //Calculate movement direction
            Vector3 moveDir = CurInputDir(direction, turnSmoothTime);

            if (InputScript.i.isRunning) { ctrl.Move(moveDir.normalized * runSpeed * Time.deltaTime); }
            else { ctrl.Move(moveDir.normalized * curSpeed * Time.deltaTime); }
        }

        else if (direction.magnitude < 0.1f && isGrounded && InputScript.i.canWalk)
        {
            InputScript.i.isWalking = false;
            InputScript.i.isIdle = true;
            if (!anm.GetBool("still")) { anm.SetBool("still", true); }
            if(InputScript.i.isRunning) { InputScript.i.runStop_Event.Invoke(); }
            InputScript.i.isRunning = false;
            
        }

        if (InputScript.i.isRolling) { ctrl.Move(rollDir.normalized * rollSpeed * Time.deltaTime); }
        if (InputScript.i.isJumprolling) { ctrl.Move(rollDir.normalized * jumpRollSpeed * Time.deltaTime); }
        if (InputScript.i.isBackrolling) { ctrl.Move(rollDir.normalized * backRollSpeed * -1f * Time.deltaTime); }
    }

    void IsCharSlowWalking(float hor, float ver)
    {
        //check to see the speed of the character's walk
        float walkStr = (Mathf.Abs(hor) + Mathf.Abs(ver) / 2);
        if (walkStr > 0.35f) { isSlowWalking = false; curSpeed = speed; anm.SetBool("fWalking", true);}
        else if (walkStr <= 0.35f && walkStr >= 0){ isSlowWalking = true; curSpeed = speed * walkStr; anm.SetBool("fWalking", false); }
    }

    // Movement Functions
    public void RollStart()
    {
        rollDir = CurInputDir(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")), 0f);
        InputScript.i.isRolling = true;
        //Debug.Log("Roll!");
        anm.SetBool("still", false);
        InputScript.i.isIdle = false;
        anm.SetTrigger("roll");
        //isGrounded = false; velocity.y = Mathf.Sqrt(2 * -2f * gravity);
        InputScript.i.canWalk = false;
    }

    public void JumpRollStart()
    {
        rollDir = CurInputDir(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")), 0f);
        InputScript.i.isJumprolling = true;
        Debug.Log("Jump roll!");
        anm.SetBool("still", false);
        InputScript.i.isIdle = false;
        anm.SetTrigger("jumproll");
        isGrounded = false; velocity.y = Mathf.Sqrt(2.3f * -2f * gravity);
        InputScript.i.canWalk = false;
    }

    public void BackDodgeStart()
    {
        if (InputScript.i.isIdle)
        {
            rollDir = CurInputDir(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")), 0f);
            InputScript.i.isBackrolling = true;
            Debug.Log("Backdodge!");
            anm.SetBool("still", false);
            InputScript.i.isIdle = false;
            anm.SetTrigger("backdodge");
            isGrounded = false; velocity.y = Mathf.Sqrt(1f * -2f * gravity);
            InputScript.i.canWalk = false;
        }
        else { RollStart(); }
    }

    public void RunStart()
    {
        Debug.Log("Run!");
        anm.SetBool("running", true);
        InputScript.i.isRunning = true;
    }

    public void RunStop()
    {
        Debug.Log("Stop Running!");
        anm.SetBool("running", false);
        InputScript.i.isRunning = false;
    }

    public void Unsheathe()
    {
        anm.SetTrigger("DrawSword");
    }

    Vector3 CurInputDir(Vector3 direction, float smoothTime)
    {    
        //Fetch current direction
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        faceDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        return faceDirection;
    }
}
