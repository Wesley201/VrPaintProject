using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

/*Control Notes:
 * Paint: Right Select
 * Erase: Left Select
 * Brush Size: Right Touchpad

    */
public partial class HandTracking : MonoBehaviour
{
    //Static Access
    private static HandTracking self;
    public static GameObject paintingHand { get { return self.m_PaintingHand; } }
    public static GameObject paletteHand { get { return self.m_PaletteHand; } }
    public static GameObject brushTip { get { return self.m_BrushTip;  } }
    public static Vector3 brushPosition { get { return self.m_BrushTip.transform.position; } }
    public static float brushSize = 0.03f;

    //Instance
    private GameObject m_Camera;

    /// Variables for Hand Tracking
    private Vector3 m_HandPos;
    private Vector3 m_HandFor;
    private Quaternion m_HandRot;

    /// Objects for tracking the hands
    public GameObject m_PaintingHand;
    public GameObject m_PaletteHand;
    
    private TrailRenderer m_TrailRenderer;

    public GameObject m_BrushTip;

    public bool m_Drawing = false;
    public bool m_LineRendererAutoDestruct = false;

    private bool m_IsTouching = false;

    public float m_TrailTrackingDistance = 0.2f;
    //public float startLineWidth = 0.01f;
    //public float endLineWidth = 0.01f;
    public float minVertexDistance = 0.03f;

    public float m_BrushStartSize = 0.03f;
    public float m_BrushMaxSize = 0.05f;
    public float m_BrushMinSize = 0.005f;
    public float m_BrushSizeRate = 0.1f;

    //Sync update intervals to the controller's updates rather than unity's updates
    float deltaTime = 0f;
    float lastUpdateTime = 0f;

    //Simple class to cleanup input.  Action methods are in other partial class files (EraseInput, PaintInput, BrushSizeInput, etc)
    List<InputAction> inputActions = new List<InputAction>();

    // MORTY, I'M A PICKLE!


    class InputAction
    {
        public InteractionSourcePressType pressType;
        public InteractionSourceHandedness handedness;
        public Action<InteractionSourcePressedEventArgs> pressedAction;
        public Action<InteractionSourceReleasedEventArgs> releasedAction;
        public Action<InteractionSourceUpdatedEventArgs> updatedAction;

        public InputAction(InteractionSourcePressType pressType, InteractionSourceHandedness handedness, Action<InteractionSourcePressedEventArgs> pressedAction,
            Action<InteractionSourceReleasedEventArgs> releasedAction, Action<InteractionSourceUpdatedEventArgs> updatedAction)
        {
            this.pressType = pressType;
            this.handedness = handedness;
            this.pressedAction = pressedAction;
            this.releasedAction = releasedAction;
            this.updatedAction = updatedAction;
        }
    }

    void Start()
    {
        self = this;
        brushSize = m_BrushStartSize;

        lastUpdateTime = Time.time;

        /// Events to listen to to track the controllers
        InteractionManager.InteractionSourceDetected += InteractionManager_SourceDetected;
        InteractionManager.InteractionSourceUpdated += InteractionManager_SourceUpdated;
        InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;

        inputActions.Add(new InputAction(InteractionSourcePressType.Select, InteractionSourceHandedness.Right, PaintPressed, PaintReleased, PaintUpdated));
        inputActions.Add(new InputAction(InteractionSourcePressType.Touchpad, InteractionSourceHandedness.Right, BrushSizePressed, BrushSizeReleased, BrushSizeUpdated));
        inputActions.Add(new InputAction(InteractionSourcePressType.Select, InteractionSourceHandedness.Left, ErasePressed, EraseReleased, EraseUpdated));

        m_Camera = Camera.main.gameObject;
        
        //InteractionManager.GetCurrentReading();
    }

