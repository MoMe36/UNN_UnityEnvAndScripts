using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parkour; 
namespace Parkour{





public class ResistManager : MonoBehaviour {

    public SceneManager LeftManager;
    public LegControl LeftLeg;  
    public SceneManager RightManager;
    public LegControl RightLeg;
    public Transform Pelvis; 
    Rigidbody Pelvis_rb; 

    [Header("Environment Constraints")]
    public Transform MinHeight; 
    public float MaxForceMagnitude; 
    public float TimeBetweenImpulsions; 
    float between_impulses; 
    // public GameObject[] Platforms;
    // public Transform [] StartStopConvey;  
    // public float ConveyorForce; 
    // Rigidbody[] platforms_rb; 

    [Header("Targets")]
    public Transform TargetAgentPelvis; 
    public GameObject TargetAgent; 
    public GameObject LearningAgent; 
    public VirtualLink[] VirtualLinks; 
    public bool ShowVirtualTarget; 

    [Header("Waypoints")]
    public bool ShowWaypoints; 
    public bool UseWaypoints;
    public bool ResetAtWP; 
    public bool AsStaticPoint;  
    public float TimeForStaticWP; 
    public float WP_DistanceMultiplier = 0.02f; 
    public Transform RootWaypoint;
    public float SpeedWaypoints;  
    public Transform WaypointTarget; 
    int current_waypoint; 
    float time_before_change_wp; 
    Transform [] waypoints; 

    [Header("Rewards")]
    public float Reward; 
    public float EpisodeReward;
    

    [HideInInspector] public Vector3 PositionCOM; 
    [HideInInspector] public Rigidbody[] body_parts; 
    public Color COM_Color;
    Vector3 PreviousCOM; 

    Vector3[] all_initial_positions; 
    Quaternion[] all_initial_rotations; 


    [SerializeField] float PELVIS_SPEED; 
    // [SerializeField] float DEBUG_MULTIPLIER; 
    // public float MutiplierDistanceRatio = 0.1f;  


