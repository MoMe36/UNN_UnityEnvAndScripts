using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents; 
using BaseMobile; 

public class SimplestAgent : Agent {

    public SceneManager manager; 
    public Transform Target; 
    public float MaxSpeed; 

    Rigidbody rb; 


    public override void InitializeAgent(){

        rb = GetComponent<Rigidbody>(); 

    }

    public override void CollectObservations(){

        AddVectorObs(rb.velocity.x);
        AddVectorObs(rb.velocity.z); 
        Vector3 dir_target = Target.position - transform.position; 
        AddVectorObs(new float []{dir_target.x, dir_target.z}); 

    }

    public override void AgentAction(float [] actions, string test_action){

        Vector3 force = new Vector3(actions[0],0f, actions[1]); 
        rb.AddForce(force*MaxSpeed); 

        float reward = manager.ComputeReward(); 

        SetReward(reward);
        if(manager.Done())
            Done(); 

    }

    public override void AgentReset(){
        manager.Reset(); 

    }

}
