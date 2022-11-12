using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullReacher; 

namespace FullReacher{

public class ReacherConstraints : MonoBehaviour{

    public GameObject AvoidContactWith; 
    public float Penalty = 0f;  

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == AvoidContactWith){
            Penalty = -1f; 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == AvoidContactWith)
        {
            Penalty = 0f; 
        }
    }


}


}