using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Basket; 
using MLAgents; 

namespace Basket {


public class BasketUNNAgent : Agent {

  public BasketManager Manager;

  
  public override void InitializeAgent(){

  }

  public override void CollectObservations(){

    AddVectorObs(Manager.GetObservations()); 

  } 

  public override void AgentAction(float [] actions, string text_action){

    Manager.SetActions(actions); 
    SetReward(Manager.ComputeReward()); 
    if(Manager.Done())
      Done(); 

  }

  public override void AgentReset(){
    Manager.Reset(); 
  }
}


}
