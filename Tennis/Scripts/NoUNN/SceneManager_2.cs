using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tennis; 

namespace Tennis{



public class SceneManager_2 : MonoBehaviour{

    public TransformArmController_Tennis ArmController; 
    public Transform MeasurmentBase; 
    public Transform MinHeight;
    public Transform Effector;  

    public GameObject Ball; 
    Rigidbody ball_rb; 

    public Vector2 InitialForceRangeTowardPaddle;
    public Vector2 InitialForceRangeTowardWall;
    public float LimitsY; 
    public Vector2 LimitsX; 

    [HideInInspector] public float AngleY; 
    [HideInInspector] public float AngleX; 


    public float Reward;
    public float EpisodeReward;  

    [HideInInspector] public bool HasContact; 
    [HideInInspector] public float last_points;

    Vector3 InitialBallPos; 
    Vector3 last_effector_position; 
    Vector3 effector_velocity; 


    [HideInInspector] public float [] targetAngles; 


    void Start(){

        ball_rb = Ball.GetComponent<Rigidbody>(); 
        InitialBallPos = Ball.transform.position; 

        targetAngles = new float [ArmController.GetAngles().Length]; 

    }

    void Update(){

    }

    void FixedUpdate(){
        ComputeEffectorSpeed(); 
    }


    void ComputeEffectorSpeed(){
        effector_velocity = (Effector.position - last_effector_position)/Time.fixedDeltaTime;
        last_effector_position = Effector.position;  
    }

    public bool Done(){
        return Ball.transform.position.y < MinHeight.position.y; 
    }
    public float[] GetObservations(){

        List<float> obs = new List<float>(); 
        List<Vector3> v_obs = new List<Vector3>(); 

        v_obs.Add(Effector.position - MeasurmentBase.position);
        v_obs.Add(Effector.rotation.eulerAngles / 180f);
        v_obs.Add(effector_velocity); 
        v_obs.Add(GetRelativeBallPosition());
        v_obs.Add(GetBallSpeed()); 

        float[] current_angles = ArmController.GetAngles(); 
        for(int i = 0 ;i<current_angles.Length;  i++) 
            obs.Add(current_angles[i]); 
        // obs.Add(ArmController.GetAngles());


        for(int i = 0 ; i<v_obs.Count; i++){
            obs.Add(v_obs[i].x);
            obs.Add(v_obs[i].y);
            obs.Add(v_obs[i].z);
        }

        return obs.ToArray(); 
    }

    public Vector3 GetRelativeBallPosition(){
        return Ball.transform.position - MeasurmentBase.position; 
    }

    public Vector3 GetBallSpeed(){
        return ball_rb.velocity; 
    }

    public void SetActions(float [] actions){
        targetAngles = actions; 
    } 

    public void ContactDetection(NbPoints points){
        last_points = points.Value; 
        HasContact = true; 
    }

    public float ComputeRewards(){

        // Vector3 effector_should_face = Ball.transform.position - Effector.transform.position; 
        // float angle = Vector3.Angle(effector_should_face, Effector.transform.right); 

        // float lateral_offset = Mathf.Abs(Effector.position.z - Ball.transform.position.z); 
        // float vertical_offset = Mathf.Abs(Effector.position.y - Ball.transform.position.y); 
        Reward = Mathf.Exp(-0.05f * Vector3.SqrMagnitude(Effector.position - Ball.transform.position)); 
        if(HasContact)
        {
            HasContact = false; 
            Reward += last_points; 
        }
        

        EpisodeReward += Reward; 
        return Reward; 
    }

    public void Reset(){

        // Debug.Log("Called reset"); 
        Ball.transform.position = InitialBallPos;
        AngleY = Random.Range(-LimitsY, LimitsY); 
        AngleX = Random.Range(LimitsX.x, LimitsY); 

        Quaternion initial_rot = Quaternion.Euler(0f, AngleY, AngleX);
        Vector3 velocity;  
        if(Random.Range(0f, 1f) < 0.5f)
            velocity = Vector3.right*(-Random.Range(InitialForceRangeTowardWall.x, InitialForceRangeTowardWall.y)); 
        else
            velocity = Vector3.right*(Random.Range(InitialForceRangeTowardPaddle.x, InitialForceRangeTowardPaddle.y)); 

        ball_rb.velocity = initial_rot * velocity;

        EpisodeReward = 0f; 
        ArmController.Reset(false); 
    }




}


}