using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class HandTracking : MonoBehaviour
{
    //Static Access
    private static HandTracking self;
    public static GameObject paintingHand { get { return self.m_PaintingHand; } }
    public static GameObject paletteHand { get { return self.m_PaletteHand; } }
    public static GameObject brushTip { get { return self.m_BrushTip;  } }
    public static Vector3 brushPosition { get { return self.m_BrushTip.transform.position; } }

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

    public float m_SelectPressedAmount = 0f;
    public float m_TrailTrackingDistance = 0.2f;
    public float startLineWidth = 0.01f;
    public float endLineWidth = 0.01f;
    public float minVertexDistance = 0.03f;

    float timeSinceLastUpdate = 0;
    float minTimeSinceLastUpdate = 0.1f;

    // MORTY, I'M A PICKLE!

    void Start()
    {
        self = this;

        /// Events to listen to to track the controllers
        InteractionManager.InteractionSourceDetected += InteractionManager_SourceDetected;
        InteractionManager.InteractionSourceUpdated += InteractionManager_SourceUpdated;
        InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;

        m_Camera = Camera.main.gameObject;

        //InteractionManager.GetCurrentReading();
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
        if (obj.state.source.handedness == InteractionSourceHandedness.Right)
        {
            m_Drawing = true;

            obj.state.sourcePose.TryGetPosition(out m_HandPos);
            obj.state.sourcePose.TryGetForward(out m_HandFor);
            CreateTrailRenderer(brushPosition);
        }
        else if(obj.state.source.handedness == InteractionSourceHandedness.Left)
        {
            DestroyAllTrails();
        }
    }
    
    private void DestroyAllTrails()
    {
        TrailRenderer[] trails = GameObject.FindObjectsOfType<TrailRenderer>();

        foreach(TrailRenderer renderer in trails)
        {
            if(renderer != m_TrailRenderer)
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

    private void InteractionManager_SourceUpdated(InteractionSourceUpdatedEventArgs state)
    {
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

        m_SelectPressedAmount = state.state.selectPressedAmount;
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
        m_TrailRenderer.startWidth = startLineWidth;
        m_TrailRenderer.endWidth = endLineWidth;
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
            }
        }
    }
    
    void EndTrail(Vector3 endPos)
    {
        m_TrailRenderer = null;
        timeSinceLastUpdate = 0;
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
