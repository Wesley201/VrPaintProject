using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Tracking : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
    }
}
