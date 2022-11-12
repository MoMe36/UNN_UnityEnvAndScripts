using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController01 : MonoBehaviour {

    public WheelCollider Front; 
    public WheelCollider Rear; 
    public float MaxTorque; 

    
    [Range(-1f, 1f)] public float FrontController; 
    [Range(-1f, 1f)] public float RearController; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
        float torque = MaxTorque;
        Front.motorTorque = FrontController*torque; 
        Rear.motorTorque = RearController*torque; 
	}
}
