using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Basket; 

namespace Basket {


public class BasketManager : MonoBehaviour {


    [Header("Environment")]
    public Transform Field; 
    public Vector2 HalfFieldCenterOffset; 
    public float TerrainWidth; 
    public float TerrainDepth; 
    public Transform MinHeight; 
    public Transform HalfFieldCenterReference; 
    public Transform HalfFieldHighReference; 

    [Header("Agent Control")]
    public HCLController Controller; 
    public Transform Center; 
    public Vector3 OffsetCenter; 
    public Vector3 OffsetHands; 
    public float Inversion; 
    [Range(-1f, 1f)] public float HandAngleX; 
    [Range(-1f, 1f)] public float HandAngleZ; 

    [Header("Ball")]
    public Transform Ball; 
    public Vector2 ThrowForce; 
    public Vector2 ThrowAngle; 
    [Range(0f, 1f)] public float RatioDrop; 
    Rigidbody ball_rb; 
    Vector3 initial_ball_position; 


    [Header("Reward")]
    public float SquareDistanceCenterBall; 
    public float Reward; 
    public float EpisodeReward; 

    [Header("Debug")]
    public bool Show; 


	// Use this for initialization
	void Start () {
		
        ball_rb = Ball.gameObject.GetComponent<Rigidbody>(); 
        initial_ball_position = Ball.position; 

	}
	
	// Update is called once per frame
	void Update () {


	}


    void FixedUpdate(){ 
        ClampAgent(); 
		
    }

    public void SetActions(float [] actions){
        OffsetCenter = new Vector3(actions[0], 0f, actions[1]); 
        OffsetHands = new Vector3(actions[2], actions[3], actions[4]);
        Inversion = actions[5];  
        HandAngleX = actions[6]; 
        HandAngleZ = actions[7]; 

        ClampOffsets(); 
    }

    public float [] GetObservations(){

        List <float> obs = new List<float>(); 
        List <Vector3> v_obs = new List<Vector3>(); 

        // Task observations 
        v_obs.Add(ball_rb.velocity); 
        v_obs.Add(Ball.position - HalfFieldCenterReference.position);

        // Body observations  
        float[] controller_obs = Controller.GetObs(); 


        for(int i = 0; i<controller_obs.Length; i ++){
            obs.Add(controller_obs[i]); 
        }

        for(int i = 0; i<v_obs.Count; i++){
            obs.Add(v_obs[i].x);
            obs.Add(v_obs[i].y);
            obs.Add(v_obs[i].z);
        }

        return obs.ToArray(); 

    }
    [ContextMenu("Obs size ? ")]
    public void ObsSize(){
        ball_rb = Ball.gameObject.GetComponent<Rigidbody>(); 
        float[] obs = GetObservations(); 

        Debug.Log("Observation vector has " + obs.Length.ToString() +  " dimensions");
    }

    public bool Done(){
        return Ball.position.y < MinHeight.position.y; 
    }

    [ContextMenu("Reset")]
    public void Reset(){
        float center_height = Center.position.y; 
        Vector3 new_center_position = Field.position + 
            new Vector3(Random.Range(-TerrainWidth/2f, TerrainWidth/2f), center_height, Random.Range(-TerrainDepth/2f, TerrainWidth/2f)); 
        
        Center.position = new_center_position; 

        if(Random.Range(0f, 1f) < RatioDrop)
            DropBall(); 
        else
            ThrowBall(); 

        EpisodeReward = 0f; 
    }


    void ClampAgent(){
        Vector3 field_center = Field.position + new Vector3(HalfFieldCenterOffset.x, 0f, HalfFieldCenterOffset.y); 
        Vector3 clamped_position = Center.position; 
        clamped_position.x = Mathf.Clamp(clamped_position.x, field_center.x - TerrainWidth/2f,field_center.x + TerrainWidth/2f ); 
        clamped_position.z = Mathf.Clamp(clamped_position.z, field_center.z - TerrainDepth/2f,field_center.z + TerrainDepth/2f );
    
        Center.position = clamped_position; 
    }


    void ClampOffsets(){
        OffsetHands.x = Mathf.Clamp(OffsetHands.x, 0.2f, 1f);
        OffsetHands.y = Mathf.Clamp(OffsetHands.y, -1f, 1f);
        OffsetHands.z = Mathf.Clamp(OffsetHands.z, 0.5f, 1f);

        OffsetCenter.x = Mathf.Clamp(OffsetCenter.x, -1f, 1f);
        OffsetCenter.y = Mathf.Clamp(OffsetCenter.y, -1f, 1f);
        OffsetCenter.z = Mathf.Clamp(OffsetCenter.z, -1f, 1f);
    }


    public float ComputeReward(){
        // float distance_target_ball = 0f; 
        float distance_ball_robot = Vector3.SqrMagnitude(Center.position - Ball.position); 
        SquareDistanceCenterBall = distance_ball_robot*0.1f; 
        // float reward_ball_to_target = Mathf.Exp(-distance_ball_robot) + Mathf.Exp(-distance_target_ball); 
        Reward = 0.1f +  1f / (distance_ball_robot * 0.5f); //Mathf.Exp(-distance_ball_robot*0.1f);
        EpisodeReward += Reward;  
        return Reward;
    }


    void ThrowBall(){
        Ball.position = initial_ball_position + new Vector3(Random.Range(-TerrainWidth/2.5f, TerrainWidth/2.5f), 0f ,0f); 
        ball_rb.velocity = Quaternion.AngleAxis(Random.Range(ThrowAngle.x, ThrowAngle.y), Vector3.right) * Vector3.forward*(-Random.Range(ThrowForce.x, ThrowForce.y)); 
        ball_rb.angularVelocity = Vector3.zero; 
    }

    void DropBall(){
        Ball.position = new Vector3(Center.position.x, HalfFieldHighReference.position.y, Center.position.z); 
        // HalfFieldHighReference.position + 
        // new Vector3(Random.Range(-TerrainWidth/2f, TerrainWidth/2f), 0f, Random.Range(-TerrainDepth/2f, TerrainWidth/2f)); 
        ball_rb.velocity = Random.Range(0f, 1f) < 0.5f ? Vector3.zero : -Vector3.forward * ThrowForce.x * Random.Range(0.2f, 0.5f);
        ball_rb.angularVelocity = Vector3.zero;
    }


    void OnGUI(){
        if(GUI.Button(new Rect(10,10,150,20), "Throw"))
            ThrowBall(); 
    }

    void OnDrawGizmos(){
        if(Show){
            Vector3 center_field = Field.position + new Vector3(HalfFieldCenterOffset.x, 1f, HalfFieldCenterOffset.y); 
            Gizmos.DrawWireCube(center_field, new Vector3(TerrainWidth, 1f, TerrainDepth));
        }
    } 
}


}
