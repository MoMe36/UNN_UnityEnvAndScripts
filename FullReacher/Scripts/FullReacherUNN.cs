using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using MLAgents; 
using FullReacher; 

namespace FullReacher{


public class FullReacherUNN : Agent{

    public TwoWheeledTrainedAgent Locomotion; 
    public ReacherTrainedAgent Prehension; 
    public SceneManager Manager; 

    public float MagnitudeLocomotion = 1f; 
    public float MagnitudePrehension = 1f; 
    

    public override void InitializeAgent(){


    }

    public override void CollectObservations(){

        AddVectorObs(Manager.GetVectorObs()); 

    }

    public override void AgentAction(float [] actions, string test_action){

        Locomotion.SetUNNObservation(new Vector2(actions[0], actions[1])*MagnitudeLocomotion);
        Prehension.SetUNNObservation(new Vector3(actions[2], actions[3], actions[4])*MagnitudePrehension);

        SetReward(Manager.ComputeReward()); 
        // Debug.Log("Called UNN action"); 
        if(Manager.Done())
        {
            Locomotion.Reset(); 
            Prehension.Reset(); 
            Done(); 
        }

    }

    public override void AgentReset(){

        // Debug.Log("Called UNN reset"); 
        Manager.Reset(); 
        Locomotion.Reset(); 
        Prehension.Reset(); 
    
    }


}
}