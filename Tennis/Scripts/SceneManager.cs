using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tennis{


public class SceneManager : MonoBehaviour {

    public Transform MinHeight; 
    public Transform Center; 

    public GameObject Ball; 
    public Vector2 InitialForceRangeTowardPaddle;
    public Vector2 InitialForceRangeTowardWall;
    public float LimitsY; 
    public Vector2 LimitsX; 

    [HideInInspector] public float AngleY; 
    [HideInInspector] public float AngleX; 

    [Header("Reward")]
    [HideInInspector] public bool HasContact; 
    [HideInInspector] public float last_points;
    public float EpisodeReward;  

    Rigidbody ball_rb; 
    Vector3 InitialBallPos; 

	// Use this for initialization
	void Start () {
		  
          ball_rb = Ball.GetComponent<Rigidbody>(); 
          InitialBallPos = Ball.transform.position; 
	}
	
	// Update is called once per frame
	void Update () {
		

        ComputeRewards(); 


	}

    public Vector3 GetRelativeBallPosition(){
        return Ball.transform.position - Center.position; 
    }

    public Vector3 GetBallSpeed(){
        return ball_rb.velocity; 
    }

    public float ComputeRewards(){
        
        if(HasContact)
        {
            HasContact = false; 
            EpisodeReward += last_points; 
            return last_points; 
        }
        else
        {
            return 0f; 
        }

    }

    public void ContactDetection(NbPoints points){
        last_points = points.Value; 
        HasContact = true; 
    }

    public bool Done(){
        return Ball.transform.position.y < MinHeight.position.y; 
    }

    void OnGUI(){
        if(GUI.Button(new Rect(10,10,50,30), "Reset"))
            Reset(); 
    }

    [ContextMenu("Reset")]
    public void Reset(){

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
    }
}
}

