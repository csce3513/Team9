using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RageCurve
{
    private Vector3[] precalcNormals;
    public RageSplinePoint[] points;

    public RageCurve Clone()
    {
        Vector3[] pts = new Vector3[points.Length];
        Vector3[] ctrl = new Vector3[points.Length * 2];
        float[] width = new float[points.Length];
        bool[] natural = new bool[points.Length];

        for (int i = 0; i < pts.Length; i++)
        {
            pts[i] = points[i].point;
            width[i] = points[i].widthMultiplier;
            ctrl[i] = points[i].inCtrl;
            ctrl[i + 1] = points[i].outCtrl;
            natural[i] = points[i].natural;
        }
        return new RageCurve(pts, ctrl, natural, width);
    }

    public RageCurve(Vector3[] pts, Vector3[] ctrl, bool[] natural, float[] width)
    {
        points = new RageSplinePoint[pts.Length];

        for (int i = 0; i < pts.Length; i++)
        {
            points[i] = new RageSplinePoint(pts[i], ctrl[i * 2], ctrl[i * 2 + 1], width[i], natural[i]);
        }
    }

    public float GetWidth(float t)
    {
        if (points.Length > 0)
        {
            int i = GetFloorIndex(t);

            float c = t * (float)points.Length - (float)i;

            if (i < points.Length - 1)
            {
                return points[i].widthMultiplier * (1f - c) + points[i + 1].widthMultiplier * c;
            }
            else
            {
                return points[i].widthMultiplier * (1f - c) + points[0].widthMultiplier * c;
            }
        }
        else
        {
            return 0f;
        }
    }
       
    public Vector3 GetNormal(float t, Vector3 up, bool usePreCalcs)
    {
        if (usePreCalcs && precalcNormals != null)
        {
            if (t > 1f || t < 0f)
            {
                t = mod(t, 0.9999f);
            }
            return precalcNormals[Mathf.Clamp(Mathf.FloorToInt(t * precalcNormals.Length), 0, precalcNormals.Length-1)];
        } 
        else 
        {
            if (points.Length > 0)
            {
                float t1 = t - 0.01f;
                int i1 = GetFloorIndex(t1);
                int i2 = GetCeilIndex(t1);
                float f1 = (t1 * points.Length) - i1;
                f1 = mod(f1, 1f);
                RageSplinePoint p1 = points[i1];
                RageSplinePoint p2 = points[i2];

                float t2 = t + 0.01f;
                int i3 = GetFloorIndex(t2);
                int i4 = GetCeilIndex(t2);
                float f2 = (t2 * points.Length) - i1;
                f2 = mod(f2, 1f);
                RageSplinePoint p3 = points[i3];
                RageSplinePoint p4 = points[i4];

                Vector3 tangent1 = (-3f * p1.point + 9f * (p1.point + p1.outCtrl) - 9f * (p2.point + p2.inCtrl) + 3f * p2.point) * f1 * f1
                    + (6f * p1.point - 12f * (p1.point + p1.outCtrl) + 6f * (p2.point + p2.inCtrl)) * f1
                    - 3f * p1.point + 3f * (p1.point + p1.outCtrl);

                Vector3 tangent2 = (-3f * p3.point + 9f * (p3.point + p3.outCtrl) - 9f * (p4.point + p4.inCtrl) + 3f * p4.point) * f2 * f2
                    + (6f * p3.point - 12f * (p3.point + p3.outCtrl) + 6f * (p4.point + p4.inCtrl)) * f2
                    - 3f * p3.point + 3f * (p3.point + p3.outCtrl);

                return Vector3.Cross((tangent1.normalized + tangent2.normalized) * 0.5f, up).normalized;

            }
            else
            {
                return new Vector3(1f, 0f, 0f);
            }
        }
    }

    public Vector3 GetAvgNormal(float t, Vector3 up, float dist, int samples)
    {
        Vector3 normal = new Vector3();
        float maxP = 999999f;
        float minP = -999999f;
        int ceil = GetCeilIndex(t);
        int flr = GetFloorIndex(t);

        if (!points[ceil].natural)
        {
            if (ceil > 0)
            {
                maxP = (float)ceil / points.Length - 0.01f;
            }
            else
            {
                maxP = points.Length - 0.01f;
            }
        }

        if (!points[flr].natural)
        {
            if (flr < points.Length-1)
            {
                minP = (float)flr / points.Length + 0.01f;
            }
            else
            {
                minP = 0.01f;
            }
        }

        for (float p = t - dist / 2f; p < t + dist / 2f + dist * 0.5f / (float)samples; p += dist / (float)samples)
        {
            if (p > minP && p < maxP)
            {
                normal += GetNormal(p, up, true);    
            }
        }
        return normal.normalized;
    }

    public void setCtrl(int index, int ctrlIndex, Vector3 value)
    {
        if (points[index].natural)
        {
            if (ctrlIndex == 0)
            {
                points[index].inCtrl = value;
                points[index].outCtrl = value * -1f;
            }
            else
            {
                points[index].inCtrl = value * -1f;
                points[index].outCtrl = value;
            }
        }
        else
        {
            if (ctrlIndex == 0)
            {
                points[index].inCtrl = value;
            }
            else
            {
                points[index].outCtrl = value;
            }
        }
    }

    public int GetFloorIndex(float t)
    {
        int i = 0;

        if (t < 0f || t > 1f)
        {
            t = mod(t, 1f);
        }

        i = Mathf.FloorToInt(t * points.Length) ;

        if (i >= points.Length || i < 0)
        {
            i = mod(i, points.Length);
        }

        return i;
    }

    public int GetCeilIndex(float t)
    {
        int i = 0;

        if (t < 0f || t > 1f)
        {
            t = mod(t, 1f);
        }

        i = Mathf.FloorToInt(t * points.Length) + 1;

        if (i >= points.Length || i < 0)
        {
            i = mod(i, points.Length);
        }

        return i;
    }

    public Vector3 GetPoint(float t)
    {
        int i = GetFloorIndex(t);
        int i2 = GetCeilIndex(t);
        
        float f = (t * points.Length) - i;

        if (f > 1f || f < 0f)
        {
            f = mod(f, 1f);
        }

        RageSplinePoint p1 = points[i];
        RageSplinePoint p2 = points[i2];

        float d = 1f - f;
        return d * d * d * p1.point + 3f * d * d * f * (p1.point + p1.outCtrl) + 3f * d * f * f * (p2.point + p2.inCtrl) + f * f * f * p2.point;
        
    }

    public void GizmoDraw(Transform transform, int verts, bool connected)
    {
        Vector3 p = GetPoint(0f);
        float maxi = 1f;
        if (!connected)
        {
            maxi = 1f - 1f / (float)(points.Length);
        }
        //Vector3 p2 = GetPointWithOffset(0f, 1f);

        for (float t = 1f / (float)(verts); t < maxi || Mathf.Approximately(t, maxi); t += maxi / (float)(verts))
        {
            Vector3 tmp = GetPoint(t);
            //Vector3 tmp2 = GetPointWithOffset(t, 1f);
            Gizmos.DrawLine(transform.TransformPoint(p), transform.TransformPoint(tmp));
            //Gizmos.DrawLine(transform.TransformPoint(p2), transform.TransformPoint(tmp2));

            //Gizmos.DrawLine(transform.TransformPoint(tmp), transform.TransformPoint(tmp + GetNormal(t, new Vector3(0f, 0f, -1f))));
            p = tmp; 
           // p2 = tmp2;
        }

        for (int i = 0; i < points.Length; i++)
        {
            points[i].GizmoDraw(transform);
        }


    }

    public Vector3 GetMiddle(int accuracy)
    {
        Vector3 middle = new Vector2();
        for (int i = 0; i < accuracy; i++)
        {
            middle += GetPoint((float)i / (float)accuracy);
        }
        middle = middle * (1f / (float)accuracy);
        return middle;
    }

    public Vector3 GetMin(int accuracy)
    {
        Vector3 min = new Vector3(99999999f, 99999999f, 99999999f);
        for (int i = 0; i < accuracy; i++)
        {
            Vector3 p = GetPoint((float)i / (float)accuracy);
            if (p.x < min.x)
            {
                min.x = p.x;
            }
            if (p.y < min.y)
            {
                min.y = p.y;
            }
            if (p.z < min.z)
            {
                min.z = p.z;
            }
        }
        return min;
    }

    public Vector3 GetMax(int accuracy)
    {
        Vector3 max = new Vector3(-99999999f, -99999999f, -99999999f);
        for (int i = 0; i < accuracy; i++)
        {
            Vector3 p = GetPoint((float)i / (float)accuracy);
            if (p.x > max.x)
            {
                max.x = p.x;
            }
            if (p.y > max.y)
            {
                max.y = p.y;
            }
            if (p.z > max.z)
            {
                max.z = p.z;
            }
        }
        return max;
    }

    public void AddPoint(float location)
    {
        RageSplinePoint[] tmpPoints = new RageSplinePoint[points.Length + 1];
        int newIndex = GetCeilIndex(location);
        Vector3 tangent = (GetPoint(location+0.001f)-GetPoint(location-0.001f)).normalized;
        float mag = (points[mod(newIndex, points.Length)].point - points[mod(newIndex + 1, points.Length)].point).magnitude * 0.25f;
        tmpPoints[newIndex] = new RageSplinePoint(GetPoint(location), mag * tangent * -1f, mag * tangent, GetWidth(location), true);
        for (int i = 0; i < tmpPoints.Length; i++)
        {
            if (i < newIndex)
            {
                tmpPoints[i] = points[i];
            }
            if (i > newIndex)
            {
                tmpPoints[i] = points[i-1];
            }
        }
        points = tmpPoints;
    }

    public void DelPoint(int index)
    {
        if (points.Length > 2)
        {
            RageSplinePoint[] tmpPoints = new RageSplinePoint[points.Length - 1];
            for (int i = 0; i < tmpPoints.Length; i++)
            {
                if (i < index)
                {
                    tmpPoints[i] = points[i];
                }
                if (i >= index)
                {
                    tmpPoints[i] = points[i + 1];
                }
            }
            points = tmpPoints;
        }
    }

    public float GetNearestSplinePoint(Vector3 position, float accuracy)
    {
        float nearestSqrDist = 99999999999f;
        float nearestPoint = 0f;
        for (int i = 0; i < accuracy; i++)
        {
            Vector3 p = GetPoint((float)i / (float)accuracy);
            if ((position - p).sqrMagnitude < nearestSqrDist)
            {
                nearestPoint = (float)i / (float)accuracy;
                nearestSqrDist = (position - p).sqrMagnitude;
            }
        }
        return nearestPoint;
    }

    public void PrecalcNormals(int points)
    {
        precalcNormals = new Vector3[points];
        Vector3 up = new Vector3(0f, 0f, -1f);
        for (int i = 0; i < points; i++)
        {
            precalcNormals[i] = GetNormal((float)i / (float)points, up, false);
        }
    }

    public void ForceZeroZ()
    {
        foreach (RageSplinePoint point in points)
        {
            if(!Mathf.Approximately(point.point.z, 0f)) {
                point.point = new Vector3(point.point.x, point.point.y, 0f);
            }
            if (!Mathf.Approximately(point.inCtrl.z, 0f))
            {
                point.inCtrl = new Vector3(point.inCtrl.x, point.inCtrl.y, 0f);
            }
            if (!Mathf.Approximately(point.outCtrl.z, 0f))
            {
                point.outCtrl = new Vector3(point.outCtrl.x, point.outCtrl.y, 0f);
            }
        }
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    private float mod(float x, float m)
    {
        return (x % m + m) % m;
    }
}

[System.Serializable]
public class RageSplinePoint
{
    public Vector3 point, inCtrl, outCtrl;
    public float widthMultiplier = 1f;
    public bool natural;

    public RageSplinePoint Clone()
    {
        return new RageSplinePoint(point, inCtrl, outCtrl, widthMultiplier, natural);
    }

    public RageSplinePoint(Vector3 point, Vector3 inCtrl, Vector3 outCtrl, float width, bool natural)
    {
        this.point = point;
        this.inCtrl = inCtrl;
        this.outCtrl = outCtrl;
        this.widthMultiplier = width;
        this.natural = natural;
    }

    public void GizmoDraw(Transform transform)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.TransformPoint(point), transform.TransformPoint(point + inCtrl));
        Gizmos.DrawLine(transform.TransformPoint(point + outCtrl), transform.TransformPoint(point));
    }
    
}


