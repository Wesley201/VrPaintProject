using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public partial class HandTracking : MonoBehaviour {

    void BrushSizePressed(InteractionSourcePressedEventArgs obj)
    {
        
    }

    void BrushSizeReleased(InteractionSourceReleasedEventArgs obj)
    {

    }

    void BrushSizeUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        if (obj.state.touchpadTouched && !m_Drawing)
        {
            Vector2 dir = obj.state.touchpadPosition.normalized;
            float scale_dir = 0.0f;

            scale_dir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? Mathf.Sign(dir.x) : Mathf.Sign(dir.y);

            brushSize = Mathf.Clamp(brushSize + scale_dir * m_BrushSizeRate * deltaTime, m_BrushMinSize, m_BrushMaxSize);
        }
    }
    
}