    void Start(){

        Pelvis_rb = Pelvis.gameObject.GetComponent<Rigidbody>(); 

        Rigidbody[] pelvis_parts =  gameObject.GetComponentsInChildren<Rigidbody>();
        Rigidbody[] ll_parts = LeftLeg.gameObject.GetComponentsInChildren<Rigidbody>();
        Rigidbody[] rl_parts = RightLeg.gameObject.GetComponentsInChildren<Rigidbody>();
        
        body_parts = new Rigidbody[pelvis_parts.Length + ll_parts.Length + rl_parts.Length];
        int counter = 0;
        foreach(Rigidbody rb in pelvis_parts){
            body_parts[counter] = rb; 
            // Debug.Log(rb); 
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


        // Debug.Break(); 

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
        between_impulses = TimeBetweenImpulsions; 


        GetWaypoints(); 
        // if(Platforms.Length > 0){
        //     platforms_rb = new Rigidbody[Platforms.Length];
        //     for(int i= 0; i< platforms_rb.Length; i++){
        //         platforms_rb[i] = Platforms[i].GetComponent<Rigidbody>(); 
        //     }
        // }


    }

    void Update(){
        // ComputeRewards();
        // ComputeCOM();
    }

    void FixedUpdate(){
        ComputeCOM();    
        // ComputeSustentation();  
        // GetObservations(); 
        // MovePlatforms(); 
        ApplyImpulse(); 
        if(UseWaypoints)
            MoveTarget(); 
    }

    void MoveTarget(){

        if(AsStaticPoint){

            time_before_change_wp -= Time.fixedDeltaTime; 
            if(time_before_change_wp < 0f){
                time_before_change_wp = TimeForStaticWP; 
                current_waypoint = (current_waypoint + 1)%waypoints.Length;
                WaypointTarget.position = waypoints[current_waypoint].position; 
            }


        } else{
            WaypointTarget.position = Vector3.MoveTowards(WaypointTarget.position, waypoints[current_waypoint].position,
                                                             Time.fixedDeltaTime * SpeedWaypoints); 

            if(Vector3.SqrMagnitude(WaypointTarget.position - waypoints[current_waypoint].position) < 0.5f){
                current_waypoint = (current_waypoint + 1)%waypoints.Length; 
            }
        }
    }

    void ApplyImpulse(){

        between_impulses -= Time.fixedDeltaTime; 
        if(between_impulses < 0f)
        {
            float angle = Random.Range(0f, 360f); 
            Vector3 force = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward*Random.Range(0f,1f)*MaxForceMagnitude;
            Pelvis_rb.AddForce(force, ForceMode.Impulse);  

            Debug.DrawRay(Pelvis.position, force, Color.red, 1f); 
            Debug.DrawRay(Pelvis.position + force, -force.normalized + Vector3.up, Color.red, 1f);
            Debug.DrawRay(Pelvis.position + force, -force.normalized + Vector3.down, Color.red, 1f); 
            between_impulses = TimeBetweenImpulsions; 
        }

    }

    // void MovePlatforms(){
    //     if(Platforms.Length > 0){
    //         platforms_rb[0].AddForce(Vector3.forward*ConveyorForce*Time.fixedDeltaTime); 
    //         for(int i =0; i<Platforms.Length; i++){
    //             Debug.Log("Plateforme " + i.ToString() + " Z: " + Platforms[i].transform.position.z.ToString() +
    //              " Max: " + StartStopConvey[1].position.z.ToString()); 
    //             if(Platforms[i].transform.position.z > StartStopConvey[1].position.z)
    //                 Platforms[i].transform.position = StartStopConvey[0].position; 
    //         }
    //     }
    // }

    public float [] GetObservations(){

        List<Vector3> obs = new List<Vector3>(); 
        List<float> obs_f = new List<float>(); 

        Vector3 pelvis_to_target = UseWaypoints ? WaypointTarget.position -  Pelvis.transform.position : TargetAgentPelvis.position - Pelvis.transform.position; 
        Vector3 pelvis_ori = Pelvis.transform.rotation.eulerAngles/180f; 
        Vector3 pelvis_com = Pelvis.transform.position - PositionCOM; 
        Vector3 pelvis_speed = Pelvis_rb.velocity; 
        Vector3 pelvis_angular_speed =  Pelvis_rb.angularVelocity; 
        Vector3 com_speed = (PositionCOM - PreviousCOM)/Time.fixedDeltaTime; 
        Vector3 LeftFoot_pelvis = LeftManager.Effector.position - Pelvis.transform.position; 
        Vector3 RightFoot_pelvis = RightManager.Effector.position - Pelvis.transform.position;; 
        Vector3 leftfoot_speed = LeftManager.GetEffectorSpeed(); 
        Vector3 rightfoot_speed = RightManager.GetEffectorSpeed();

        // Debug.DrawRay(Pelvis.transform.position, pelvis_to_target, Color.green,  0.2f); 
      
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

    [ContextMenu("Set targets")]
    public void AutomaticTargetDetection(){
        AnimTarget[] on_target = TargetAgent.GetComponentsInChildren<AnimTarget>(); 
        AnimTarget[] on_agent = LearningAgent.GetComponentsInChildren<AnimTarget>();

        Dictionary<string, AnimTarget> dict_target = new Dictionary<string,AnimTarget>(); 
        Dictionary<string, float> dict_weights = new Dictionary<string, float>(); 

        for(int i = 0; i<on_target.Length; i++){
            dict_target.Add(on_target[i].Name, on_target[i]);
            dict_weights.Add(on_target[i].Name, on_target[i].Weight);  
        }


        List<VirtualLink> vt_links = new List<VirtualLink>(); 
        for(int i = 0; i<on_agent.Length; i++){
            Transform t1 = on_agent[i].gameObject.transform; 
            Transform t2 = dict_target[on_agent[i].Name].gameObject.transform; 

            VirtualLink new_link = new VirtualLink(t1, t2, dict_weights[on_agent[i].Name]); 
            // vt_links.Add(new VirtualLink(t1,t2));
            vt_links.Add(new_link);  
        }

        VirtualLinks = vt_links.ToArray(); 

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
            Reward = 0f; 
        }
        else{
            float r_error = 0f; 
            foreach(VirtualLink vl in VirtualLinks){
                r_error += Mathf.Exp(-5f*vl.GetDistance())*vl.GetImportance(); 
            }

            float waypoint_multiplier = 1f; 
            if(UseWaypoints){
                // DEBUG_MULTIPLIER = Mathf.Exp(-WP_DistanceMultiplier* Vector3.SqrMagnitude(Pelvis.position - TargetAgentPelvis.position)); 
                waypoint_multiplier += Mathf.Exp(-WP_DistanceMultiplier * Vector3.SqrMagnitude(Pelvis.position - TargetAgentPelvis.position));
            }
            Reward = 0.1f + waypoint_multiplier * MinimizePelvisSpeedReward(); 
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

        int counter = 0; 

        Vector3 offset_waypoint = waypoints[Random.Range(0, waypoints.Length)].position - all_initial_positions[0]; 
        offset_waypoint = Vector3.ProjectOnPlane(offset_waypoint, Vector3.up); 

        Vector3 Offset = ResetAtWP ? offset_waypoint : Vector3.zero; 
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

    }

    [ContextMenu("Get waypoints")]
    void GetWaypoints(){
        Transform[] waypoints_with_parent = RootWaypoint.gameObject.GetComponentsInChildren<Transform>(); 
        waypoints =  new Transform[waypoints_with_parent.Length -1]; 
        for(int i = 0; i < waypoints.Length; i ++){
            waypoints[i] = waypoints_with_parent[i+1]; 
        }
        Debug.Log("Found " + waypoints.Length.ToString() + " waypoints"); 
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


    void OnDrawGizmos(){
        Gizmos.color = COM_Color; 
        Gizmos.DrawSphere(PositionCOM, 0.1f); 


        if(ShowVirtualTarget){
            Gizmos.color = Color.blue; 

            if(VirtualLinks.Length > 0){
                Gizmos.color = Color.yellow; 
                foreach(VirtualLink vl in VirtualLinks){
                    Gizmos.DrawLine(vl.L1.position, vl.L2.position); 
                }
            }
        }

        if(ShowWaypoints){
            Gizmos.color = Color.red; 
            if(UseWaypoints){
                if(waypoints != null){
                    for(int i = 0; i < waypoints.Length; i++){
                        Gizmos.DrawLine(waypoints[i].position, waypoints[(i+1)%waypoints.Length].position);
                    }
                } else {
                    GetWaypoints(); 
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

    void OnGUI(){

        if(GUI.Button(new Rect(10,10,150,20), "Force"))
            ApplyImpulse(); 
        // Vector3[] data = GetObservations(); 
        // string recap = ""; 
        // foreach(Vector3 v in data){
        //     recap += v.ToString() + " \n"; 
        // }
        // GUI.TextArea(new Rect(10, 10, 200, 200), recap, 200);
    }




}
}