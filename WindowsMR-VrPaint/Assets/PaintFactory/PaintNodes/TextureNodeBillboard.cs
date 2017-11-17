using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureNodeBillboard : MonoBehaviour {
    
	void Update () {
        transform.forward = -(Camera.main.transform.position - transform.position).normalized;
	}
}
