using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class HandTracking : MonoBehaviour
{
    /// Objects for tracking the hands
    public GameObject m_PaintingHand;
    public GameObject m_PaletteHand;

    /// Variables for Hand Tracking
    private Vector3 m_HandPos;
    private Vector3 m_HandFor;

    private float Offset = 3f;
    public bool m_Drawing = false;
    public float m_SelectPressedAmount = 0f;

    public TrailRenderer m_TrailRenderer;
    public float startLineWidth = 0.01f;
    public float endLineWidth = 0.01f;
    public bool ErrorShaderTest = false;
    float timeSinceLastUpdate = 0;
    float minTimeSinceLastUpdate = 0.1f;
    
    GameObject m_Cam;
    Vector3 TrailPos;

    public GameObject PalleteColor;

    void Start ()
    {
        /// Events to listen to to track the controllers
        InteractionManager.InteractionSourceDetected += InteractionManager_SourceDetected;
        InteractionManager.InteractionSourceUpdated += InteractionManager_SourceUpdated;
        InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;

        m_Cam = Camera.main.gameObject;

        InteractionManager.GetCurrentReading();
    }
    void Update ()
    {
		
	}

    /// Event Args for the Interaction Manager Events
    private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        m_Drawing = false;

        obj.state.sourcePose.TryGetPosition(out m_HandPos);
        EndTrail(m_HandPos);
    }

    private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs obj)
    {
        m_Drawing = true;

        obj.state.sourcePose.TryGetPosition(out m_HandPos);
        TrailPos = m_Cam.transform.forward * 2 + m_HandPos;
        Trail(TrailPos);
        m_TrailRenderer.transform.position = TrailPos;

        //If user is not hovering on a pallet color...
        if (PalleteColor != null)
        {
            //Run ChangeColor method in ColorPicker script
            ColorPicker.ChangeColor();
        }
    }

    private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
    {
        m_Drawing = false;

        obj.state.sourcePose.TryGetPosition(out m_HandPos);
        EndTrail(m_HandPos);
    }

    private void InteractionManager_SourceUpdated(InteractionSourceUpdatedEventArgs state)
    {
        InteractionSourcePose statePose = state.state.sourcePose;

        statePose.TryGetPosition(out m_HandPos, InteractionSourceNode.Pointer);
        statePose.TryGetForward(out m_HandFor, InteractionSourceNode.Pointer);

        if (state.state.source.handedness == InteractionSourceHandedness.Right)
        {
            m_PaintingHand.transform.position = m_HandPos;

            if (m_Drawing)
            {
                TrailPos = m_Cam.transform.forward * 2 + m_HandPos;
                m_TrailRenderer.transform.position = TrailPos;
            }
        }

        if (state.state.source.handedness == InteractionSourceHandedness.Left)
        {
            m_PaletteHand.transform.position = m_HandFor;
        }

        m_SelectPressedAmount = state.state.selectPressedAmount;
    }

    private void InteractionManager_SourceDetected(InteractionSourceDetectedEventArgs state)
    {
        //state.state.sourcePose.TryGetPosition(out m_HandPos);
        //TrailPos = m_Cam.transform.forward * 2 + m_HandPos;
        //Trail(TrailPos);
        //m_TrailRenderer.transform.position = TrailPos;
    }

    void Trail(Vector3 startPos)
    {
       m_TrailRenderer = new GameObject("Trail").AddComponent<TrailRenderer>();

       m_TrailRenderer.material = ColorPicker.brushColor;
       if (ColorPicker.brushColor == null)      
        {
            //Throws error if somehow the brush material has not been set in ColorPicker.cs
            Debug.LogError("ERROR: Brush color is null. Has not been set in ColorPicker.cs");
        }
        m_TrailRenderer.startWidth = startLineWidth;
        m_TrailRenderer.endWidth = endLineWidth;
        m_TrailRenderer.transform.position = startPos;
    }

    // Stops drawing the created line
    void EndTrail(Vector3 endPos)
    {
        Vector3 v = m_HandPos * 2 + endPos;
        timeSinceLastUpdate = 0;
    }

    //Color Pallete management
  void OnCollisionEnter(Collision collisionObj)
    {
        //If drawing hand collides with an object tagged PalleteColor
        if (collisionObj.gameObject.CompareTag("PalleteColor"))
        {
            //Store the game object into PalleteColor for reference
            PalleteColor = collisionObj.gameObject;
            //Retrieve the material of collisionObj and store it in the ColorPicker script's variable "selectedPalleteMaterial"
            ColorPicker.selectedPalleteMaterial = collisionObj.gameObject.GetComponent<Renderer>().material;
        }
    }
}
