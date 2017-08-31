using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class HandTracking : MonoBehaviour
{
    private GameObject m_Cam;

    /// Objects for tracking the hands
    public GameObject m_PaintingHand;
    public GameObject m_PaletteHand;

    /// Variables for Hand Tracking
    private Vector3 m_HandPos;
    private Vector3 m_HandFor;
    public Vector3 m_HandTrackingOffset;
    private Quaternion m_HandRot;

    private Vector3 TrailPos;

    private float Offset = 3f;
    public bool m_Drawing = false;
    public float m_SelectPressedAmount = 0f;

    //MORTY, I'M A PICKLE!

    public TrailRenderer m_TrailRenderer;
    public float startLineWidth = 0.01f;
    public float endLineWidth = 0.01f;
    public bool ErrorShaderTest = false;
    float timeSinceLastUpdate = 0;
    float minTimeSinceLastUpdate = 0.1f;
    
    private GameObject PalleteColor;
 

    //Stores the color the user is selecting from the pallet hand. If no color is being selected then this will be null. See OnCollisionEnter/Exit below.
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

    /// Event Args for the Interaction Manager Events
    private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        m_Drawing = false;

        obj.state.sourcePose.TryGetPosition(out m_HandPos);
        EndTrail(m_HandPos + m_HandTrackingOffset);
    }

    private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs obj)
    {
        //We check if the user is trying to change color instead of painting before continuing on
        if (PalleteColor != null)
        {
            //Run ChangeColor method in ColorPicker script
            ColorPicker.ChangeColor();
        }
        else
        {
            m_Drawing = true;

            obj.state.sourcePose.TryGetPosition(out m_HandPos);
        TrailPos = m_Cam.transform.forward * 2 + m_HandPos + m_HandTrackingOffset;
            Trail(TrailPos);
            m_TrailRenderer.transform.position = TrailPos;
        }
    }

    private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
    {
        m_Drawing = false;

        obj.state.sourcePose.TryGetPosition(out m_HandPos);
        EndTrail(m_HandPos + m_HandTrackingOffset);
    }

    private void InteractionManager_SourceUpdated(InteractionSourceUpdatedEventArgs state)
    {
        InteractionSourcePose statePose = state.state.sourcePose;

        statePose.TryGetPosition(out m_HandPos, InteractionSourceNode.Pointer);
        statePose.TryGetForward(out m_HandFor, InteractionSourceNode.Pointer);
        statePose.TryGetRotation(out m_HandRot, InteractionSourceNode.Pointer);

        if (state.state.source.handedness == InteractionSourceHandedness.Right)
        {
            m_PaintingHand.transform.localPosition = m_HandPos + m_HandTrackingOffset;
            m_PaintingHand.transform.localRotation = m_HandRot;

            if (m_Drawing)
            {
                TrailPos = m_Cam.transform.forward * 2 + m_HandPos + m_HandTrackingOffset; ;
                m_TrailRenderer.transform.localPosition = TrailPos;
            }
        }

        if (state.state.source.handedness == InteractionSourceHandedness.Left)
        {
            m_PaletteHand.transform.localPosition = m_HandFor + m_HandTrackingOffset;
            m_PaletteHand.transform.localRotation = m_HandRot;
        }

        m_SelectPressedAmount = state.state.selectPressedAmount;
    }

    private void InteractionManager_SourceDetected(InteractionSourceDetectedEventArgs state)
    {
        state.state.sourcePose.TryGetPosition(out m_HandPos);
        TrailPos = m_Cam.transform.forward * 2 + m_HandPos;
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

    //Color Pallet management
    //--OnCollisionEnter()
    //--OnCollisionExit()
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

    void OnCollisionExit(Collision collisionObj)
    {
        //If user is not trying to change color we change the PalletColor to null
        if (collisionObj.gameObject.CompareTag("PalleteColor"))
        {
            PalleteColor = null;
        }
    }
    private void OnDestroy()
    {
        InteractionManager.InteractionSourceDetected -= InteractionManager_SourceDetected;
        InteractionManager.InteractionSourceUpdated -= InteractionManager_SourceUpdated;
        InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
    }}
