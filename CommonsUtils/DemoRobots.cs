using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reachers; 

public class DemoRobots : MonoBehaviour {


    public TransformArmController [] ArmController; 
    public float DelayBeforePose; 
    public float RobotSpeed = 1f; 


    float[] delays; 

	// Use this for initialization
	void Start () {
	
        delays = new float[ArmController.Length]; 	

        for(int i = 0; i < ArmController.Length; i++ ){
            SetLimits(ArmController[i]); 
            SetSpeed(ArmController[i]); 
            delays[i] = Random.Range(0.1f, DelayBeforePose); 
        }

	}
	
	// Update is called once per frame
	void Update () {

        for(int i = 0; i < delays.Length; i++){
            delays[i] -= Time.deltaTime; 
            if(delays[i] < 0f){
                delays[i] = DelayBeforePose;
                SetNewPose(ArmController[i]); 
            }
        }

        foreach(TransformArmController controler in ArmController)
            controler.UpdateArm(); 
		
	}

    void SetLimits(TransformArmController tac){

        for(int i = 0; i< tac.Arm.GetLength(); i++){
            tac.Arm.ChangeSegmentLimit(i, true, new Vector2(-0.7f, 0.7f)); 
        }
    }

    void SetNewPose(TransformArmController tac){

        int size = tac.Arm.GetLength(); 
        float[] next_pose = new float [size];

        for(int i = 0; i < size; i ++){
            // next_pose[i] = Random.Range(-0.7f, 0.7f); 
            tac.Arm.Components[i].TargetAngle = Random.Range(-0.5f, 0.5f); 
        }  




    }

    void SetSpeed(TransformArmController tac){
        tac.Arm.Speed = RobotSpeed; 
    }
}
