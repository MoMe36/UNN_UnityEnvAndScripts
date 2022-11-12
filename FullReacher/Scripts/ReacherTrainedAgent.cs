using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using MLAgents; 
using Reachers; 

public class ReacherTrainedAgent : Agent{
    

    public TransformArmController ArmController; 
    public Vector3 DirectionTarget; 


    public override void InitializeAgent(){

    }

    public override void CollectObservations(){
        AddVectorObs(ArmController.GetAngles());
        AddVectorObs(DirectionTarget);  
    }

    public override void AgentAction(float [] action, string text_action){

        ArmController.SetAngles(action); 
        ArmController.UpdateArm(); 
        // Debug.Log("CalledActionReacher"); 
    }

    public void SetUNNObservation(Vector3 unn_obs){
        DirectionTarget = unn_obs; 
    }

    public void Reset(){
        ArmController.Reset(false);
    }
    



}