using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHelper : MonoBehaviour 
{
	private GameObject drawingObject = null;

	public void Update()
	{
		if (Input.GetMouseButton (0)) 
		{
			AddPoints (Input.mousePosition, false);
		}

		if (Input.GetMouseButtonUp(0))
		{
			AddPoints (Input.mousePosition, true);
		}
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
			drawingObject.GetComponent<LineRenderer> ().SetWidth(0.2f, 0.2f);
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
