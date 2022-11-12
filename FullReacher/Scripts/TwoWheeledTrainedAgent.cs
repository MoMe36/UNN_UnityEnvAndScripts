using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using MLAgents; 
using BaseMobile; 

public class TwoWheeledTrainedAgent : Agent{
    

    public Vector2 DirectionTarget; 
    Rigidbody rb; 
    WheelController wheel_controller; 


    public override void InitializeAgent(){

        rb = GetComponent<Rigidbody>(); 
        wheel_controller = GetComponent<WheelController>(); 

    }

    public override void CollectObservations(){

        AddVectorObs(rb.velocity.x);
        AddVectorObs(rb.velocity.z); 

        AddVectorObs(DirectionTarget); 

        float angle = Vector3.SignedAngle(new Vector3(DirectionTarget.x, 0f, DirectionTarget.y), transform.forward, Vector3.up)/180f; 
        AddVectorObs(angle); 
        // Get steering angle & last torque applied
        AddVectorObs(wheel_controller.GetDrivingInfos()); 
    }

    public override void AgentAction(float [] actions, string test_action){

        float new_steering = actions[0]; 
        float new_torque = actions[1]; 

        wheel_controller.SetInputsValues(new_steering, new_torque); 
    }

    public void SetUNNObservation(Vector2 unn_obs){
        DirectionTarget = unn_obs; 
    }

    public void Reset()
    {

    }



}