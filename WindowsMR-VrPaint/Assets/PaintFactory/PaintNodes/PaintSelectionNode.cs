using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PaintSelectionNode : MonoBehaviour {

    public void OnTriggerExit(Collider c)
    {
        OnTriggerEnter(c);
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.gameObject == HandTracking.brushTip)
        {
            OnSelected();
        }
    }

    protected virtual void OnSelected()
    {
        
    }

    public virtual void CreateDisplayMaterial()
    {

    }
    
    void Start () {

	}
	
	void Update () {
		
	}
}
