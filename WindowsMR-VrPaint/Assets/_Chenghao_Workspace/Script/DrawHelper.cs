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

		new UnityEngine.XR.WSA.Input.TappedEventArgs((value) => {
			Debug.Log("tapped");	
		});

		new UnityEngine.XR.WSA.Input.GestureRecognizer.HoldStartedEventDelegate ((source, headRay) => {
			Debug.Log (source);
			OnHoldStarted (headRay.origin);
		});

		new UnityEngine.XR.WSA.Input.GestureRecognizer.HoldCompletedEventDelegate ((source, headRay) => {
			Debug.Log (source);
			OnHoldCompleted (headRay.origin);
		});

		gestureRecognizer.StartCapturingGestures ();
	}

	public void OnHoldStarted(Vector3 position)
	{
		AddPoints (position, false);
	}

	public void OnHoldCompleted(Vector3 position)
	{
		AddPoints (position, true);
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
