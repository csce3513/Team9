using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RageSpline))]
public class RageSplineEditor : Editor
{
    public int selectedCurveIndex = -1;
    public bool showGradientAngle;
    
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        EditorGUILayout.LabelField("","");
        EditorGUILayout.LabelField("DOUBLECLICK = Add new point", "");
        EditorGUILayout.LabelField("DEL = Delete point", "");
        EditorGUILayout.LabelField("N = Natural/sharp at selected point", "");
        EditorGUILayout.LabelField("K/L = Outline width at selected point", "");
        EditorGUILayout.LabelField("", "");

        if (GUILayout.Button("Refresh"))
        {
            RageSpline spMesh = target as RageSpline;
            spMesh.RefreshMesh(true);
        }
       
        if (Event.current.type == EventType.ExecuteCommand || Event.current.type == EventType.KeyUp || Event.current.type == EventType.used)
        {
            RageSpline spMesh = target as RageSpline;
            spMesh.RefreshMesh(false);
        }

        if (Event.current.type == EventType.ExecuteCommand)
        {
            Undo.CreateSnapshot();
            Undo.RegisterSnapshot();
        }

    }

    public void OnSceneGUI()
    {
        Undo.SetSnapshotTarget(target, "Modified object");

        RageSpline spMesh = target as RageSpline;

        if (spMesh.showGradientGizmos && spMesh.fill == RageSpline.Fill.Gradient)
        {
            Handles.color = Color.green;
            spMesh.gradientOffset =
                spMesh.transform.InverseTransformPoint(
                Handles.FreeMoveHandle(
                    spMesh.transform.TransformPoint(spMesh.gradientOffset),
                    Quaternion.identity,
                    HandleUtility.GetHandleSize(spMesh.transform.TransformPoint(spMesh.gradientOffset)) * 0.2f,
                    Vector3.zero,
                    Handles.CircleCap
                ));
            
            Handles.color = Color.green;
            Vector3 up = new Vector3(0f, 1f, 0f);
            Vector3 point = spMesh.RotatePoint2D_CCW(up, spMesh.gradientAngle * Mathf.Deg2Rad);
            Vector3 middle = spMesh.gradientOffset;

            Vector3 pos =
                spMesh.transform.InverseTransformPoint(
                    Handles.FreeMoveHandle(
                        spMesh.transform.TransformPoint(middle + point * (1f / spMesh.gradientScale)),
                        Quaternion.identity,
                        HandleUtility.GetHandleSize(spMesh.transform.TransformPoint(point)) * 0.1f,
                        Vector3.zero,
                        Handles.CircleCap
                    )
                );

            Vector2 pos2d = pos;
            spMesh.gradientScale = Mathf.Clamp(1f / (pos2d - spMesh.gradientOffset).magnitude, 0.0001f, 1f);
            
            Vector3 dir = (pos - middle).normalized;
            spMesh.gradientAngle = spMesh.Vector2Angle_CCW(dir);
        }

        if (spMesh.showTexturingGizmos && spMesh.fill != RageSpline.Fill.None)
        {

            Handles.color = Color.magenta;
            spMesh.textureOffset = 
                spMesh.transform.InverseTransformPoint(
                Handles.FreeMoveHandle(
                    spMesh.transform.TransformPoint(spMesh.textureOffset),
                    Quaternion.identity,
                    HandleUtility.GetHandleSize(spMesh.transform.TransformPoint(spMesh.textureOffset)) * 0.2f,
                    Vector3.zero,
                    Handles.CircleCap
                ));

            Vector3 up = new Vector3(0f, 1f, 0f);
            Vector3 point = spMesh.RotatePoint2D_CCW(up, spMesh.textureAngle * Mathf.Deg2Rad);
            Vector3 middle = spMesh.textureOffset;

            Vector3 pos =
                spMesh.transform.InverseTransformPoint(
                    Handles.FreeMoveHandle(
                        spMesh.transform.TransformPoint(middle + point * (1f / spMesh.textureScale)),
                        Quaternion.identity,
                        HandleUtility.GetHandleSize(spMesh.transform.TransformPoint(point)) * 0.1f,
                        Vector3.zero,
                        Handles.CircleCap
                    )
                );

            Vector2 pos2d = pos;
            spMesh.textureScale = 1f/(pos2d - spMesh.textureOffset).magnitude;
            Vector3 dir = (pos - middle).normalized;
            spMesh.textureAngle = spMesh.Vector2Angle_CCW(dir);
        }


        if (spMesh.showEmbossGizmos && spMesh.emboss != RageSpline.Emboss.None)
        {

            Handles.color = Color.blue;

            Vector3 up = new Vector3(0f, 1f, 0f);
            Vector3 point = spMesh.RotatePoint2D_CCW(up, spMesh.embossAngle * Mathf.Deg2Rad);
            Vector3 middle = spMesh.spline.GetMiddle(10);

            Vector3 pos =
                spMesh.transform.InverseTransformPoint(
                    Handles.FreeMoveHandle(
                        spMesh.transform.TransformPoint(middle + point * (spMesh.embossSize * 4f)),
                        Quaternion.identity,
                        HandleUtility.GetHandleSize(spMesh.transform.TransformPoint(point)) * 0.2f,
                        Vector3.zero,
                        Handles.ConeCap
                    )
                );

            Vector2 pos2d = pos - middle;
            spMesh.embossSize = pos2d.magnitude * 0.25f;
            Vector2 dir = pos2d.normalized;
            spMesh.embossAngle = spMesh.Vector2Angle_CCW(dir);
        }
        
        if (spMesh.showSplineGizmos)
        {
           
            if (Event.current.type == EventType.mouseDown)
            {
                selectedCurveIndex = -1;

                if (Event.current.control || Event.current.clickCount > 1)
                {
                    Vector3 localPosition = spMesh.transform.InverseTransformPoint(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).GetPoint((spMesh.transform.position-Camera.current.transform.position).magnitude));
                    localPosition.z = spMesh.transform.position.z;
                    float nearest = spMesh.spline.GetNearestSplinePoint(localPosition, 1000f);
                    spMesh.spline.AddPoint(nearest);
                    spMesh.RefreshMesh(false);
                    Event.current.Use();
                    GUI.changed = true;
                }
            }
            
            if (Event.current.type == EventType.mouseUp)
            {
                Vector3 localPosition = spMesh.transform.InverseTransformPoint(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).GetPoint((spMesh.transform.position - Camera.current.transform.position).magnitude));
                selectedCurveIndex = spMesh.GetNearestRageSplinePoint(localPosition, 1000f);
            }

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.N && selectedCurveIndex >= 0)
                {
                    if (spMesh.spline.points[selectedCurveIndex].natural)
                    {
                        spMesh.spline.points[selectedCurveIndex].natural = false;
                    }
                    else
                    {
                        spMesh.spline.points[selectedCurveIndex].natural = true;
                    }
                    GUI.changed = true;
                    EditorUtility.SetDirty(target);
                    Event.current.Use();
                }
                else if (Event.current.keyCode == KeyCode.L && selectedCurveIndex >= 0)
                {
                    spMesh.spline.points[selectedCurveIndex].widthMultiplier *= 1.15f;
                    GUI.changed = true;
                    EditorUtility.SetDirty(target);
                    Event.current.Use();
                }
                else if (Event.current.keyCode == KeyCode.K && selectedCurveIndex >= 0)
                {
                    spMesh.spline.points[selectedCurveIndex].widthMultiplier *= 0.85f;
                    GUI.changed = true;
                    EditorUtility.SetDirty(target);
                    Event.current.Use();
                }
                else if (Event.current.keyCode == KeyCode.Delete && selectedCurveIndex >= 0)
                {
                    spMesh.spline.DelPoint(selectedCurveIndex);
                    GUI.changed = true;
                    Event.current.Use();
                }
                
            }
                                
            Handles.color = Color.white;                      
                
            for (int s = 0; s < spMesh.spline.points.Length; s++)
            {
                Handles.DrawCapFunction capFunc;
                float capSizeMultiplier = 1f;
                if (spMesh.spline.points[s].natural)
                {
                    capFunc = Handles.SphereCap;
                    capSizeMultiplier = 2.5f;
                }
                else
                {
                    capFunc = Handles.RectangleCap;
                    capSizeMultiplier = 1f;
                }

                if (s > 0)
                {
                    spMesh.spline.points[s].point = spMesh.transform.InverseTransformPoint(
                        Handles.FreeMoveHandle(
                            spMesh.transform.TransformPoint(spMesh.spline.points[s].point),
                            Quaternion.identity,
                            HandleUtility.GetHandleSize(spMesh.transform.TransformPoint(spMesh.spline.points[s].point)) * 0.075f * capSizeMultiplier,
                            Vector3.zero,
                            capFunc
                            )
                        );
                }
                else
                {

                    spMesh.spline.points[s].point = spMesh.transform.InverseTransformPoint(
                        Handles.FreeMoveHandle(
                            spMesh.transform.TransformPoint(spMesh.spline.points[s].point),
                            Quaternion.identity,
                            HandleUtility.GetHandleSize(spMesh.transform.TransformPoint(spMesh.spline.points[s].point)) * 0.075f * capSizeMultiplier,
                            Vector3.zero,
                            capFunc
                            )
                        );
                }

                spMesh.spline.setCtrl(s, 0, spMesh.transform.InverseTransformPoint(
                        Handles.FreeMoveHandle(
                            spMesh.transform.TransformPoint(spMesh.spline.points[s].point + spMesh.spline.points[s].inCtrl),
                            Quaternion.identity,
                            HandleUtility.GetHandleSize(spMesh.transform.TransformPoint(spMesh.spline.points[s].point)) * 0.1f,
                            Vector3.zero,
                            Handles.SphereCap
                            )
                        ) - spMesh.spline.points[s].point
                    );

                spMesh.spline.setCtrl(s, 1, spMesh.transform.InverseTransformPoint(
                        Handles.FreeMoveHandle(
                            spMesh.transform.TransformPoint(spMesh.spline.points[s].point + spMesh.spline.points[s].outCtrl),
                            Quaternion.identity,
                            HandleUtility.GetHandleSize(spMesh.transform.TransformPoint(spMesh.spline.points[s].point)) * 0.1f,
                            Vector3.zero,
                            Handles.SphereCap
                            )
                        ) - spMesh.spline.points[s].point
                    );

            }
            
                
        }

        if (Event.current.type == EventType.mouseDown)
        {
            Undo.CreateSnapshot();
            Undo.RegisterSnapshot();
        }

        if (GUI.changed)
        {
            spMesh.RefreshMesh(false);
            EditorUtility.SetDirty(target);
        }

    }
}