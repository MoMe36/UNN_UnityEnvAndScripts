using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tennis; 

namespace Tennis{


public class PaddleControl : MonoBehaviour {

    public Transform Center; 
    public Transform Paddle; 

    Rigidbody rb_paddle; 

    [Header("Position")]
    public float MovementSpeed; 
    public float MaxMagnitude; 
    public Vector3 Offset; 
    Vector3 offset; 

    [Header("Override Position")]
    public bool DoOverride; 
    public Transform ToFollow; 

    [Header("Rotation")]
    public float RotationSpeed; 
    public Vector2 MaxAngleX;
    public Vector2 MaxAngleZ; 
    public float AngleX; 
    public float AngleZ; 

    [Header("DEBUG")]
    public bool UseMoveRB; 

    float angle_x; 
    float angle_z; 

    Quaternion InitialPaddleRotation; 
    Vector3 InitialPaddlePosition; 
    Vector3 InitialOffset; 

    [SerializeField] Vector2 AnglesFromUNN; 
    // Use this for initialization
    void Start () {
        
        rb_paddle = Paddle.gameObject.AddComponent<Rigidbody>(); 
        rb_paddle.isKinematic = true; 

        InitialPaddlePosition = Paddle.position; 
        InitialPaddleRotation = Paddle.rotation;
        InitialOffset = (Paddle.position - Center.position)/MaxMagnitude; 

        Offset = InitialOffset; 
        offset = Offset;   

    }
    
    // Update is called once per frame
    void Update () {

        
    }

    public void UpdatePaddle(){

        angle_x = Mathf.MoveTowards(angle_x, AngleX, RotationSpeed*Time.deltaTime); 
        angle_z = Mathf.MoveTowards(angle_z, AngleZ, RotationSpeed*Time.deltaTime);
        Paddle.rotation = InitialPaddleRotation*Quaternion.Euler(angle_x, 0f, angle_z);
        
        // rb_paddle.WakeUp(); 

        if(DoOverride){
            Paddle.position = ToFollow.position; 
            Offset = ToFollow.position - Center.position;  
            offset = Offset; 

        }
        else{
            
            offset = Vector3.MoveTowards(offset, Offset, Time.deltaTime*MovementSpeed);  
            if(UseMoveRB)
                rb_paddle.MovePosition(Center.position + Quaternion.AngleAxis(180f, Vector3.up) * offset*MaxMagnitude); 
            else
                Paddle.position = Center.position + Quaternion.AngleAxis(180f, Vector3.up) * offset*MaxMagnitude; 
        }

        

    }

    float Remap(float current_val, float min_a, float max_a, float min_b, float max_b){

        float new_val = (((current_val - min_a) * (max_b - min_b)) / (max_a - min_a)) + min_b; 

        return new_val; 
    }

    public void ReceiveAction(Vector3 offset_target, float angle_x_target, float angle_z_target){

        AnglesFromUNN.x = angle_x_target; 
        AnglesFromUNN.y = angle_z_target; 
 
        if(Mathf.Abs(angle_x_target - AngleX) <5)
            AngleX = AngleX; 
        else
            AngleX = Remap(angle_x_target, -1f, 1f, MaxAngleX.x, MaxAngleX.y);
        
        if(Mathf.Abs(angle_x_target - AngleX) <5)
            AngleZ = AngleZ; 
        else
            AngleZ = Remap(angle_z_target, -1f, 1f, MaxAngleZ.x, MaxAngleZ.y);
        

        Vector3 processed_offset = (offset_target - Offset).sqrMagnitude < 0.3f*0.3f ? Offset : offset_target; 
        Offset = processed_offset;   
    }   


    public Vector3 GetRelativePaddlePosition(){
        return Paddle.position - Center.position; 
    }

    public Vector2 GetPaddleOrientation(){
        return new Vector2(angle_x, angle_z); 
    }

    public Vector3 GetPaddleSpeed(){
        return rb_paddle.velocity; 
    }


    [ContextMenu("Reset")]
    public void Reset(){

        // Offset = Quaternion.Euler(0f, Random.Range(-180f, 180f), Random.Range(-90f, 90f)) * InitialOffset*Random.Range(0f, 1f); 
        Offset = InitialOffset*Random.Range(0f, 1f); 
        offset = Offset;   

        AngleX = Random.Range(MaxAngleX.x, MaxAngleX.y);   
        AngleZ = Random.Range(30f, MaxAngleZ.y); 
        angle_x = AngleX;
        angle_z = AngleZ; 

    }

    public void ResetToEffector(){
        Reset();
        Offset = Vector3.zero; 
        offset = Offset; 
    }

    
}
}

