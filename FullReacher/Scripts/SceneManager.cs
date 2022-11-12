using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullReacher{


public class SceneManager : MonoBehaviour {


    public Transform Agent; 
    public Transform Effector; 
    public Transform Target; 
    public float TargetSpeed = 0.5f; 
    public float DistanceThreshold; 
    public Transform MinHeight; 
    public Transform Ground; 


    [Header("Initial condition strategies")]
    [Range(0f, 1f)] public float RatioInitSamePlace = 0.5f;


    [Header("Target Movement strategy")]
    public Vector2 AvailableArea;
    public float SmallVariations; 
    public float HeightVariation; 
    public enum TargetMovement {A2B, MoveAround};
    public TargetMovement MovementStrategy; 


    [Header("Constraints")]
    public bool UseBaseDistanceBonus; 
    public ReacherConstraints [] Constraints; 
    public float ConstraintsWeights = 1f;  

    public bool ShowGizmos = true; 

    [SerializeField] float Distance; 
    [SerializeField] float Penalty; 
    [SerializeField] float BaseDistanceBonus; 

    Vector3 NextPos; 
    Vector3 MoveAroundPos; 


    // Use this for initialization
    void Start () {
        SelectNextPos(false); 
        ShowGizmos = false; 
    }
    
    // Update is called once per frame
    void Update () {

        // Distance = Vector3.Distance(Agent.position, Target.position); 
        
    }

    [ContextMenu("Test")]
    void ResetDebug()
    {
        Reset(); 
    }

    void FixedUpdate()
    {
        Target.transform.position = Vector3.MoveTowards(Target.transform.position, NextPos, TargetSpeed*Time.deltaTime);
        if(Vector3.SqrMagnitude(Target.position - NextPos) < 0.4f)
        {
            bool remote_position = MovementStrategy == TargetMovement.A2B ? true : false; 
            SelectNextPos(remote_position); 
        }

        float clamped_x = Mathf.Clamp(Agent.position.x, transform.position.x - AvailableArea.x/2f, transform.position.x + AvailableArea.x/2f); 
        float clamped_z = Mathf.Clamp(Agent.position.z, transform.position.z - AvailableArea.y/2f, transform.position.z + AvailableArea.y/2f); 
        Vector3 Clamped_position = new Vector3(clamped_x, Agent.position.y, clamped_z); 
        Agent.position = Clamped_position;  


    }

    void SelectNextPos(bool remote)
    {
        if(remote)
        {
            Vector3 random_offset = new Vector3(Random.Range(-AvailableArea.x/2.5f, AvailableArea.x/2.5f),
                                                Random.Range(-HeightVariation, HeightVariation),
                                                Random.Range(-AvailableArea.y/2.5f, AvailableArea.y/2.5f));
            NextPos = transform.position + random_offset; 
            NextPos.y = Mathf.Max(Ground.position.y, NextPos.y); 
            MoveAroundPos = NextPos; 
        }
        else{

            float next_y = transform.position.y + Random.Range(0f, HeightVariation); 

            NextPos = new Vector3(MoveAroundPos.x + Random.Range(-SmallVariations, SmallVariations), 
                                  next_y,
                                  MoveAroundPos.z + Random.Range(-SmallVariations, SmallVariations)); 
            NextPos.y = Mathf.Max(Ground.position.y, NextPos.y);
        }
    }

    public float ComputeReward()
    {
        float d = Vector3.Distance(Effector.position, Target.position);
        float reward = d < DistanceThreshold ? 0.1f : -0.01f*d; 

        BaseDistanceBonus = 0f; 
        if(UseBaseDistanceBonus)
        {
            float bonus = 0.1f*Vector3.Distance(Agent.position, Target.position); 
            reward = reward > 0f ? reward + bonus : reward;
            BaseDistanceBonus = bonus;  
        }


        Penalty = 0f; 

        if(Constraints.Length > 0)
        {
            foreach(ReacherConstraints constraint in Constraints){
                reward += constraint.Penalty*ConstraintsWeights; 
                Penalty += constraint.Penalty; 
            }
        }
        Distance = d; 

        return reward; 
    }

    public bool Done()
    {
        return Agent.position.y < MinHeight.position.y; 
    }

    public float[] GetVectorObs()
    {
        float[] vector_obs = new float [6]{0f, 0f, 0f, 0f, 0f, 0f}; 
        Vector3 eff_target = Target.position - Effector.position; 
        Vector3 body_target = Target.position - Agent.position; 

        vector_obs[0] = eff_target.x; 
        vector_obs[1] = eff_target.y; 
        vector_obs[2] = eff_target.z; 
        vector_obs[3] = body_target.x;
        vector_obs[4] = body_target.y;
        vector_obs[5] = body_target.z;

        return vector_obs;  
    }

    [ContextMenu("Reset")]
    public void Reset()
    {

        if(MovementStrategy == TargetMovement.MoveAround)
        {
            SelectNextPos(true); // select remote position 
            Target.position = MoveAroundPos; // sets target to position 
            SelectNextPos(false);  

        }
        else
        {
            SelectNextPos(true); // remote = true 
        }
        
        bool init_same_place = Random.Range(0f, 1f) < RatioInitSamePlace ? true : false;

        if(init_same_place) // using heuristics for easing learning process 
        {
            float current_height = Agent.position.y; 
            Agent.position = new Vector3(Target.position.x, current_height, Target.position.z); 
        } 
        else{
            Vector3 agent_offset =  new Vector3(Random.Range(-AvailableArea.x/2.5f, AvailableArea.x/2.5f),
                                                0f,
                                                Random.Range(-AvailableArea.y/2.5f, AvailableArea.y/2.5f));
            Agent.position = transform.position + agent_offset; 
        }
        
        Agent.rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up); 

        if(Agent.gameObject.GetComponent<Rigidbody>() != null)
        {
            Agent.gameObject.GetComponent<Rigidbody>().velocity *= 0f; 
            Agent.gameObject.GetComponent<Rigidbody>().angularVelocity *= 0f; 
        }
    }

    void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if(ShowGizmos)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(AvailableArea.x, 1f, AvailableArea.y)); 
            // Gizmos.DrawWireCube(Target.position, new Vector3(0.5f, HeightVariation, 0.5f));  
            Gizmos.DrawWireCube(MoveAroundPos, new Vector3(SmallVariations*2f, HeightVariation, SmallVariations*2f)); 
        }
        #endif
    }
}


}