    /// Event Args for the Interaction Manager Events
    private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        foreach (InputAction action in inputActions)
        {
            if (action.pressType == obj.pressType && action.handedness == obj.state.source.handedness && action.releasedAction != null)
            {
                action.releasedAction(obj);
            }

        }
    }
    
    private void InteractionManager_SourceUpdated(InteractionSourceUpdatedEventArgs state)
    {
        //Update timing
        deltaTime = Time.time - lastUpdateTime;
        lastUpdateTime = Time.time;

        //Update transforms
        InteractionSourcePose statePose = state.state.sourcePose;
        
        statePose.TryGetPosition(out m_HandPos, InteractionSourceNode.Grip);
        statePose.TryGetForward(out m_HandFor, InteractionSourceNode.Grip);
        statePose.TryGetRotation(out m_HandRot, InteractionSourceNode.Grip);

        if (state.state.source.handedness == InteractionSourceHandedness.Right)
        {
            m_PaintingHand.transform.localPosition = m_HandPos;
            m_PaintingHand.transform.localRotation = m_HandRot;

            if (m_Drawing && m_TrailRenderer != null)
            {
                m_TrailRenderer.transform.localPosition = brushPosition;
            }
        }

        if (state.state.source.handedness == InteractionSourceHandedness.Left)
        {
            m_PaletteHand.transform.localPosition = m_HandPos;
            m_PaletteHand.transform.localRotation = m_HandRot;
        }
        
        //Allow input specific update callbacks
        foreach (InputAction action in inputActions)
        {
            if (action.handedness == state.state.source.handedness && action.updatedAction != null)
            {
                action.updatedAction(state);
            }

        }
    }

    private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs obj)
    {

        foreach (InputAction action in inputActions)
        {
            if (action.pressType == obj.pressType && action.handedness == obj.state.source.handedness && action.pressedAction != null)
            {
                action.pressedAction(obj);
            }
                
        }
    }

    private void DestroyAllTrails()
    {
        TrailRenderer[] trails = GameObject.FindObjectsOfType<TrailRenderer>();

        foreach (TrailRenderer renderer in trails)
        {
            if (renderer != m_TrailRenderer)
            {
                GameObject.Destroy(renderer);
            }
        }

    }

    private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
    {
        m_Drawing = false;

        obj.state.sourcePose.TryGetPosition(out m_HandPos);

        EndTrail(m_HandPos);
    }
    

    private void InteractionManager_SourceDetected(InteractionSourceDetectedEventArgs state)
    {
        state.state.sourcePose.TryGetPosition(out m_HandPos);
        state.state.sourcePose.TryGetForward(out m_HandFor);
    }

    void CreateTrailRenderer(Vector3 startPos)
    {
        
        GameObject newTrailObject = new GameObject("Trail");
        newTrailObject.transform.position = startPos;
        m_TrailRenderer = newTrailObject.AddComponent<TrailRenderer>();
        m_TrailRenderer.autodestruct = m_LineRendererAutoDestruct;
        m_TrailRenderer.startWidth = brushSize;// startLineWidth;
        m_TrailRenderer.endWidth = brushSize;// endLineWidth;
        m_TrailRenderer.minVertexDistance = minVertexDistance;
        m_TrailRenderer.time = float.MaxValue;
        m_TrailRenderer.textureMode = LineTextureMode.RepeatPerSegment;
        m_TrailRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbesAndSkybox;
        m_TrailRenderer.generateLightingData = true;

        PaintFactory.ApplyPaintToTrailRenderer(m_TrailRenderer);
    }

    private void Update()
    {
        if(m_BrushTip != null)
        {
            MeshRenderer renderer = m_BrushTip.GetComponent<MeshRenderer>();
            if(renderer != null)
            {
                renderer.enabled = !m_Drawing;
                renderer.transform.localScale = brushSize * Vector3.one;
            }
        }
    }
    
    void EndTrail(Vector3 endPos)
    {
        m_TrailRenderer = null;
    }
    
    private void OnDestroy()
    {
        InteractionManager.InteractionSourceDetected -= InteractionManager_SourceDetected;
        InteractionManager.InteractionSourceUpdated -= InteractionManager_SourceUpdated;
        InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
        
    }
}
