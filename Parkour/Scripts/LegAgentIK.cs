using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents; 
using Parkour; 

namespace Parkour{


public class LegAgentIK : Agent {

    public LegControl Robot; 
    public SceneManager Manager; 

    public bool Training; 


    public override void InitializeAgent(){

       
    }

    public override void CollectObservations(){
        AddVectorObs(Manager.GetObservations()); 
    }

    public override void AgentAction(float [] actions, string test_action){

        Robot.SetTorques(actions[0], actions[1]);
        if(Training) 
            SetReward(Manager.ComputeReward()); 
    }

    public override void AgentReset(){
        
        if(Training){
            Manager.Reset(); 
            Robot.Reset(); 
        }
    }

}

}
