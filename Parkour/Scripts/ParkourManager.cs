using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parkour; 
namespace Parkour{

[System.Serializable]
public class VirtualLink{
    public Transform L1; 
    public Transform L2;
    public float Importance = 1f; 

    public VirtualLink(Transform t1, Transform t2){
        L1 = t1; 
        L2 = t2; 
    }

    public VirtualLink(Transform t1, Transform t2, float im){
        L1 = t1; 
        L2 = t2; 
        Importance = im; 
    }

    public float GetDistance(){
        return Vector3.SqrMagnitude(L1.position - L2.position); 
    } 

    public float GetImportance(){
        return Importance; 
    }

} 


public class ParkourManager : MonoBehaviour {

    public SceneManager LeftManager;
    public LegControl LeftLeg;  
    public SceneManager RightManager;
    public LegControl RightLeg;
    public Transform Pelvis; 
    Rigidbody Pelvis_rb; 

    [Header("Control")]
    public int NbRays; 
    public float RayOffsets; 
    public bool ShowRays; 

    [Header("Environment Constraints")]
    public Transform MinHeight; 
    public Transform GoalPoint; 
    public bool DiversifyResetPoints; 
    public float[] ZOffsets; 
    float initial_distance; 

    [Header("Rewards")]
    public float Reward; 
    public float EpisodeReward;
    public enum RewardType {positive_dist, negative_dist, virtual_target_positive_distance, vt_min_speed_pelvis};
    public RewardType RewardStrategy;  
    
    public VirtualLink[] VirtualLinks; 
    public Transform VirtualTarget; 
    public float VirtualTargetSpeed;
    public bool ShowVirtualTarget; 

    Vector3 vt_initial_pos;  

    [HideInInspector] public Vector3 PositionCOM; 
    [HideInInspector] public Rigidbody[] body_parts; 
    public Color COM_Color;
    Vector3 PreviousCOM; 

    Vector3[] all_initial_positions; 
    Quaternion[] all_initial_rotations; 


    // Sustentation 

    Vector3 up_left_corner; 
    Vector3 up_right_corner; 
    Vector3 low_right_corner; 
    Vector3 low_left_corner; 

    [SerializeField] float PELVIS_SPEED; 
    [SerializeField] float[] RAY_VALUES; 


    void Start(){

        RAY_VALUES = new float[NbRays]; 

        Pelvis_rb = Pelvis.gameObject.GetComponent<Rigidbody>(); 

        Rigidbody[] pelvis_parts =  GetComponentsInChildren<Rigidbody>();
        Rigidbody[] ll_parts = LeftLeg.gameObject.GetComponentsInChildren<Rigidbody>();
        Rigidbody[] rl_parts = RightLeg.gameObject.GetComponentsInChildren<Rigidbody>();
        
        body_parts = new Rigidbody[pelvis_parts.Length + ll_parts.Length + rl_parts.Length];
        int counter = 0;
        foreach(Rigidbody rb in pelvis_parts){
            body_parts[counter] = rb; 
            counter ++; 
        } 
        foreach(Rigidbody rb in ll_parts){
            body_parts[counter] = rb; 
            counter ++; 
        }
        foreach(Rigidbody rb in rl_parts){
            body_parts[counter] = rb; 
            counter ++; 
        }


        Transform[] pelvis_transform = GetComponentsInChildren<Transform>();
        Transform[] ll_transform = LeftLeg.gameObject.GetComponentsInChildren<Transform>();  
        Transform[] rl_transform = RightLeg.gameObject.GetComponentsInChildren<Transform>();

        all_initial_positions = new Vector3[pelvis_transform.Length + ll_transform.Length + rl_transform.Length];
        all_initial_rotations = new Quaternion[pelvis_transform.Length + ll_transform.Length + rl_transform.Length];  

        counter = 0; 

        foreach(Transform tr in pelvis_transform){
            // body_parts[counter] = tr; 
            all_initial_positions[counter] = tr.position;
            all_initial_rotations[counter] = tr.rotation;
            counter ++; 
        } 
        foreach(Transform tr in ll_transform){
            all_initial_positions[counter] = tr.position;
            all_initial_rotations[counter] = tr.rotation;
            counter ++; 
        }
        foreach(Transform tr in rl_transform){
            all_initial_positions[counter] = tr.position;
            all_initial_rotations[counter] = tr.rotation;
            counter ++; 
        }

        ComputeCOM(); 

        initial_distance = Vector3.SqrMagnitude(Pelvis.transform.position - GoalPoint.position); 

        if(ShouldMoveTarget()){
            vt_initial_pos = VirtualTarget.position;
        }
    }

