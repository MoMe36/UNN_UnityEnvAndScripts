using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullReacher; 

namespace FullReacher{



public class SimplestBodyController : MonoBehaviour {

    public enum RobotType {holonome, non_holonome,speed_angle}; 
    public RobotType TypeRobot; 
    public bool RotateBody; 

    public Transform Body; 
    public Transform Effector;
    public Transform EffectorAnchor;  
    public float SpeedBody =1f; 
    public float BodyRotationSpeed = 1f; 
    public float SpeedEffector = 1f; 
    public float MagnitudeEffector =1f;

    public Vector3 EffectorOffset;
    public Vector3 BodyDirection; 
    public float MagnitudeThreshold; 


    // Use this for initialization
    void Start () {

    }
    
    // Update is called once per frame
    void FixedUpdate () {

        if(TypeRobot == RobotType.non_holonome)
        {
            if(BodyDirection.magnitude > 0.1f)
            {
                Body.forward = Vector3.RotateTowards(Body.forward, BodyDirection, BodyRotationSpeed*Time.deltaTime, 0f); 
                Body.position += Body.forward*SpeedBody*Time.deltaTime*Mathf.Clamp01(BodyDirection.magnitude); 
            }
        }
        else if(TypeRobot == RobotType.speed_angle)
        {
            if(Mathf.Abs(BodyDirection.z) > 0.3f)
            {
                Quaternion target_rot = Body.rotation*Quaternion.AngleAxis(BodyDirection.x,Vector3.up);
                Body.rotation = Quaternion.Slerp(Body.rotation, target_rot, Time.deltaTime*BodyRotationSpeed);  
                Body.position += Body.forward*SpeedBody*Time.deltaTime*Mathf.Clamp(BodyDirection.z, -1f, 1f); 
            }
        }
        else if(TypeRobot == RobotType.holonome)
        {
            if(BodyDirection.magnitude > 0.3f)
                Body.position += SpeedBody*BodyDirection*Time.deltaTime; 
        }

        
        if(Effector != null)
        {
            Effector.position = Vector3.MoveTowards(Effector.position, EffectorAnchor.position + EffectorOffset, SpeedEffector*Time.deltaTime); 
        }
    }

    public void SetDirections(Vector2 body, Vector3 effector){

        EffectorOffset = effector; 
        BodyDirection = new Vector3(body.x, 0f, body.y); 
        BodyDirection = BodyDirection.magnitude > MagnitudeThreshold ? BodyDirection : Vector3.zero; 

    }


   
}
}
