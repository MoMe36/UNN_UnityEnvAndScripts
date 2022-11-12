using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectVentouseContact: MonoBehaviour{

    public Transform TriggerTransform ;

    void Start(){

    }

    void OnTriggerEnter(Collider other)
    {
        TriggerTransform = other.gameObject.transform; 
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.transform == TriggerTransform)  
            TriggerTransform = null; 
    }

    public Transform Catch(){
        return TriggerTransform; 
    }

    public bool HasContact(){
        return TriggerTransform != null; 
    }

}