    void Update(){
        // ComputeRewards();
        // ComputeCOM();
    }

    void FixedUpdate(){
        ComputeCOM();    
        MoveVirtualTarget(); 
        // ComputeSustentation();  
        // GetObservations(); 
    }

    public bool ShouldMoveTarget(){
        return RewardStrategy == RewardType.virtual_target_positive_distance ||
               RewardStrategy == RewardType.vt_min_speed_pelvis; 
    }

    void MoveVirtualTarget(){

        if(ShouldMoveTarget())

            VirtualTarget.position = Vector3.MoveTowards(VirtualTarget.position, GoalPoint.position, VirtualTargetSpeed*Time.fixedDeltaTime); 

    }

    public float [] GetObservations(){

        List<Vector3> obs = new List<Vector3>(); 
        List<float> obs_f = new List<float>(); 

        Vector3 pelvis_to_target = VirtualTarget.position - Pelvis.transform.position; 
        Vector3 pelvis_ori = Pelvis.transform.rotation.eulerAngles/180f; 
        Vector3 pelvis_com = Pelvis.transform.position - PositionCOM; 
        Vector3 pelvis_speed = Pelvis_rb.velocity; 
        Vector3 pelvis_angular_speed =  Pelvis_rb.angularVelocity; 
        Vector3 com_speed = (PositionCOM - PreviousCOM)/Time.fixedDeltaTime; 
        Vector3 LeftFoot_pelvis = LeftManager.Effector.position - Pelvis.transform.position; 
        Vector3 RightFoot_pelvis = RightManager.Effector.position - Pelvis.transform.position;; 
        Vector3 leftfoot_speed = LeftManager.GetEffectorSpeed(); 
        Vector3 rightfoot_speed = RightManager.GetEffectorSpeed();

        Ray[] rays = new Ray[NbRays]; 
        for(int i = 0; i< NbRays; i ++){
            rays[i] = new Ray(Pelvis.position - Vector3.forward*RayOffsets*(i+1), Vector3.down);  
        }

        int ray_counter = 0;
        foreach(Ray ray in rays){
            RaycastHit hit; 
            Physics.Raycast(ray, out hit, 20f); 
            float height = transform.position.y - hit.point.y; 
            obs_f.Add(height);


            // RAY_VALUES[ray_counter] = height; 
            ray_counter ++;  
        }


        obs.Add(pelvis_to_target); 
        obs.Add(pelvis_ori); 
        obs.Add(pelvis_com);
        obs.Add(pelvis_speed);
        obs.Add(pelvis_angular_speed);
        obs.Add(com_speed);
        obs.Add(LeftFoot_pelvis);
        obs.Add(RightFoot_pelvis);
        obs.Add(leftfoot_speed);
        obs.Add(rightfoot_speed);


        for(int i = 0; i<obs.Count; i++){
            obs_f.Add(obs[i].x);
            obs_f.Add(obs[i].y);
            obs_f.Add(obs[i].z);
        }



        return obs_f.ToArray(); 
    } 

    [ContextMenu("Obs size ? ")]
    public void GetObsSize(){
        Pelvis_rb = Pelvis.gameObject.GetComponent<Rigidbody>(); 
        int length = GetObservations().Length; 
        Debug.Log("Agent will receive obs vector of size " + length.ToString()); 
    }  

