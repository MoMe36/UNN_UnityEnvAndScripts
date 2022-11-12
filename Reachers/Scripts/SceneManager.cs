using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reachers; 

namespace Reachers{


public class SceneManager : MonoBehaviour{

    public TransformArmController ArmController; 

    public Transform RobotBase; 
    public Transform Target; 
    public Transform Effector; 
    public float TargetSpeed; 

    public enum TargetMovementStrategy {random, orbiting}; 
    public TargetMovementStrategy TargetMovement; 
    public float DistanceThreshold; 
    public float ReachableRadius; 

    [Header("Orbiting strategy")]
    public float LowOrbitHeight; 
    public float LowOrbitRadius; 
    public float MediumOrbitHeight; 
    public float MediumOrbitRadius; 
    public float HighOrbitHeight; 
    public float HighOrbitRadius;
    public Vector2 RotationSpeedLimits;  
    public Vector2 LerpingSpeedLimits; 

    public enum RewardStrategies {basic, limit_angle}; 
    public RewardStrategies RewardStrategy; 

    [Range(0f, 1f)] public float RandomizeResetRatio; 

    [SerializeField] float Distance; 
    [SerializeField] float LastReward; 

    Vector3 NextPos; 
    [Header("Target position")]
    public float NextRadius; 
    public float NextHeight; 
    float RotationDirection; 

    float current_radius; 
    float current_height; 
    float current_angle; 
    float RotationSpeed; 
    float LerpingSpeed; 

    void Start()
    {
        SelectNextPos(); 
        if(TargetMovement == TargetMovementStrategy.orbiting)
        {
            current_angle = 0f; 
            current_height = LowOrbitHeight; 
            current_radius = LowOrbitRadius; 
            RotationDirection = 1f; 
        }
    }

    void Update()
    {
        ArmController.UpdateArm();
    }


    void FixedUpdate()
    {

        if(TargetMovement == TargetMovementStrategy.random){
            Target.transform.position = Vector3.MoveTowards(Target.transform.position, NextPos, TargetSpeed*Time.deltaTime); 
            if(Vector3.SqrMagnitude(Target.position - NextPos) < 0.4f)
            {
                SelectNextPos(); 
            }
        }
        else{

            current_radius = Mathf.MoveTowards(current_radius, NextRadius, Time.deltaTime*LerpingSpeed); 
            current_height = Mathf.MoveTowards(current_height, NextHeight, Time.deltaTime*LerpingSpeed);
            current_angle += RotationDirection*Time.deltaTime*RotationSpeed;
            Vector3 next_orbit_pos = Quaternion.AngleAxis(current_angle, Vector3.up) * Vector3.forward*current_radius;
            next_orbit_pos.y = current_height; 
            Target.transform.position =  transform.position + next_orbit_pos; 

            if(Vector2.SqrMagnitude(new Vector2(current_radius, current_height) - new Vector2(NextRadius, NextHeight)) < 0.2f)
            {
                SelectNextPos(); 
            }

        }
    }

    public float ComputeReward()
    {
        Distance = Vector3.Distance(Target.position, Effector.position); 
        float reward = 0f; 

        if(RewardStrategy == RewardStrategies.basic)
        {
            reward = Distance < DistanceThreshold ? 0.1f : -Distance; 
        }
        else if (RewardStrategy == RewardStrategies.limit_angle)
        {
            float angle_measure = Vector3.Angle(Vector3.ProjectOnPlane(Target.position - RobotBase.position, Vector3.up), RobotBase.forward); 
            if(angle_measure < 30f)
            {
                reward = Distance < DistanceThreshold ? 0.1f : -Distance;
            }
            else
            {
                reward = -Distance; //Distance < DistanceThreshold ? 0f : -0.1f*Distance;
            }
        }

        LastReward = reward; 
        return reward; 
    }

    [ContextMenu("SelectNextPos")]
    void SelectNextPos()
    {
        if(TargetMovement == TargetMovementStrategy.random){
            float y_angle = Random.Range(-180f, 180f); 
            float z_angle = Random.Range(0f, 90f); 
            float magn = Random.Range(0f, 1f) > 0.5f ? 0.9f : 0.4f;  
            Vector3 Offset = Quaternion.AngleAxis(y_angle, Vector3.up)*Quaternion.AngleAxis(z_angle, Vector3.forward)*Vector3.right*magn*ReachableRadius; 
            NextPos = transform.position + Offset;  
        }
        else{

            bool high = Random.Range(0f, 1f) > 0.5f ? true: false; 
            float ratio = Random.Range(0f, 1f);  
            
            if(high){
                NextHeight = ratio*HighOrbitHeight + (1f-ratio)*MediumOrbitHeight; 
                NextRadius = Random.Range(HighOrbitRadius, ratio*HighOrbitRadius + (1f-ratio)*MediumOrbitRadius);
            }
            else{

                NextHeight = ratio*MediumOrbitHeight + (1f-ratio)*LowOrbitHeight; 
                NextRadius = Random.Range(LowOrbitRadius, ratio*MediumOrbitRadius + (1f-ratio)*LowOrbitRadius);
            }

            LerpingSpeed = Random.Range(LerpingSpeedLimits.x,LerpingSpeedLimits.y);
            RotationSpeed = Random.Range(RotationSpeedLimits.x,RotationSpeedLimits.y);
            if(Random.Range(0f, 1f) < 0.2f){
                float value_ = Random.Range(0f,1f ) < 0.5f ? 90f: 270f; 
                current_angle = (current_angle + value_) % 360f; 
            }
            
        }
    }

    public float GetDistance(){
        return Distance; 
    }

    public void Reset()
    {
        if(TargetMovement == TargetMovementStrategy.random){
            bool randomize_reset = Random.Range(0f, 1f) < RandomizeResetRatio ? true : false; 
            ArmController.Reset(randomize_reset); 
            Target.transform.position = randomize_reset ? Effector.transform.position : Target.transform.position; 
        }
        else{
            bool randomize_reset = Random.Range(0f, 1f) < RandomizeResetRatio ? true : false; 
            ArmController.Reset(randomize_reset);
            RotationDirection = -1f*RotationDirection;
            current_angle = Random.Range(0f, 360f);  
        }
    }



    void OnDrawGizmos()
    {
        #if UNITY_EDITOR

        if(TargetMovement == TargetMovementStrategy.orbiting){
            DrawOrbits(LowOrbitHeight, LowOrbitRadius, 50); 
            DrawOrbits(HighOrbitHeight, HighOrbitRadius, 50); 
            DrawOrbits(MediumOrbitHeight, MediumOrbitRadius, 50); 
        }
        else
            Gizmos.DrawWireSphere(transform.position, ReachableRadius);

        #endif

        // Gizmos.color = Color.yellow; 
        // Vector3 next_orbit_pos = Quaternion.AngleAxis(current_angle, Vector3.up) * Vector3.forward*NextRadius; 
        // next_orbit_pos.y = NextHeight;
        // next_orbit_pos = transform.position + next_orbit_pos; 
        // Gizmos.DrawSphere(next_orbit_pos, 0.2f); 
    }

    void DrawOrbits(float height, float radius, int nb_points){

        Gizmos.color = Color.red; 
        float theta_inc = 360f/(float)nb_points; 
        Vector3 p0 = Vector3.forward*radius; 
        p0.y = height; 
        Vector3 p1; 
        for(int i = 0; i < nb_points; i++){

            p1 = Quaternion.AngleAxis(theta_inc, Vector3.up)*p0; 
            Gizmos.DrawLine(transform.position + p0, transform.position + p1); 
            p0 = p1; 

        }

    }


}


}