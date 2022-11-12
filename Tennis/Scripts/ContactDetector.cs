using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tennis; 

namespace Tennis{

    public class ContactDetector : MonoBehaviour{

        public SceneManager Manager; 
        public SceneManager_2 OtherManager; 

        void OnTriggerEnter(Collider other){

            NbPoints points = other.gameObject.GetComponent<NbPoints>(); 
            if(points != null){
                if(Manager != null)
                    Manager.ContactDetection(points);
                else
                    OtherManager.ContactDetection(points);  
            } 
        }


    }  


}