    public void SetActions(float [] actions){


        // Fake targets are angles and distance ratio 

        float left_angle_target = actions[0];
        float left_ratio_target = Mathf.Clamp01(actions[1]);
        float right_angle_target = actions[2];
        float right_ratio_target = Mathf.Clamp01(actions[3]);

        LeftManager.SetFakeTarget(left_angle_target, left_ratio_target);
        RightManager.SetFakeTarget(right_angle_target, right_ratio_target);

    }

    public float ComputeRewards(){
        // Reward = 0.1f*(Pelvis.position.y - MinHeight.position.y);
        if(Done()){
            if(RewardStrategy == RewardType.positive_dist)
                Reward = 0f;
            else if(RewardStrategy == RewardType.virtual_target_positive_distance) {
                Reward = 0f; 
            } else {
                Reward = 0f; 
            } 
        }
        else{
            if(RewardStrategy == RewardType.positive_dist){
                Reward = initial_distance - Vector3.SqrMagnitude(Pelvis.position - GoalPoint.position);
                Reward /= initial_distance;  
            }
            else if(RewardStrategy == RewardType.negative_dist){
                Reward = -Vector3.SqrMagnitude(Pelvis.position - GoalPoint.position);
                Reward /= initial_distance;  
            } else if(RewardStrategy == RewardType.virtual_target_positive_distance){
                
                // float r_dist = initial_distance - Vector3.SqrMagnitude(Pelvis.position - GoalPoint.position);
                // r_dist /= initial_distance; 
                // float r_error = Mathf.Exp(-Vector3.SqrMagnitude(Pelvis.position - VirtualTarget.position));
                float r_error = 0f; 
                foreach(VirtualLink vl in VirtualLinks){
                    r_error += Mathf.Exp(-vl.GetDistance()); 
                }
                Reward = r_error;
            } else if(RewardStrategy == RewardType.vt_min_speed_pelvis){
                float r_error = 0f; 
                foreach(VirtualLink vl in VirtualLinks){
                    r_error += Mathf.Exp(-vl.GetDistance()); 
                }
                Reward = r_error + 0.1f*Mathf.Exp(-Vector3.SqrMagnitude(0.1f*Pelvis_rb.velocity));
            } else {
                Reward = 0f; 
            }
        }

        EpisodeReward += Reward; 
        return Reward;  
        // return 0f; 
    }

    float MinimizePelvisSpeedReward(){
        PELVIS_SPEED = Vector3.SqrMagnitude(Pelvis_rb.velocity); 
        return Mathf.Exp(-Vector3.SqrMagnitude(Pelvis_rb.velocity)); 
    }

    public bool Done(){
        return Pelvis.position.y < MinHeight.position.y; 
        // return Pelvis.position.y < DoneHeight.position.y; 
        // return false; 
    }

    [ContextMenu("Reset")]
    public void Reset(){

        // Reset position and rotations 
        Transform[] pelvis_transform = GetComponentsInChildren<Transform>();
        Transform[] ll_transform = LeftLeg.gameObject.GetComponentsInChildren<Transform>();  
        Transform[] rl_transform = RightLeg.gameObject.GetComponentsInChildren<Transform>();

        Vector3 Offset = DiversifyResetPoints ? -Vector3.forward * ZOffsets[Random.Range(0, ZOffsets.Length)] : Vector3.zero; 
        int counter = 0; 
        foreach(Transform tr in pelvis_transform){
            tr.position = all_initial_positions[counter] + Offset; 
            tr.rotation = all_initial_rotations[counter]; 
            counter ++; 
        }
        foreach(Transform tr in ll_transform){
            tr.position = all_initial_positions[counter] + Offset; 
            tr.rotation = all_initial_rotations[counter]; 
            counter ++; 
        }
        foreach(Transform tr in rl_transform){
            tr.position = all_initial_positions[counter] + Offset; 
            tr.rotation = all_initial_rotations[counter]; 
            counter ++; 
        }

        // Reset velocities 
        foreach(Rigidbody rb in body_parts){
            rb.velocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero; 
        }

        // Other variables 

        EpisodeReward = 0f; 
        
        LeftLeg.Reset(); 
        RightLeg.Reset(); 

        if(ShouldMoveTarget()){
            VirtualTarget.position = vt_initial_pos + Offset; 
        }
    }

