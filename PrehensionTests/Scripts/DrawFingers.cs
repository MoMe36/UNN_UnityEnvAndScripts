using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawFingers : MonoBehaviour {

    public GameObject [] FingerJoints; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDrawGizmos(){

        Color[] colors = new Color[3]{Color.red, Color.yellow, Color.blue}; 

        if(FingerJoints.Length > 1){
            for(int i = 0; i < FingerJoints.Length -1; i ++){   
                Gizmos.color = colors[i]; 
                Gizmos.DrawLine(FingerJoints[i].transform.position, FingerJoints[i+1].transform.position); 
            }
        }
    }
}
