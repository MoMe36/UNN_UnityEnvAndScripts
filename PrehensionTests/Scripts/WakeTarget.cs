using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeTarget : MonoBehaviour {

    Rigidbody rb; 

	// Use this for initialization
	void Start () {
		
        rb = GetComponent<Rigidbody>(); 

	}
	
	// Update is called once per frame
	void Update () {

        rb.WakeUp(); 
		
	}
}
