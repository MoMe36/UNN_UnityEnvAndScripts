using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class DifferentialWheels{

    public WheelCollider RightWheel; 
    public WheelCollider LeftWheel; 

    float current_left_torque; 
    float current_right_torque; 

    public void Apply(float left_thrust, float right_thrust, float MaxTorque)
    {
        RightWheel.motorTorque = right_thrust*MaxTorque; 
        LeftWheel.motorTorque = left_thrust*MaxTorque; 

        current_left_torque = left_thrust; 
        current_right_torque = right_thrust; 
    }

    public float [] GetMotorTorque()
    {
        return new float []{current_left_torque, current_right_torque}; 
    }

  

}

[System.Serializable]
public class CycleWheels{

    public WheelCollider [] FrontWheels; 
    public WheelCollider [] RearWheels; 

    float current_steering; 
    float current_torque; 

    public void Apply(float steer, float thrust, float MaxTorque, float MaxSteering)
    {

        current_torque = thrust; 
        current_steering = steer; 

        foreach(WheelCollider wheel in FrontWheels)
        {
            wheel.steerAngle = steer*MaxSteering; 
        }
        foreach(WheelCollider wheel in RearWheels)
        {
            wheel.motorTorque = thrust*MaxTorque;
        }
    }

    public float GetMotorTorque()
    {
        return current_torque; 
    }

    public float GetSteeringAngle()
    {
        return current_steering; 
    }

}

public class WheelController : MonoBehaviour {

    public enum RobotType {cycle, diff}; 
    public RobotType WheelArchi; 

    [Header("Torque & Steering")]
    public float MaxTorque; 
    public float MaxSteering = 35f; 

    [Header("DifferentialWheels")]
    public DifferentialWheels DiffWheels;

    [Header("Cycle Wheels")] 
    public CycleWheels WheelsCycle; 

    public Vector3 Inputs; 
    public bool UserControl; 

    // Use this for initialization
    void Start () {
        
    }

    void Update()
    {
        if(UserControl)
        {
            Inputs = new Vector3(Input.GetAxis("Horizontal"),
                                 Input.GetAxis("Vertical"), 
                                 Input.GetAxis("VerCam")); 
        }
    }

    public void SetInputsValues(float x, float y)
    {
        if(WheelArchi == RobotType.cycle)
        {
            Inputs = new Vector3(x,y, 0f); 
        }
        else {
            Inputs = new Vector3(0f, x,y); 
        }
    }

    public float [] GetDrivingInfos()
    {
        if(WheelArchi == RobotType.cycle)
        {
            return new float []{WheelsCycle.GetSteeringAngle(), WheelsCycle.GetMotorTorque()}; 
        }
        else
        {
            return DiffWheels.GetMotorTorque(); 
        }
    }
    
    // Update is called once per frame
    void FixedUpdate () {
        
        if(WheelArchi == RobotType.cycle)
        {
            WheelsCycle.Apply(Inputs.x, Inputs.y, MaxTorque, MaxSteering); 
        }
        else
        {
            DiffWheels.Apply(Inputs.y, Inputs.z, MaxTorque); 
            // DiffControl(y,y_c); 
        }


    }

}
