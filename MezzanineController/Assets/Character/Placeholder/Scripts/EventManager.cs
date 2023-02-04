using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public GameObject twoHSwordBack;
    public GameObject twoHSwordHand;

    public Animator anm;

    private void Awake()
    {
        anm = GetComponent<Animator>();
    }

    public void StopRoll()
    {
        InputScript.i.isBackrolling = false;
        InputScript.i.isJumprolling = false;
        InputScript.i.isRolling = false;
        InputScript.i.canWalk = true;
    }

    public void StopMove()
    {
        InputScript.i.canMove = false;
    }

    public void StartMove()
    {
        InputScript.i.canMove = true;
    }

    public void StopWalk()
    {
        InputScript.i.canWalk = false;
        InputScript.i.canMove = false;
    }

    public void StartWalk()
    {
        InputScript.i.canWalk = true;
        InputScript.i.canMove = true;
    }

    public void TwoHSwordDraw()
    {
        twoHSwordBack.SetActive(false);
        twoHSwordHand.SetActive(true);
    }

    public void TwoHSwordSheathe()
    {
        twoHSwordBack.SetActive(true);
        twoHSwordHand.SetActive(false);
    }

}
