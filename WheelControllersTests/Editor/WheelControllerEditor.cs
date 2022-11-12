using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 


public enum RobotOptions
{
    Cycle = 0,
    Diff = 1
}


[CustomEditor(typeof(WheelController))]
public class WheelControllerEditor : Editor{

    // RobotOptions rob_option = RobotOptions.Cycle;

    // public override void OnInspectorGUI()
    // {
    //     EditorGUIUtility.LookLikeInspector(); 
    //     WheelController Target = (WheelController)target; 

    //     var serializedObject = new SerializedObject(target); 
    //     // var prop = serializedObject.FindProperty("TestWheels"); 

    //     // EditorGUILayout.PropertyField(prop, true); 


    //     rob_option = (RobotOptions)EditorGUILayout.EnumPopup("Robot Type:", rob_option);

    //     if(rob_option == RobotOptions.Cycle)
    //     {
    //         var prop = serializedObject.FindProperty("WheelsCycle"); 

    //         Target.WheelsCycle.FrontWheels = EditorGUILayout.PropertyField(prop, false); 
    //         EditorGUILayout.FloatField("Cycle", 0f);
    //     }
    //     else
    //     {
    //         EditorGUILayout.FloatField("Diff", 4f); 
    //     }
    //     serializedObject.ApplyModifiedProperties(); 
    // }



}
