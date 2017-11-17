using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public partial class HandTracking : MonoBehaviour {

    void ErasePressed(InteractionSourcePressedEventArgs obj)
    {

    }

    void EraseReleased(InteractionSourceReleasedEventArgs obj)
    {
        DestroyAllTrails();
    }

    void EraseUpdated(InteractionSourceUpdatedEventArgs obj)
    {

    }
    
}
