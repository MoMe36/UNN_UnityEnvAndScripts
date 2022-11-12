using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tennis; 
using MLAgents; 
using Reachers; 


namespace Tennis{

    public class LinkTennisReacher : MonoBehaviour{

       public Vector3 DirectionForArm; 
       public Transform Center; 
       public Transform Effector; 

       [Header("DEBUG DATA")]
       public TransformArmController ArmController; 
       public GameObject Ball; 
       public bool PaddleRotate; 
       public float Magnitude; 
       [Range(0f,1f)] public float TimeScale = 1f; 

       void Start(){

        Magnitude = Center.gameObject.GetComponent<PaddleControl>().MaxMagnitude; 

       }

       public void Update(){
        
            Time.timeScale = TimeScale; 
            Time.fixedDeltaTime = 0.02f*Time.timeScale; 

            ArmController.Arm.Speed = 0.5f/TimeScale; 

       }
       
       public void SetDirection(Vector3 v){

            DirectionForArm = v; 
       } 

       public Vector3 GetDirection(){

        Vector3 vec = Center.position - Effector.position; 
        Vector3 dir = DirectionForArm; 
        dir *= Magnitude; 
        if(PaddleRotate)
            dir = Quaternion.AngleAxis(180f, Vector3.up)*dir;


        DirectionForArm = vec + dir; 
        // DirectionForArm = dir - vec; 

        // Vector3 center_to_effector = Effector.position - Center.position; 

        Debug.DrawRay(Effector.position, vec, Color.blue, 0.2f); 
        Debug.DrawRay(Center.position, dir, Color.green, 0.2f); 
        Debug.DrawRay(Effector.position, dir + vec, Color.red, 0.2f); 
        return DirectionForArm; 
        // return DirectionForArm; 
       }

       void OnGUI(){
        if(GUI.Button(new Rect(80,10,100,30), "DEBUG MODE")){
            Ball.GetComponent<Rigidbody>().isKinematic = true; 
            ArmController.Arm.Speed = 1f; 
        }
       }

    void OnDrawGizmos(){

        Gizmos.color = Color.red; 
        Gizmos.DrawSphere(Effector.position + DirectionForArm, 0.3f); 


    }

    }

}