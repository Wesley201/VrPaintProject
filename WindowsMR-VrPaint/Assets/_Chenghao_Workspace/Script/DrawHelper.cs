using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using System;

public class DrawHelper : MonoBehaviour 
{
	private GameObject drawingObject = null;

	public void Start()
	{
		UnityEngine.XR.WSA.Input.GestureRecognizer gestureRecognizer = new UnityEngine.XR.WSA.Input.GestureRecognizer();
		gestureRecognizer.SetRecognizableGestures(UnityEngine.XR.WSA.Input.GestureSettings.Hold | UnityEngine.XR.WSA.Input.GestureSettings.Tap);

		gestureRecognizer.HoldStartedEvent += OnHoldStarted;
		gestureRecognizer.HoldCompleted += OnHoldCompleted;

		gestureRecognizer.StartCapturingGestures ();
	}

	public void OnHoldStarted(UnityEngine.XR.WSA.Input.InteractionSourceKind source, Ray ray)
	{
		AddPoints (ray.origin, false);
	}

	public void OnHoldCompleted(UnityEngine.XR.WSA.Input.InteractionSourceKind source, Ray ray)
	{
		AddPoints (ray.origin, true);
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
