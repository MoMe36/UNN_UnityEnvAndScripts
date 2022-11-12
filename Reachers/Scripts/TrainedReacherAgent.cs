using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reachers;
using MLAgents; 
using Tennis; 
using ManipVentouse; 
using VentouseV2; 

namespace Reachers{

public class TrainedReacherAgent : Agent{

    public TransformArmController ArmController; 
    public Transform RootTransform; 

    public enum LinkType {tennis, manip, manip_v2, debug}; 
    public LinkType Link; 
    public LinkTennisReacher LinkTennis; 
    
    [Header("Manip")]
    public LinkManipReacher LinkReacher; 
    public enum ArmType {left, right}; 
    public ArmType ArmPosition; 
    // public DebugTrainedReacher Link; 

    [Header("Manip V2")]
    public VentouseController LinkReacher2; 
    public Transform UNNTransform; 
    public Transform ThisArmEffector; 


    [HideInInspector] public Vector3 DirectionVector; 

    public bool DoScale; 
    public bool MirrorXAxis; 
    public bool DoInvert = true; 

    public override void InitializeAgent(){

    }

    public void SetDirection(Vector3 dir){
        DirectionVector = dir; 
    }

    public override void CollectObservations(){
        AddVectorObs(ArmController.GetAngles());
        Quaternion invert = Quaternion.Inverse(RootTransform.rotation);
        Vector3 scale = RootTransform.localScale; 


        if(!DoInvert)
            invert = Quaternion.identity; 
        // Take into account the scale and initial rotation of the object. Should make the behaviour invariant to transformation.  

        if(Link == LinkType.tennis)
            DirectionVector = LinkTennis.GetDirection();
        else if(Link == LinkType.manip){
            string arm_id = ArmPosition == ArmType.left ? "left" : "right"; 
            DirectionVector= LinkReacher.GetDirection(arm_id); 
        } else if(Link == LinkType.manip_v2) {
            Vector3 dir_from_brain = LinkReacher2.GetDirection(); 
            Vector3 v1 = UNNTransform.position - ThisArmEffector.position;
            Vector3 target_pos = ThisArmEffector.position + v1 + dir_from_brain;
            Vector3 target_vec = target_pos - ThisArmEffector.position;   
            DirectionVector = target_vec; 

            Debug.DrawRay(ThisArmEffector.position, DirectionVector, Color.red, 0.2f); 

        }


        if(MirrorXAxis)
            DirectionVector = Vector3.Scale(DirectionVector, new Vector3(-1f, 1f , 1f)); 

        Vector3 processed_vector =  invert*DirectionVector; 
        if(DoScale)
            processed_vector = Vector3.Scale(processed_vector, new Vector3(1f/scale.x, 1f/scale.y,1f/scale.z)); 

        // Vector3 processed_vector = invert*(Vector3.Scale(DirectionVector, new Vector3(1f/scale.x, 1f/scale.y,1f/scale.z)));  
        AddVectorObs(processed_vector);   
    }

    public override void AgentAction(float [] action, string text_action){

        ArmController.SetAngles(action); 
        ArmController.UpdateArm(); 


    }
    public override void AgentReset(){


    }


}

}