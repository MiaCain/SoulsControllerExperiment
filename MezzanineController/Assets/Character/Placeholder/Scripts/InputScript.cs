using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class InputScript : MonoBehaviour
{
    public static InputScript i;


    [Header("Input Data")]
    public float timeToRoll = 0.3f;
    public float rollTimer;
    public bool rollTimerTicking;
    public float timetoJumpRoll = 2.3f;
    public float jumpRollTimer;
    public bool jumpRollTimerTicking;

    [Header("Movement Locks")]
    public bool isWalking;
    public bool isRunning;
    public bool isRolling;
    public bool isIdle;
    public bool isJumprolling;
    public bool isBackrolling;
    public bool canWalk;
    public bool canMove;
    public bool isLockedOn;

    [Header("Realtime Data")]
    public GameObject LockedEnemy;
    public Cinemachine.CinemachineTargetGroup targetGroup;

    [Header("Player Data")]
    public Animator HumanAnimator;
    public int twoHSwordAnimLayer;
    public Cinemachine.CinemachineBrain cineBrain;
    public GameObject TPCam;
    public GameObject LockCam;


    [Header("Events")]
    public UnityEvent roll_Event;
    public UnityEvent runStart_Event;
    public UnityEvent runStop_Event;
    public UnityEvent jumpRoll_Event;
    public UnityEvent backdodge_Event;

    //Attack events
    public UnityEvent unsheathe_Event;

    public UnityEvent recentre3pCam_Event;

    private void Awake()
    {
        //set Singleton
        if (i != null && i != this)
        {
            Destroy(this);
        }
        else
        {
            i = this;
        }

        //set Events
        if (roll_Event == null) { roll_Event = new UnityEvent(); }
        if (runStart_Event == null) { runStart_Event = new UnityEvent(); }
        if (runStop_Event == null) { runStop_Event = new UnityEvent(); }
        if (recentre3pCam_Event == null) {recentre3pCam_Event = new UnityEvent(); }
        if (jumpRoll_Event == null) { jumpRoll_Event = new UnityEvent(); }
        if (backdodge_Event == null) { backdodge_Event = new UnityEvent(); }
        if (unsheathe_Event == null) { unsheathe_Event = new UnityEvent(); }
    }


    //Look for inputs. Trigger the relevant events depending on the button press
    private void Update()
    {
        if (Input.GetButtonDown("Run") && canMove && !rollTimerTicking)
        {
            rollTimer = 0f;
            rollTimerTicking = true;
        }

        // Count up the roll timers
        if (rollTimerTicking) { rollTimer += Time.deltaTime; }
        if (jumpRollTimerTicking) { jumpRollTimer += Time.deltaTime;}

        //Run button is being held for longer than the roll timer
        if (Input.GetButton("Run") && rollTimer >= timeToRoll && canMove)
        {
            //Debug.Log("Run button has been held for " + rollTimer);
            rollTimerTicking = false;
            rollTimer = 0f;
            runStart_Event.Invoke();
        }

        //Actions to take if runbutton is released
        if (Input.GetButtonUp("Run") && roll_Event != null && runStop_Event != null && jumpRoll_Event != null && backdodge_Event != null)
        {
            Debug.Log("canMove = " + canMove);
            Debug.Log("canWalk = " + canWalk);
            Debug.Log("isIdle = " + isIdle);
            Debug.Log("isRolling = " + isRolling);
            if (jumpRollTimerTicking && canMove && !isRolling && !isJumprolling && !isBackrolling)  //jump roll
                { jumpRoll_Event.Invoke(); jumpRollTimer = 0f; jumpRollTimerTicking = false; }

            if (rollTimerTicking && canMove && !isRolling && !isJumprolling && !isBackrolling && !isIdle && isWalking)  //regular roll forward
            { roll_Event.Invoke(); rollTimer = 0f; rollTimerTicking = false; }
           
            else if (rollTimerTicking && canMove && !isRolling && !isJumprolling && !isBackrolling && isIdle && !isWalking) //back dodge
                { backdodge_Event.Invoke(); rollTimer = 0f; rollTimerTicking = false; }
            
            else if (!rollTimerTicking && isRunning && !isRolling)  //stop running
                { runStop_Event.Invoke(); jumpRollTimer = 0f; jumpRollTimerTicking = true; }

            else  //Emergency case? 
            { Debug.Log("Stop running FULL STOP"); rollTimer = 0f; rollTimerTicking = false; isRunning = false; }
        }

        //After stopping run, you can use these to cycle into a running jump
        if (jumpRollTimer >= timetoJumpRoll && jumpRollTimerTicking) {jumpRollTimer = 0f; jumpRollTimerTicking = false; }
        if (rollTimer >= timeToRoll && rollTimerTicking) { rollTimer = 0f; rollTimerTicking = false; }

        //Light attack
        if (Input.GetButtonUp("RB"))
        {
            HumanAnimator.SetLayerWeight(twoHSwordAnimLayer, 1);
            unsheathe_Event.Invoke();
        }


        //mouse focus
        if (Input.GetButtonUp("Focus")) { Debug.Log("Focus");
            //recentre3pCam_Event.Invoke();
            isLockedOn = !isLockedOn;
            TPCam.SetActive(!isLockedOn);
            LockCam.SetActive(isLockedOn);
            if (isLockedOn && targetGroup.m_Targets[1].target != LockedEnemy.transform) { targetGroup.m_Targets[1].target = LockedEnemy.transform; }
            }
    }

}
