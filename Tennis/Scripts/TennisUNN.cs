using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tennis; 
using MLAgents; 
using Reachers; 


namespace Tennis{

public class TennisUNN : Agent{

    public SceneManager Manager; 

    public enum TrainingType {normal, with_arm, debug}; 
    public TrainingType Training; 

    [Header("Initial training")]
    public PaddleControl Controller; 

    [Header("Arm control")]
    // public TrainedReacherAgent ReacherController;
    public LinkTennisReacher Link;  

    public bool RotateAll; 

    public override void InitializeAgent(){

    } 

    public override void CollectObservations(){

        if(RotateAll){

            AddVectorObs(Quaternion.AngleAxis(180f, Vector3.up) * Controller.GetRelativePaddlePosition()); 
            AddVectorObs(Controller.GetPaddleOrientation()); 
            AddVectorObs(Quaternion.AngleAxis(180f, Vector3.up) * Controller.GetPaddleSpeed()); 

            AddVectorObs(Quaternion.AngleAxis(180f, Vector3.up) * Manager.GetRelativeBallPosition()); 
            AddVectorObs(Quaternion.AngleAxis(180f, Vector3.up) * Manager.GetBallSpeed()); 
        
        } else{
            
            AddVectorObs(Controller.GetRelativePaddlePosition()); 
            AddVectorObs(Controller.GetPaddleOrientation()); 
            AddVectorObs(Controller.GetPaddleSpeed()); 

            AddVectorObs(Manager.GetRelativeBallPosition()); 
            AddVectorObs(Manager.GetBallSpeed()); 

        }

    }

    public override void AgentAction(float [] action, string text_action){
        if(Training == TrainingType.normal)
        {
            if(RotateAll){
                Vector3 ideal_dir = new Vector3(action[0], action[1], action[2]); 
                Controller.ReceiveAction(Quaternion.AngleAxis(180f, Vector3.up) * ideal_dir, action[3], action[4]); 
            } else {
                Controller.ReceiveAction(new Vector3(action[0], action[1], action[2]), action[3], action[4]); 
            }

            Controller.UpdatePaddle(); 
        }
        else if(Training == TrainingType.with_arm){
            Link.SetDirection(new Vector3(action[0], action[1], action[2]));


            // Make the paddle center as the arm effector so having no movement will make the paddle follow the effector
            // Still allows for sending target angles . 
            Controller.ReceiveAction(Vector3.zero, action[3], action[4]);
            Controller.UpdatePaddle(); 
        }
        else if(Training == TrainingType.debug){

            // Does the arm follow the paddle? 

            Controller.ReceiveAction(new Vector3(action[0], action[1], action[2]), action[3], action[4]); 
            Controller.UpdatePaddle();

            Link.SetDirection(new Vector3(action[0], action[1], action[2]));


            // Make the paddle center as the arm effector so having no movement will make the paddle follow the effector
            // Still allows for sending target angles .
            Controller.UpdatePaddle();
        }


        SetReward(Manager.ComputeRewards()); 
        if(Manager.Done())
            Done(); 
    }

    public override void AgentReset(){

        if(Training == TrainingType.normal)
            Controller.Reset();
        else
            Controller.ResetToEffector();  
        Manager.Reset(); 
    }


}


}
