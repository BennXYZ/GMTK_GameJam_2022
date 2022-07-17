using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RollableDice))]
public class RollableDiceInterface : UnityEditor.Editor
{
    RollableDice rollableDice;

    private void OnEnable()
    {
        rollableDice = target as RollableDice;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Save Rotation"))
        {
            rollableDice.values.Add(new Quaternion(rollableDice.transform.rotation.x,
                rollableDice.transform.rotation.y,
                rollableDice.transform.rotation.z,
                rollableDice.transform.rotation.w));
        }
    }
}
