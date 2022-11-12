using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePince : MonoBehaviour {

    public float Speed = 0.5f; 
    public Vector3 Direction; 

    public enum MoveStrategy {rigidbody_mv, force, transform}; 
    public MoveStrategy move_strategy; 


    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {

        if(move_strategy == MoveStrategy.transform)
        {
            transform.position += Direction*Speed*Time.deltaTime; 
        }
        else if(move_strategy == MoveStrategy.rigidbody_mv){
            GetComponent<Rigidbody>().MovePosition(transform.position + Direction*Speed*Time.deltaTime); 
        }
        else
        {
            GetComponent<Rigidbody>().AddForce(transform.forward*0.01f); 
        }
        
    }
}
