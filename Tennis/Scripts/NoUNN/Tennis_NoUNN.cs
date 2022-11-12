using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tennis; 
using MLAgents; 
using Reachers; 


namespace Tennis{

public class Tennis_NoUNN : Agent{

    public SceneManager_2 Manager; 

    public override void InitializeAgent(){

    } 

    public override void CollectObservations(){

        AddVectorObs(Manager.GetObservations()); 

    }

    public override void AgentAction(float [] action, string text_action){
       
        Manager.SetActions(action); 

        SetReward(Manager.ComputeRewards()); 
        if(Manager.Done())
            Done(); 
    }

    public override void AgentReset(){

        Manager.Reset(); 
    }


}


}
