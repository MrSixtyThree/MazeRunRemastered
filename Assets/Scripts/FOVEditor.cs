using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/*
[CustomEditor (typeof (EnemyUI))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyUI ui = (EnemyUI)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(ui.transform.position, Vector3.up, Vector3.forward, 360, ui.viewRadius);

        Vector3 viewAngleA = ui.DirectionFromAngle(-ui.viewAngle / 2, false);
        Vector3 viewAngleB = ui.DirectionFromAngle(ui.viewAngle / 2, false);

        Handles.DrawLine(ui.transform.position, ui.transform.position + viewAngleA * ui.viewRadius);
        Handles.DrawLine(ui.transform.position, ui.transform.position + viewAngleB * ui.viewRadius);
    }
}
*/
