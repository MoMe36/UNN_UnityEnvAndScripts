using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parkour; 
namespace Parkour {

public class LegControl : MonoBehaviour {


    public Transform ArmatureTransform; 

    public ConfigurableJoint Thigh; 
    public ConfigurableJoint Knee;  


    [Range(-1f, 1f)] public float ThighAngle; 
    [Range(-1f, 1f)] public float KneeAngle;

    public float SpringForce = 1000; 
    public float Damper = 150; 
    public float AngleLimit = 120f; 
    public float LerpTorqueSpeed = 5f;  

    JointDrive Spring300; 
    // Use this for initialization
    void Start () {
        

        Spring300 = new JointDrive();
        Spring300.positionSpring = SpringForce;
        Spring300.positionDamper = Damper;
        Spring300.maximumForce = Mathf.Infinity;

    }

    public void SetTorques(float thigh, float knee){
        // Debug.Log("Thigh angle " + thigh.ToString() + " Knee angle " + knee.ToString()); 
        ThighAngle = Mathf.MoveTowards(ThighAngle, thigh, LerpTorqueSpeed*Time.fixedDeltaTime); 
        KneeAngle = Mathf.MoveTowards(KneeAngle, knee, LerpTorqueSpeed*Time.fixedDeltaTime);  
    }
    
    // Update is called once per frame

    void FixedUpdate(){
        Quaternion rot_thigh = Quaternion.Euler(new Vector3 (ThighAngle * AngleLimit, 0f, 0f));
        Quaternion rot_knee = Quaternion.Euler(new Vector3 (KneeAngle * AngleLimit, 0f, 0f)); 

        Thigh.angularXDrive = Spring300; 
        Thigh.angularYZDrive = Spring300; 
        Thigh.targetRotation = rot_thigh; 

        Knee.angularXDrive = Spring300; 
        Knee.angularYZDrive = Spring300; 
        Knee.targetRotation = rot_knee;
    }

    public void Reset(){
        // SetTorques(0f, 0f);
        ThighAngle = 0f; 
        KneeAngle = 0f;  
    }

   

}
}