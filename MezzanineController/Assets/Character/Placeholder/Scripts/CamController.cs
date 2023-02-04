using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public CinemachineFreeLook freelook;
    public Transform SnapAxis;
    public Transform SnapRot;
    private void Start()
    {

        Recenter();
    }

    private void Awake()
    {
        InputScript.i.recentre3pCam_Event.AddListener(Recenter);
        Recenter();
    }

    void Recenter()
    {
        freelook.ForceCameraPosition(SnapAxis.position, SnapRot.rotation);
    }

}
