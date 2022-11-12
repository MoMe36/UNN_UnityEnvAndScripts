using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControl : MonoBehaviour {

    public Rigidbody HandRB; 
    public FingerControl [] Fingers; 
    public float CloseDelay;
    public float [] FingersValue; 
    public float [] InitialDelayValues; 

    public bool OverrideTime; 
    public bool ApplyForce; 
    public float ForceMagnitude; 
    float[] delays;  

    // Use this for initialization
    void Start () {

        delays = new float [4]; 
        for(int i = 0; i< delays.Length; i++){
            delays[i] = InitialDelayValues[i];

        }


    }
    
    // Update is called once per frame
    void Update () {

        if(OverrideTime){
            for(int i = 0; i< delays.Length; i++)
            {
                Fingers[i].Target =  FingersValue[i]; 
            }

            if(ApplyForce){
                HandRB.AddForce(ForceMagnitude*Vector3.up); 
            }
        }
        else
        {

        for(int i = 0; i< delays.Length; i++)
        {
            delays[i] -= Time.deltaTime;
            if(delays[i] < 0f){
                Fingers[i].Target =  Fingers[i].Target == 0f ? 60f : 0f; 
                delays[i] = CloseDelay; 
            } 

        }
        }
    }  

    void OnGUI(){
        if(GUI.Button(new Rect(10,10,80,30), "Force"))   
            ApplyForce = !ApplyForce; 
    } 

    // [ContextMenu("Rig finger")]
    // void RigFingers(){

    //     string[] names = new string []{"j1", "j2", "j3"}; 
    //     Dictionary <string, HingeJoint> hinge_dict = new Dictionary<string, HingeJoint>(); 
    //     for(int i = 0; i < names.Length; i++ ){
    //         GameObject g = GameObject.Find(Finger.name + "/" + names[i]); 
    //         HingeJoint j = g.AddComponent<HingeJoint>(); 

    //         hinge_dict.Add(names[i], j);
    //         if(i > 0){
    //             hinge_dict[names[i]].connectedBody = hinge_dict[names[i-1]].gameObject.GetComponent<Rigidbody>(); 
    //         } 
    //     } 

    //     if(ConnectedTo != null){
    //         hinge_dict[names[0]].connectedBody = ConnectedTo; 

    //     }

    // }

    // void MakeHierarchy(){

    //     Transform[] children = Finger.GetComponentsInChildren<Transform>();
    //     Dictionary <string, HingeJoint> hinge_dict = new Dictionary<string, HingeJoint>(); 

    //     for(int i = 0; i <children.Length; i++){
    //         string name = children[i].gameObject.name; 
    //         if(name.StartsWith("Motor")){
    //             GameObject motor = children[i].gameObject; 
    //             HingeJoint joint = motor.AddComponent<HingeJoint>(); 
    //             string numbers = name.Remove(0,5); 

    //             hinge_dict.Add(numbers, joint); 
    //             GameObject cylinder = GameObject.Find(Finger.name + "/Finger" + numbers);
    //             cylinder.transform.parent = motor.transform; 
    //         }
    //     }

    //     string[] names_ = new string [3] {"0_2", "0_1", "0_0"}; 
    //     for(int i = 0; i< names_.Length - 1; i++){
    //         hinge_dict[names_[i]].connectedBody =hinge_dict[names_[i+1]].gameObject.GetComponent<Rigidbody>(); 
    //     }   


    // }
}
