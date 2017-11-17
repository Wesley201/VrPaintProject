using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiate512cubes : MonoBehaviour {
    public GameObject _sampleCubePrefab;
    //public int _numCubes;
    GameObject[] _sampleCube = new GameObject[512/*_numCubes*/];
    // Use this for initialization
    //private float _cubeCircle;
    public float _maxScale;
    //public float _cubeBaseScale;

    void Start () {
        //_cubeCircle = 360f / _numCubes;
        for (int i = 0; i < 512 /*_numCubes*/; i++)
        {
            GameObject _instanceSampleCube = (GameObject)Instantiate(_sampleCubePrefab);
            _instanceSampleCube.transform.position = this.transform.position;
            _instanceSampleCube.transform.parent = this.transform;
            _instanceSampleCube.name = "SampleCube" + i;
            this.transform.eulerAngles = new Vector3(0, -0.703125f /*_cubeCircle*/ * i, 0);
            _instanceSampleCube.transform.position = Vector3.forward * 100; //perhaps change to scale based on number of cubes.
            _sampleCube[i] = _instanceSampleCube;
        }
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < 512 /* _numCubes*/; i++)
        {
            if (_sampleCube != null)
            {
                _sampleCube[i].transform.localScale = new Vector3 (10 /*_cubeBaseScale*/, Visualizer._samples[i] * _maxScale + 2, 10 /*_cubeBaseScale*/);
            }
        }
	}
}
