using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reachers; 

public class DebugTrainedReacherCenter : MonoBehaviour {

    public Transform Target; 
    public Transform Effector; 
    public Vector3 Offset; 

    public DebugTrainedReacher TrainedReacher; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        Offset = Target.position - transform.position; 


        Vector3 ec = transform.position - Effector.position; 
        Vector3 ct = Target.position - transform.position; 
        Vector3 full =  ec + ct; 

        Debug.DrawRay(Effector.position, ec, Color.red, 0.1f); 
        Debug.DrawRay(transform.position, ct, Color.green, 0.1f); 
        Debug.DrawRay(Effector.position, full, Color.blue, 0.1f); 
        

        TrainedReacher.SetDirection(full); 
	}
}
