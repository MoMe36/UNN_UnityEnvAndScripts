using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Basket; 

namespace Basket {


public class HCLController : MonoBehaviour {

   public BasketManager Manager;
   [Header("Body")] 

   public Transform LeftHand; 
   public Transform RightHand; 
   public Transform Center; 

   public float Speed; 
   public float HandSpeed; 
   public float ArmLength; 

   Vector3 current_right_offset; 
   Vector3 current_left_offset; 
   float current_angle_x;
   float current_angle_z;

   Vector3 previous_left_hand_position; 
   Vector3 previous_right_hand_position; 

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void FixedUpdate(){
      MoveBody(); 
    }

    void MoveBody(){        

      previous_right_hand_position = RightHand.position; 
      previous_left_hand_position = LeftHand.position; 

      float invert = Manager.Inversion > 0f ? -1f : 1f; 
      Vector3 RightOffset = new Vector3(-Manager.OffsetHands.x, Manager.OffsetHands.y*invert, Manager.OffsetHands.z); 

      current_left_offset = Vector3.MoveTowards(current_left_offset, Manager.OffsetHands, HandSpeed*Time.deltaTime); 
      current_right_offset = Vector3.MoveTowards(current_right_offset, RightOffset, HandSpeed*Time.deltaTime); 

      current_angle_x = Mathf.MoveTowards(current_angle_x, Manager.HandAngleX, HandSpeed*Time.deltaTime);
      current_angle_z = Mathf.MoveTowards(current_angle_z, Manager.HandAngleZ, HandSpeed*Time.deltaTime); 

      Center.Translate(Manager.OffsetCenter*Time.deltaTime*Speed); 
      LeftHand.position = Center.position + current_left_offset*ArmLength;
      RightHand.position = Center.position + current_right_offset*ArmLength; 

      LeftHand.rotation = Quaternion.Euler(current_angle_x*90f, 0f, current_angle_z*90f); 
      RightHand.rotation = Quaternion.Euler(current_angle_x*90f, 0f, -current_angle_z*90f);

    }

    public float[] GetObs(){

      List <Vector3> v_obs= new List<Vector3>(); 
      List <float> obs = new List<float>(); 

      Vector3 left_hand_speed = (LeftHand.position - previous_left_hand_position)/Time.fixedDeltaTime; 
      Vector3 right_hand_speed = (RightHand.position - previous_right_hand_position)/Time.fixedDeltaTime; 
      Vector3 left_hand_to_center = LeftHand.position - Center.position; 
      Vector3 right_hand_to_center = RightHand.position - Center.position; 

      v_obs.Add(left_hand_speed); 
      v_obs.Add(right_hand_speed); 
      v_obs.Add(left_hand_to_center);
      v_obs.Add(right_hand_to_center);

      for(int i = 0; i<v_obs.Count; i++){
        obs.Add(v_obs[i].x);
        obs.Add(v_obs[i].y);
        obs.Add(v_obs[i].z);
      }

      obs.Add(current_angle_x); 
      obs.Add(current_angle_z); 

      return obs.ToArray(); 
    }


}


}
