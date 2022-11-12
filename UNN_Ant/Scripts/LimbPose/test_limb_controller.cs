using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Limb{

    public HingeJoint Jy; 
    public HingeJoint Jz; 
    public HingeJoint J1;

    public float interp_speed; 
    SO_Pose target_pose; 

    public void SetTargetPose(SO_Pose pose){

        target_pose = pose; 
    }

    public void ChangePose(SO_Pose pose){

        Jy.spring = JointHandler.AdaptHinge(Jy, pose.JY);
        Jz.spring = JointHandler.AdaptHinge(Jz, pose.JZ);
        J1.spring = JointHandler.AdaptHinge(J1, pose.J1);


    }

    public void UpdateLimb(){
        SO_Pose intermediate_pose = ScriptableObject.CreateInstance<SO_Pose>(); //new SO_Pose();
        Debug.Log(target_pose.JY); 
        intermediate_pose.JY = Mathf.MoveTowards(Jy.angle, target_pose.JY, Time.deltaTime*interp_speed); 
        intermediate_pose.JZ = Mathf.MoveTowards(Jz.angle, target_pose.JZ, Time.deltaTime*interp_speed); 
        intermediate_pose.J1 = Mathf.MoveTowards(J1.angle, target_pose.J1, Time.deltaTime*interp_speed); 

        ChangePose(intermediate_pose); 
    }



}

public class JointHandler{

    public static JointSpring AdaptHinge(HingeJoint target_hinge, float target_angle){
        JointSpring js = target_hinge.spring; 
        js.targetPosition = target_angle; 
        return js; 
    }

}

public class test_limb_controller : MonoBehaviour {

    public Limb Limbs; 
    public SO_Pose [] poses; 
    public int go_to_pose; 

    int pose_nb; 
	// Use this for initialization
	void Start () {
		
        Limbs.SetTargetPose(poses[0]); 
	}
	
	// Update is called once per frame
	void Update () {

        go_to_pose = Mathf.Clamp(go_to_pose, 0, poses.Length -1);
        if(pose_nb != go_to_pose)
        {
            pose_nb = go_to_pose;
            Limbs.SetTargetPose(poses[pose_nb]); 
        }

        Limbs.UpdateLimb(); 
		
	}

    void OnGUI(){
        if(GUI.Button(new Rect(10,10,30,30), "+"))
        {
            go_to_pose = (go_to_pose + 1)%poses.Length;
        }

        if(GUI.Button(new Rect(10,50,30,30), "-"))
        {
            go_to_pose = go_to_pose - 1; 
            if(go_to_pose < 0)
                go_to_pose = poses.Length-1; 
        }
    }
}
