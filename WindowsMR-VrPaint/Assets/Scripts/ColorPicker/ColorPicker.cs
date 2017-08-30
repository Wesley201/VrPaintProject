/*
 * README
 * This script will not connect to the TrailRenderer as is. Once we decide where that code to generate the 
 * LineRenderer goes, we need to call on this brush color material in the section where a new trail is created.
 * Each time a new trail is started/created, it will use the brushColor material that is set. All we need to add
 * is a reference to this Component to call forth its stored materials. From there we can set it to the
 * TrailRenderer.material property in the code which creates the trail.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    /*== CONTAINERS FOR COLORS ==*/

    //private Material stagedColor;             //Not using this yet, will implement if we move to RGB picker then this will be a staging material to preview what you're picking
    private Material brushColor;

    /*== COLORS ==*/
    public Material colorSmoke;            //Default color
    public Material colorBlue;
    public Material colorGreen;
    public Material colorRed;
    
	void Start ()
	{
        //If brush color is null, set the color to Smoke. If somehow a value is present then do nothing.
        if(brushColor == null)
	    brushColor = colorSmoke;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            brushColor = colorBlue;
            Debug.Log("Brush color has been set to BLUE");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            brushColor = colorGreen;
            Debug.Log("Brush color has been set to GREEN");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            brushColor = colorRed;
            Debug.Log("Brush color has been set to RED");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            brushColor = colorSmoke;
            Debug.Log("Color has been set to WHITE");
        }
    }
}
