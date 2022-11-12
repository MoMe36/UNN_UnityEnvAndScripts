using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents; 
using Parkour; 

namespace Parkour{


public class BipedIK : Agent {

    public enum Task {parkour, imitation, resist}; 
    public Task ToDo; 
    public ImitationManager iManager; 
    public ParkourManager Manager; 
    public ResistManager rManager; 
    public bool ManualControl; 
    [Range(-1f, 1f)] public float [] ManualValues;


    public override void InitializeAgent(){

       
    }

    public override void CollectObservations(){
        if(DoingParkour())
            AddVectorObs(Manager.GetObservations());
        else if(DoingImitation())
            AddVectorObs(iManager.GetObservations());
        else 
            AddVectorObs(rManager.GetObservations()); 

        // foreach(Vector3 v in obs)
        //     AddVectorObs(v); 
    }

    public override void AgentAction(float [] actions, string test_action){
        
        if(ManualControl)
            Manager.SetActions(ManualValues);
        else {
            if(DoingParkour())
                Manager.SetActions(actions);
            else if(DoingImitation())
                iManager.SetActions(actions);
            else
                rManager.SetActions(actions);   
        }

        if(DoingParkour()){
            SetReward(Manager.ComputeRewards());
            if(Manager.Done())
                Done();      
        } else if(DoingImitation()) {
            SetReward(iManager.ComputeRewards());
            if(iManager.Done())
                Done(); 
        } else {
            SetReward(rManager.ComputeRewards());
            if(rManager.Done())
                Done();
        }

        
        // if(Manager.Done())
        //     Done(); 
    }

    public override void AgentReset(){     
        if(DoingParkour())   
            Manager.Reset();
        else if(DoingImitation())
            iManager.Reset();    
        else 
            rManager.Reset();  
    }

    public bool DoingParkour(){
        return ToDo == Task.parkour; 
    }

    public bool DoingImitation(){
        return ToDo == Task.imitation; 
    }

}

}
