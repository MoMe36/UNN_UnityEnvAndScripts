using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents; 

namespace UNNAnt{

public class TargetLogic : MonoBehaviour {

    public Transform Limb;
    public Transform p1; 
    public Transform p2; 
    public Transform ph; 

    public float TrajectoryTime; 
    public enum GoingTo{high, low}; 
    public GoingTo gt; 

    [SerializeField] float current_time; 



	// Use this for initialization
	void Start () {
		
        current_time = 0f; 
        transform.position = p1.position; 
	}
	
	// Update is called once per frame
	void Update () {

        current_time = Mathf.MoveTowards(current_time, 1, Time.deltaTime/TrajectoryTime); 
        transform.position = (1f - current_time)*p1.position + current_time*ph.position; 

	}
}

}
