using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents; 

namespace Morpho {



public class ChainControl : MonoBehaviour {

    [Header("Segments")]
    public Transform [] Hiereachy; 
    [Range(-1f, 1f)] public float [] Angles; 
    public float Magnitude = 1f; 

    [Header("Target")]
    public Transform Target; 

	// Use this for initialization
	void Start () {

        GetSegments(); 
		Angles = new float [Hiereachy.Length];
	}
	
	// Update is called once per frame
	void Update () {
		
        UpdatePositions(); 
	}

    public void GetSegments()
    {
        AgentSegment[] holder = GetComponentsInChildren<AgentSegment>(); 
        Hiereachy = new Transform [holder.Length]; 
        for(int i = 0; i < holder.Length; i++)
        {
            Hiereachy[i] = holder[i].gameObject.transform; 
            holder[i].SetTarget(Target); 
            holder[i].SetMessage(i*1f); 
            holder[i].SetControl(this);
            holder[i].SetNumber(i); 
        }
    }

    public void SetAngle(int number, float angle)
    {
        Angles[number] = angle; 
    }

    public void UpdatePositions()
    {
        int length = Hiereachy.Length; 
        for(int i = 0; i<length;i++)
        {
            if(i > 0)
            {
                Hiereachy[i].position = Hiereachy[i-1].position + 
                                        Quaternion.AngleAxis(Angles[i-1]*180f, Vector3.right)*Vector3.down*Magnitude; 
            }
            
            Hiereachy[i].rotation = Quaternion.AngleAxis(Angles[i]*180f, Vector3.right); 
        }
    }
}

}
