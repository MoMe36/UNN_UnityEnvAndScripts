using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents; 
using BaseMobile; 

public class FourWheeledAgent : Agent {

    public SceneManager manager; 
    public Transform Target; 

    Rigidbody rb; 
    WheelController wheel_controller; 


    public override void InitializeAgent(){

        wheel_controller = GetComponent<WheelController>(); 
        rb = GetComponent<Rigidbody>(); 
    }

    public override void CollectObservations(){

        AddVectorObs(rb.velocity.x);
        AddVectorObs(rb.velocity.z); 
        Vector3 dir_target = Target.position - transform.position; 
        AddVectorObs(new float []{dir_target.x, dir_target.z});

        float angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(dir_target, Vector3.up), transform.forward, Vector3.up)/180f; 
        AddVectorObs(angle); 
        // Get steering angle & last torque applied
        AddVectorObs(wheel_controller.GetDrivingInfos()); 

    }

    public override void AgentAction(float [] actions, string test_action){

        float new_steering = actions[0]; 
        float new_torque = actions[1]; 

        wheel_controller.SetInputsValues(new_steering, new_torque); 


        float reward = manager.ComputeReward(); 

        SetReward(reward);
        if(manager.Done())
            Done(); 

    }

    public override void AgentReset(){
        manager.Reset(); 

    }

}
