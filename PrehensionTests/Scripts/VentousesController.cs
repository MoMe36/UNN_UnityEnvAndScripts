using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ventouse{

    public Transform ventouse; 
    public GameObject ContactDetector; 

    public Ventouse (GameObject g)
    {
        ventouse = g.transform; 
    }

    public void Move(Vector3 direction){
        ventouse.position += direction*Time.deltaTime; 
    }

    public Transform Catch(){
        return ContactDetector.GetComponent<DetectVentouseContact>().Catch();  
    }


    public void Rotate(){

    }

    public Vector3 GetPosition(){
        return ventouse.position; 
    }


}

public class VentousesController : MonoBehaviour {

    public Ventouse V1; 
    public Ventouse V2; 

    public float SpeedV1; 
    public float SpeedV2;

    public Vector3 DirectionV1;
    public Vector3 DirectionV2; 

    Ventouse VirtualVentouse; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(VirtualVentouse == null){
    		V1.Move(DirectionV1*SpeedV1); 
            V2.Move(DirectionV2*SpeedV2); 
        }
        else{
            Vector3 sum_direction = (DirectionV1*SpeedV1 + DirectionV2*SpeedV2)*0.5f; 
            VirtualVentouse.Move(sum_direction);
            V1.Move(sum_direction);
            V2.Move(sum_direction);
        }

	}

    void MakeVentouse(){

        Debug.Log("Making ventouse"); 
        GameObject g = new GameObject(); 
        g.transform.position = (V1.GetPosition() + V2.GetPosition())*0.5f; 
        VirtualVentouse = new Ventouse(g);     
    }

    void PrepareObject(Transform caught){

        GameObject g = caught.gameObject;
        g.GetComponent<Rigidbody>().isKinematic = true; 
        caught.parent = VirtualVentouse.ventouse; 
    }

    void OnGUI(){
        if(GUI.Button(new Rect(10,10,50,15), "Catch")){
            Transform b1 = V1.Catch(); 
            Transform b2 = V2.Catch();

            Debug.Log(b1.ToString() + " " +b2.ToString());

            if(b1 == b2){
                // create virtual object and make target object the children of virtual ventouse
                MakeVentouse();
                PrepareObject(b1); 

            } 
        }
    }

}