    // void TestStop(){
    //     Debug.Break(); 
    // }

    public void ComputeCOM(){

        PreviousCOM = PositionCOM; 

        Vector3 pos = Vector3.zero;
        float mass_sum = 0f;  
        for(int i = 0; i<body_parts.Length; i++){
            mass_sum += body_parts[i].mass;
            pos += body_parts[i].gameObject.transform.position*body_parts[i].mass;
        }

        PositionCOM = pos/mass_sum; 
    }

    public void ComputeSustentation(){
        Vector3 l_foot = LeftManager.Effector.position; 
        Vector3 r_foot = RightManager.Effector.position; 

        Vector3 width_vector = -Vector3.ProjectOnPlane(l_foot - r_foot, Vector3.up); 
        Vector3 length_vector = Vector3.Cross(width_vector, Vector3.up).normalized; 

        up_left_corner = l_foot + length_vector/2f + Vector3.up;  
        up_right_corner = up_left_corner + width_vector;
        low_right_corner = up_right_corner - length_vector;
        low_left_corner = low_right_corner - width_vector;
    }

    void OnDrawGizmos(){
        Gizmos.color = COM_Color; 
        Gizmos.DrawSphere(PositionCOM, 0.1f); 

        if(ShowRays){

            Gizmos.color = Color.red; 
            for(int i = 0; i < NbRays;i++ ){
                Gizmos.DrawLine(Pelvis.position - Vector3.forward*(i+1)*RayOffsets, Pelvis.position - Vector3.forward*(i+1)*RayOffsets + Vector3.down*5); 
            }
        }

        if(ShowVirtualTarget){
            Gizmos.color = Color.blue; 
            Gizmos.DrawSphere(VirtualTarget.position, 0.2f); 

            if(VirtualLinks.Length > 0){
                Gizmos.color = Color.yellow; 
                foreach(VirtualLink vl in VirtualLinks){
                    Gizmos.DrawLine(vl.L1.position, vl.L2.position); 
                }
            }
        }

        if(DiversifyResetPoints){
            Gizmos.color = Color.red; 
            if(ZOffsets.Length > 0){
                foreach(float offset in ZOffsets){
                    Gizmos.DrawSphere(vt_initial_pos - Vector3.forward*offset, 0.2f); 
                }
            }
        }

        // DrawSustentation(); 
    }

    // void DrawSustentation(){
    //     // Draw Square 
       
    //     Gizmos.DrawLine(up_left_corner, up_right_corner);
    //     Gizmos.DrawLine(up_right_corner, low_right_corner);
    //     Gizmos.DrawLine(low_right_corner, low_left_corner);
    //     Gizmos.DrawLine(low_left_corner, up_left_corner); 

    //     Gizmos.color = Color.black;
    //     Gizmos.DrawSphere(up_left_corner, 0.1f); 
    //     Gizmos.color = Color.white;
    //     Gizmos.DrawSphere(up_right_corner, 0.1f); 
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawSphere(low_right_corner, 0.1f); 
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawSphere(low_left_corner, 0.1f); 

    //     Vector3 com_0 = PositionCOM; 
    //     com_0.y = up_right_corner.y; 
    //     Gizmos.color = Color.green; 
    //     Gizmos.DrawLine(PositionCOM, com_0); 


    // }

    // void OnGUI(){
    //     Vector3[] data = GetObservations(); 
    //     string recap = ""; 
    //     foreach(Vector3 v in data){
    //         recap += v.ToString() + " \n"; 
    //     }
    //     GUI.TextArea(new Rect(10, 10, 200, 200), recap, 200);
    // }




}
}