using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents; 

namespace Morpho {


public class AgentSegment : Agent{

    public Transform Target; 
    public float Message; 
    public int Number; 
    public ChainControl control; 

    public override void InitializeAgent(){


    }

    public override void CollectObservations(){

        Vector3 msg = Vector3.one*Message;
        AddVectorObs(msg); 
        AddVectorObs(transform.position - Target.position);  

    }

    public override void AgentAction(float [] action, string text_action){

        control.SetAngle(Number, action[0]); 


    }

    public override void AgentReset(){

        control.SetAngle(Number, 0f); 

    }

    public void SetNumber(int i)
    {
        Number = i; 
    }

    public void SetControl(ChainControl c)
    {
        control = c; 
    }

    public void SetTarget(Transform t)
    {
        Target = t; 
    }

    public void SetMessage(float i)
    {
        Message = i; 
    }

}


}
