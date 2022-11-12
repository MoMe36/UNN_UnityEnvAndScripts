using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reachers; 
using MLAgents; 


namespace Reachers{


    public class DebugTrainedReacher : MonoBehaviour {

        public TransformArmController ArmController;
        public Transform Target;  
        public Transform Center; 
        public Transform Effector; 

        public Vector3 DirectionForArm; 

        public enum PerceptionType {from_effector, from_center, compensate_center, debug_test};
        public PerceptionType Perception;   


        void Start(){

        }

        void Update(){

            if(Perception == PerceptionType.from_effector){
                DirectionForArm = Target.position - Effector.position;   
            }
            else if(Perception == PerceptionType.from_center){
                DirectionForArm = Target.position - Center.position;  
            }
            else if(Perception == PerceptionType.compensate_center){
                Vector3 vec = Effector.position - Center.position; 
                Vector3 dir = Target.position - Center.position;  
                DirectionForArm = dir - vec; 

                Vector3 ec = Center.position - Effector.position; 
                Vector3 ct = DirectionForArm; //Target.position - Center.position; 
                Vector3 full =  ec + ct;

                DirectionForArm = full; 
            }
            else{

            }

        }

        public void SetDirection(Vector3 v){
            DirectionForArm = v; 
        } 

        public Vector3 GetDirection(){

            return DirectionForArm; 
            // Vector3 center_to_effector = Effector.position - Center.position; 
            // return DirectionForArm - center_to_effector; 
        // return DirectionForArm; 
        }

        // void OnGUI(){

        //     if(GUI.Button(new Rect(80,10,100,30), "DEBUG MODE")){
        //     Ball.GetComponent<Rigidbody>().isKinematic = true; 
        //     ArmController.Arm.Speed = 1f; 
        //     }
        // }

        }

}
