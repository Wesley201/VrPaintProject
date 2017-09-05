using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.WSA.Input;

public class GazeController : MonoBehaviour
{
    //Stores reference to which object is the camera
    private Camera cam;

    private GestureRecognizer gestureRecognizer;

	// Use this for initialization
	void Start ()
	{
        //Initializes the camera as the main camera
	    cam = Camera.main;

        //sets up gesture recognizer
        gestureRecognizer = new GestureRecognizer();
	    gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        gestureRecognizer.Tapped += GestureRecognizer_Tapped;
        gestureRecognizer.StartCapturingGestures();
	}

    private void GestureRecognizer_Tapped(TappedEventArgs obj)
    {
        //sets up a container for which object is hit by the ray
        RaycastHit rayHit = new RaycastHit();

        //Fires a raycast looking to hit a button
        if (Physics.Raycast(cam.transform.localPosition, cam.transform.forward, out rayHit, 10f))
        {
            //If button name is "Button_New", load Main scene
            if (rayHit.collider.name == "Button_New")
            {
                SceneManager.LoadScene("Main");
            }
            //If button name is "Button_Exit", exit the application
            if (rayHit.collider.name == "Button_Exit")
            {
                Application.Quit();
            }
        }
    }

    //gesture recognizer cleanup code.
    void OnDestroy()
    {
        gestureRecognizer.Tapped -= GestureRecognizer_Tapped;
        gestureRecognizer.StopCapturingGestures();
        gestureRecognizer.Dispose();
    }
}
