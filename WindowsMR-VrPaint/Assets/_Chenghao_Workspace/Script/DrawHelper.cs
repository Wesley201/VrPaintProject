using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using System;

public class DrawHelper : MonoBehaviour 
{
	private GameObject drawingObject = null;
	private Vector3 handsPosition;

	public void Start()
	{
		GestureRecognizer gestureRecognizer = new UnityEngine.XR.WSA.Input.GestureRecognizer();
		gestureRecognizer.SetRecognizableGestures(GestureSettings.Hold | GestureSettings.Tap);

		InteractionManager.InteractionSourcePressed += OnHoldStarted;
		InteractionManager.InteractionSourceReleased += OnHoldCompleted;

		gestureRecognizer.StartCapturingGestures ();
		InteractionManager.GetCurrentReading();
	}
	// InteractionSourceState state
	public void OnHoldStarted(InteractionSourcePressedEventArgs obj)
	{
		obj.state.sourcePose.TryGetPosition(out handsPosition);
		AddPoints (handsPosition, false);
	}

	public void OnHoldCompleted(InteractionSourceReleasedEventArgs obj)
	{
		obj.state.sourcePose.TryGetPosition(out handsPosition);
		AddPoints (handsPosition, true);
	}
		
	public static DrawHelper GetInstance()
	{
		return GameObject.FindObjectOfType<DrawHelper> ();
	}

	public void AddPoints(Vector3 position, bool isFinished)
	{
		if (drawingObject == null) 
		{
			drawingObject = new GameObject ("DrawingObject");
			drawingObject.AddComponent<LineRenderer> ();
			drawingObject.GetComponent<LineRenderer> ().material = new Material(Shader.Find("Particles/Additive"));
			drawingObject.GetComponent<LineRenderer> ().positionCount = 0;
			drawingObject.GetComponent<LineRenderer> ().SetWidth(5f, 5f);
			drawingObject.GetComponent<LineRenderer> ().SetColors(Color.red, Color.red);
		}
			
		int newSize = drawingObject.GetComponent<LineRenderer> ().positionCount + 1;
		drawingObject.GetComponent<LineRenderer> ().positionCount = newSize;
		 
		drawingObject.GetComponent<LineRenderer> ().SetPosition (newSize - 1, position);

		if (isFinished) 
		{
			drawingObject = null;
		}
	}
}
