using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reachers;
using MLAgents; 

namespace Reachers{

public class ReacherAgent : Agent{

    public SceneManager manager; 
    public TransformArmController ArmController; 
    public Transform Target; 
    public Transform Effector; 
    public Transform RootTransform; 


    public override void InitializeAgent(){

    }

    public override void CollectObservations(){
        AddVectorObs(ArmController.GetAngles());
        // AddVectorObs(Target.position - Effector.position);
        Quaternion invert = Quaternion.Inverse(RootTransform.rotation); 
        AddVectorObs(invert*(Target.position - Effector.position));   
    }

    public override void AgentAction(float [] action, string text_action){

        // counter += 1; 
        // if(counter == SendEvery)
        // {
        ArmController.SetAngles(action); 
        //     counter = 0; 
        // }
        ArmController.UpdateArm(); 

        SetReward(manager.ComputeReward()); 


    }
    public override void AgentReset(){

        manager.Reset(); 
        // counter = 0; 

    }


}

}