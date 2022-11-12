using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickRobotSetup : MonoBehaviour {

    public GameObject Robot; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	// void Update () {
		
	// }

    [ContextMenu("Setup")]
    void QuickSetup(){

        List <GameObject> created = new List<GameObject>(); 
        Transform[] all_parts = Robot.GetComponentsInChildren<Transform>(); 
        foreach(Transform part in all_parts){
            if(part.gameObject != Robot)
            {
                GameObject p = new GameObject(); 
                p.transform.position = part.position; 
                p.gameObject.name = part.gameObject.name; 
                part.transform.parent = p.transform; 

                created.Add(p); 
            }
        }
        
        // for(int i = 0 ; i < Robot.transform.childCount; i ++){

        //     GameObject p = new GameObject();
        //     Transform current_part = Robot.transform.GetChild(0);
        //     p.transform.position = current_part.position; 
        //     p.name = current_part.gameObject.name; 
        //     current_part.parent = p.transform; 
        //     created.Add(p); 
        // }

        for(int i = 0; i<created.Count; i++){
            created[i].transform.parent = Robot.transform; 
        }


    }
}
