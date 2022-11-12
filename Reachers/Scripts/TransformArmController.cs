using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Reachers{



[System.Serializable]
public struct Segment{

    public string Name; 
    public Transform SegmentTransform;  
    [Range(-1f, 1f)] public float Angle;
    [Range(-1f, 1f)] public float TargetAngle; 
    public bool HasLimits;
    public Vector2 Limits;
    public Vector3 RotationAxis;   

    public void ChangeLimit(bool state){
        HasLimits = state; 
    }
}

[System.Serializable]
public class ReacherArm{

    public Segment [] Components; 
    public Vector2 SpeedLimits; 
    public float Speed; 
    public float MoveThreshold = 0.1f; 
    public Vector3 [] Offsets; 

    public void Init()
    {
        Offsets = new Vector3[Components.Length -1]; 
        for(int i = 1; i < Components.Length; i++)
        {
            Quaternion rot_init = Components[i].SegmentTransform.root.rotation;   
            Offsets[i-1] = Quaternion.Inverse(rot_init)*(Components[i].SegmentTransform.position - Components[i-1].SegmentTransform.position); 
            // if(i > 1)
            // {
            //     Offsets[i-1] = Components[i].SegmentTransform.position - Components[i-1].SegmentTransform.position; 
            // }
            // else
            // {
            //     Offsets[i-1] = Components[i].SegmentTransform.position - Components[i-1].SegmentTransform.position; 
            // }
        }
    }

    public void UpdateRotations()
    {   
        UpdateAngles(); 

        for(int i = 0; i < Components.Length; i++)
        {
            if(i > 0)
            {
                Components[i].SegmentTransform.position = Components[i-1].SegmentTransform.position 
                                                        + Components[i-1].SegmentTransform.rotation*Offsets[i-1];
                
                Components[i].SegmentTransform.rotation = Components[i-1].SegmentTransform.rotation*Quaternion.AngleAxis(Components[i].Angle*180f, Components[i].RotationAxis);  
            }
            else
            {
                Quaternion rot_init = Quaternion.identity; 
                if(Components[i].SegmentTransform.parent != null)
                {
                    rot_init = Components[i].SegmentTransform.parent.rotation; 
                }
                Components[i].SegmentTransform.rotation = rot_init*Quaternion.AngleAxis(Components[i].Angle*180f, Components[i].RotationAxis);  

            }
        }
    }

    public void UpdateAngles()
    {
        for(int i = 0; i< Components.Length; i++)
        {
            float clamped_angle = Components[i].Angle; 

            if(Components[i].HasLimits)
            {
                if(Mathf.Abs(clamped_angle - Components[i].TargetAngle) > MoveThreshold)
                    clamped_angle = Mathf.MoveTowards(clamped_angle, Components[i].TargetAngle, Speed*Time.deltaTime); 
                
                clamped_angle = Mathf.Clamp(clamped_angle, Components[i].Limits.x, Components[i].Limits.y); 
            }    
            else
            {
                float target_angle= Components[i].TargetAngle ;

                if(target_angle > 0.3f)
                {
                    clamped_angle = clamped_angle + Speed*Time.deltaTime;
                    if(clamped_angle > 1f)
                    {
                        clamped_angle = -1f; 
                    }
                }
                else if(target_angle < -0.3f)
                {
                    clamped_angle = clamped_angle - Speed*Time.deltaTime;
                    if(clamped_angle < -1f)
                    {
                        clamped_angle = 1f; 
                    }
                }

            }
            Components[i].Angle = clamped_angle; 
        }
    }

    public float [] GetAngles(){
        float[] angles = new float [Components.Length];
        for(int i = 0; i < angles.Length; i++)
        {
            angles[i] = Components[i].Angle;
        }
        return angles; 
    }

    public void SetAngles(float [] angles)
    {
        for(int i = 0; i< Components.Length; i++)
        {
            Components[i].Angle = angles[i]; 
        } 
    }

    public void SetTargetAngles(float [] angles)
    {
        for(int i = 0; i< Components.Length; i++)
        {           
           Components[i].TargetAngle = angles[i]; 
        }
    }

    public void Reset(bool randomize)
    {
        float[] new_angles = new float [Components.Length];
        if(randomize)
        {
            for(int i = 0; i < Components.Length; i++)
                new_angles[i] = Random.Range(-1f, 1f);        
        }
        else
        {
            for(int i = 0; i < Components.Length; i++)
                new_angles[i] =0f;             
        } 

        Speed = Random.Range(SpeedLimits.x, SpeedLimits.y); 
        SetAngles(new_angles); 
        SetTargetAngles(new_angles);
        UpdateRotations(); 
    }

    public int GetLength(){
        return Components.Length; 
    }

    public void ChangeSegmentLimit(int i, bool state, Vector2 range){
        if(state){
            Components[i].HasLimits = true; 
            Components[i].Limits = range;
        }
        else
        {
            Components[i].HasLimits = false; 
        }
    }
}

public class TransformArmController : MonoBehaviour {

    public bool Solo = false;
    public ReacherArm Arm;  
	// Use this for initialization
	void Start () {

        Arm.Init(); 
		// Changer systeme rotation de la base(autoriser tour complet) et ajouter bonus lorsque le transform de la base fait face à la cible 
	}
	
	// Update is called once per frame
	void Update () {
        if(Solo)
            UpdateArm(); 
	}


    public void SetAngles(float [] angles)
    {
        Arm.SetTargetAngles(angles); 
    }

    public float [] GetAngles()
    {
        return Arm.GetAngles();
    }

    public void UpdateArm()
    {
        Arm.UpdateRotations(); 
    }

    public void Reset(bool randomize)
    {
        Arm.Reset(randomize); 
    }
}
}