using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public partial class HandTracking : MonoBehaviour {

    void PaintPressed(InteractionSourcePressedEventArgs obj)
    {
        m_Drawing = true;

        obj.state.sourcePose.TryGetPosition(out m_HandPos);
        obj.state.sourcePose.TryGetForward(out m_HandFor);
        CreateTrailRenderer(brushPosition);
    }

    void PaintReleased(InteractionSourceReleasedEventArgs obj)
    {
        m_Drawing = false;

        obj.state.sourcePose.TryGetPosition(out m_HandPos);
        EndTrail(m_HandPos);
    }

    void PaintUpdated(InteractionSourceUpdatedEventArgs obj)
    {

    }
    
}
