using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseMobile{


public class CloneMaker : MonoBehaviour {

    public GameObject Environment; 
    public int RootNumber; 
    public float Spacing; 

    public bool Vertical; 

    [ContextMenu("Make clones")]
    void MakeClones()
    {
        int counter = 0; 
        for(int i = 0; i<RootNumber; i++)
        {
            for(int j = 0; j<RootNumber; j++)
            {
                Vector3 env_pos = Vector3.zero; 
                if(Vertical)
                    env_pos = new Vector3(i*Spacing, j*Spacing,  0f); 
                else
                    env_pos = new Vector3(i*Spacing, 0f, j*Spacing); 
                GameObject env = Instantiate(Environment, env_pos, Environment.transform.rotation) as GameObject;
                env.name = "Environment " + counter.ToString(); 
                counter ++ ; 
            }
        }
    }


    void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        for(int i = 0; i<RootNumber; i++)
        {
            for(int j = 0; j<RootNumber; j++)
            {
                Vector3 env_pos = Vector3.zero; 
                if(Vertical)
                    env_pos = new Vector3(i*Spacing, j*Spacing,  0f); 
                else
                    env_pos = new Vector3(i*Spacing, 0f, j*Spacing); 
                Gizmos.DrawWireSphere(env_pos, 1f); 
            }
        }
        #endif
    }
}


}
