using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parkour;

namespace Parkour{


public class SceneManager : MonoBehaviour {

    public enum TrainingType {LegOnly, UNN};
    public TrainingType Training; 
    float fake_angle; 
    float fake_dist; 

    [Header("Robot")]
    public LegControl RobotControl; 

    [Header("Reset Parameters")]
    public float MinRadius; 
    public float MaxRadius; 
    public float MinAngle; 
    public float MaxAngle; 

    // public Vector3 TargetPos;
    [Header("Particular Points")] 
    public Transform Target; 
    public Transform Effector; 
    public Transform Head; 
    Vector3 LastEffectorPos; 
    Vector3 NewEffectorPos; 

    [Header("Angles computer")]
    public Transform [] UpToDownJoints; 
    Vector3 [] JointsInitialDir; 

    [Header("ScaleComputer")]
    public Transform HighestPoint; 
    public Transform LowestPoint;
    public float Height; 

    [Header("Reward")]
    public Transform [] AgentPoints; 
    public Transform [] TargetPoints;  


    [Header("DEBUG")]
    public float SCALED_DISTANCE; 
    public float SCALED_ANGLE; 
    public bool Show; 

    // Use this for initialization
    void Start () {


        // Initialize angles 


        JointsInitialDir = new Vector3 [UpToDownJoints.Length-1]; 
        for(int i = 0 ; i < JointsInitialDir.Length; i ++){
            JointsInitialDir[i]= UpToDownJoints[i+1].position - UpToDownJoints[i].position;  
        }
        
    }

    void FixedUpdate(){
        UdpateEffectorSpeed(); 
    }

    void UdpateEffectorSpeed(){
        LastEffectorPos = NewEffectorPos; 
        NewEffectorPos = Effector.position; 
    }

    public void SetFakeTarget(float angle, float distance){
        fake_angle = angle; 
        fake_dist = distance; 
    }

    public Vector3 GetEffectorSpeed(){
        return (NewEffectorPos - LastEffectorPos)/Time.fixedDeltaTime; 
    }

    public float [] GetObservations(){

        // First: Angles 
        float[] angles = GetAngles(); 

        // Second: To target 
        float scaled_dist, scaled_angle;
        if(Training == TrainingType.LegOnly){
            Vector3 head_target = Target.position - Head.position; 
            scaled_dist = head_target.magnitude / Height; 
            scaled_angle = Vector3.SignedAngle(Vector3.forward, head_target, Vector3.right)/180f; 
        } else {
            scaled_angle = fake_angle; 
            scaled_dist = fake_dist;
        }

        SCALED_ANGLE = scaled_angle;
        SCALED_DISTANCE = scaled_dist;

        return new float [] {angles[0]/180f, angles[1]/180f, scaled_dist, scaled_angle}; 
    }

    public float[] GetAngles(){
        float thigh_angle = Vector3.SignedAngle(JointsInitialDir[0], UpToDownJoints[1].position - UpToDownJoints[0].position, Vector3.right); 
        float knee_angle = Vector3.SignedAngle(JointsInitialDir[1], UpToDownJoints[2].position - UpToDownJoints[1].position, Vector3.right) - thigh_angle; 
        // Debug.Log("Thigh angle: " + thigh_angle.ToString() + "  Knee angle: " + knee_angle.ToString()); 
        return new float []{thigh_angle, knee_angle}; 
    }



    public float ComputeReward(){
        float r_dist = ComputeDistances();

        return -r_dist;  
    }

    float ComputeDistances(){
        float dist = 0f; 
        for(int i = 0; i< AgentPoints.Length; i++){
            dist += Vector3.SqrMagnitude(AgentPoints[i].position - TargetPoints[i].position); 
        }
        return dist; 
    }

    [ContextMenu("Compute Height")]
    public void ComputeHeight(){
        Height = Mathf.Abs(HighestPoint.position.y - LowestPoint.position.y);
    }   

    [ContextMenu("Reset")]
    public void Reset(){
        ResetTarget(); 
    }   

    void ResetTarget(){

        float radius = Random.Range(MinRadius, MaxRadius); 
        float angle = Random.Range(MinAngle, MaxAngle);
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.right);  
        Target.position = transform.position + rot * Vector3.down * radius;  

        // Target.position = TargetPos; 
    }

    void OnDrawGizmos(){

        if(Show){
            Gizmos.color = Color.blue; 
            DrawArc(MinRadius, MinAngle, MaxAngle); 
            
            Gizmos.color = Color.green; 
            DrawArc(MaxRadius, MinAngle, MaxAngle); 
        }

        // Gizmos.DrawSphere(TargetPos, 0.2f); 

    }


    void DrawArc(float r, float min_a, float max_a){

        int nb_points= 50;
        float inc = (max_a - min_a)/(float)nb_points;

        Vector3 p0 = Quaternion.AngleAxis(min_a, Vector3.right) * Vector3.down*r; 
        Vector3 p1; 
        for(int i = 0; i < nb_points; i++){
            p1 =  Quaternion.AngleAxis(inc, Vector3.right) * p0;
            Gizmos.DrawLine(transform.position + p0, transform.position + p1);
            p0 = p1; 
        } 

    }
}


}
