using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseMobile{


public class SceneManager : MonoBehaviour {


    public Transform Agent; 
    public Transform Target; 
    public Vector2 AvailableArea;

    public float DistanceThreshold; 
    public Transform MinHeight; 

    [SerializeField] float Distance; 


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // Distance = Vector3.Distance(Agent.position, Target.position); 
		
	}

    [ContextMenu("Test")]
    void ResetDebug()
    {
        Reset(); 
    }

    public float ComputeReward()
    {
        float d = Vector3.Distance(Agent.position, Target.position);
        float reward = d < DistanceThreshold ? 0.1f : 0f; 
        return reward; 
    }

    public bool Done()
    {
        return Agent.position.y < MinHeight.position.y; 
    }

    public void Reset()
    {
        Vector3 random_offset = new Vector3(Random.Range(-AvailableArea.x/2f, AvailableArea.x/2f),
                                            0f,
                                            Random.Range(-AvailableArea.y/2f, AvailableArea.y/2f));

        Target.transform.position = transform.position + random_offset;  

        Agent.position = transform.position; 
        Agent.gameObject.GetComponent<Rigidbody>().velocity *= 0f; 
        Agent.gameObject.GetComponent<Rigidbody>().angularVelocity *= 0f; 
        Agent.rotation = Quaternion.identity; 
    }

    void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        Gizmos.DrawWireCube(transform.position, new Vector3(AvailableArea.x, 1f, AvailableArea.y)); 
        #endif
    }
}


}
