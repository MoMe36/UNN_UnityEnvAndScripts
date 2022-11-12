using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reachers; 

public class DemoReachers : MonoBehaviour {


    public SceneManager Manager; 
    public float ThresholdDistance = 0.2f; 
    public int MinCounter = 10; 
    int counter; 

    void Update(){

        if(Manager.ComputeReward() > 0f){
            
            counter += 1; 
            if(counter > MinCounter){
                counter = 0; 
                Manager.Reset(); 
            }
        }
    }

    void OnGUI(){
        GUI.TextField(new Rect(10,10, 150, 25), "Distance: " + Manager.GetDistance().ToString()); 
    }


}


