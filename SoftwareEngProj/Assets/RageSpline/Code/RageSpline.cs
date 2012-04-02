using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RageSpline : MonoBehaviour
{
    public enum Outline { None = 0, Loop, Free };
    public Outline outline = Outline.Loop;
    public Color outlineColor1 = Color.black;

    public enum Fill { None = 0, Solid, Gradient };
    public Fill fill = Fill.Solid;
    public Color fillColor1 = Color.gray;
    public Color fillColor2 = Color.blue;

    [HideInInspector]
    public Vector2 gradientOffset;
    [HideInInspector]
    public float gradientAngle;
    //[HideInInspector]
    public float gradientScale = 10f;
    [HideInInspector]
    public Vector2 textureOffset;
    [HideInInspector]
    public float textureAngle;
    [HideInInspector]
    public float textureScale = 10f;

    public enum Emboss { None = 0, Sharp, Blurry };
    public Emboss emboss = Emboss.None;
    public Color embossColor1 = Color.white;
    public Color embossColor2 = Color.black; 
    [HideInInspector]
    public float embossAngle = 180f;
    public float embossOffset = 0.5f;
    [HideInInspector]
    public float embossSize = 10f;
    public float embossCurveSmoothness = 3f;
        
    public enum Physics { None = 0, Boxed };
    public Physics physics = Physics.None;
    public bool createPhysicsInEditor;
    public PhysicMaterial physicsMaterial;

    public int outlineVertexCount = 64;
    public int fillVertexCount = 64;
    public int embossVertexCount = 64;
    public int physicsColliderCount = 32;

    private int lastPhysicsVertsCount = 50;
    private BoxCollider[] boxColliders;
    
    public float AntiAliasingWidth = 0.5f;
    public float OutlineWidth = 1f;
             
    public bool showVertexGizmos=false;
    public bool showSplineGizmos=true;
    public bool showGradientGizmos=false;
    public bool showTexturingGizmos=false;
    public bool showEmbossGizmos=false;
    [HideInInspector]
    public RageCurve spline;

    private struct RageVertex
    {
        public Vector3 position;
        public Vector2 uv;
        public Color color;
        public Vector3 normal;
        public float splinePosition;
    }

    void Awake()
    {
        
        if (spline == null)
        {
            CreateRandomSpline();
        }

        DestroyImmediate(GetComponent(typeof(MeshFilter)));
        (GetComponent(typeof(RageSpline)) as RageSpline).RefreshMesh(true);
    }

	// Use this for initialization
	void Start () {

	}

    void FixedUpdate()
    {

    }
	
	// Update is called once per frame
	void OnDrawGizmosSelected() {
        if (showSplineGizmos)
        {
            spline.GizmoDraw(transform, Mathf.Clamp(64, 32, 2048), outline == Outline.Loop || outline == Outline.None);
        }

        if (showVertexGizmos)
        {
            MeshFilter mf = GetComponent(typeof(MeshFilter)) as MeshFilter;
            Mesh m = mf.sharedMesh;

            for (int i = 0; i < m.vertexCount; i++)
            {
                Vector3 pos = transform.TransformPoint(m.vertices[i]);
                Gizmos.DrawLine(pos - new Vector3(1f, 0f, 0f), pos + new Vector3(1f, 0f, 0f));
                Gizmos.DrawLine(pos - new Vector3(0f, 1f, 0f), pos + new Vector3(0f, 1f, 0f));
            }
        }

        if (showEmbossGizmos && emboss != RageSpline.Emboss.None)
        {
            Vector3 up = new Vector3(0f, 1f, 0f);
            Vector3 middle = spline.GetMiddle(10);
            Vector3 point = RotatePoint2D_CCW(up, embossAngle * Mathf.Deg2Rad) * (embossSize*4f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.TransformPoint(middle), transform.TransformPoint(middle + point));
            Gizmos.color = Color.blue * new Color(1f, 1f, 1f, 0.33f);
        }

        if (showGradientGizmos && fill == RageSpline.Fill.Gradient)
        {
            Vector3 up = new Vector3(0f, 1f, 0f);
            Vector3 middle = gradientOffset;
            Vector3 point = RotatePoint2D_CCW(up, gradientAngle * Mathf.Deg2Rad) * (1f / gradientScale);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.TransformPoint(middle), transform.TransformPoint(middle + point));
            Gizmos.color = Color.green * new Color(1f, 1f, 1f, 0.2f);

            Vector3 mid = transform.TransformPoint(middle);
            for (int i = 4; i <= 360; i += 4)
            {
                Gizmos.DrawLine(
                    mid + ScaleToGlobal(RotatePoint2D_CCW(up, (i - 4) * Mathf.Deg2Rad) * (1f / gradientScale)), 
                    mid + ScaleToGlobal(RotatePoint2D_CCW(up, i * Mathf.Deg2Rad) * (1f / gradientScale))
                    );
            }
        }

        if (showTexturingGizmos && fill != RageSpline.Fill.None)
        {
            Vector3 up = new Vector3(0f, 1f, 0f);
            Vector3 middle = textureOffset;
            Vector3 point = RotatePoint2D_CCW(up, textureAngle * Mathf.Deg2Rad) * (1f/textureScale);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.TransformPoint(middle), transform.TransformPoint(middle + point));
            Gizmos.color = Color.magenta * new Color(1f, 1f, 1f, 0.5f);

            Vector2 mid = middle;

            Gizmos.DrawLine(transform.TransformPoint(mid + RotatePoint2D_CCW(new Vector3(0.5f, 0.5f, 0f), textureAngle * Mathf.Deg2Rad) * (1f / textureScale)), transform.TransformPoint(mid + RotatePoint2D_CCW(new Vector3(-0.5f, 0.5f, 0f), textureAngle * Mathf.Deg2Rad) * (1f / textureScale)));
            Gizmos.DrawLine(transform.TransformPoint(mid + RotatePoint2D_CCW(new Vector3(-0.5f, 0.5f, 0f), textureAngle * Mathf.Deg2Rad) * (1f / textureScale)), transform.TransformPoint(mid + RotatePoint2D_CCW(new Vector3(-0.5f, -0.5f, 0f), textureAngle * Mathf.Deg2Rad) * (1f / textureScale)));
            Gizmos.DrawLine(transform.TransformPoint(mid + RotatePoint2D_CCW(new Vector3(-0.5f, -0.5f, 0f), textureAngle * Mathf.Deg2Rad) * (1f / textureScale)), transform.TransformPoint(mid + RotatePoint2D_CCW(new Vector3(0.5f, -0.5f, 0f), textureAngle * Mathf.Deg2Rad) * (1f / textureScale)));
            Gizmos.DrawLine(transform.TransformPoint(mid + RotatePoint2D_CCW(new Vector3(0.5f, -0.5f, 0f), textureAngle * Mathf.Deg2Rad) * (1f / textureScale)), transform.TransformPoint(mid + RotatePoint2D_CCW(new Vector3(0.5f, 0.5f, 0f), textureAngle * Mathf.Deg2Rad) * (1f / textureScale)));
        }
	}

    public void RefreshMesh(bool forcePhysics)
    {
        if (GetComponent(typeof(MeshFilter)) as MeshFilter == null)
        {
            gameObject.AddComponent(typeof(MeshFilter));
        }
        if (GetComponent(typeof(MeshRenderer)) as MeshRenderer == null)
        {
            MeshRenderer meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            Material mat = Resources.LoadAssetAtPath("Assets/RageSpline/Materials/RageSplineMaterial.mat", typeof(Material)) as Material;
            meshRenderer.sharedMaterial = mat;
        }
        spline.PrecalcNormals(outlineVertexCount);
        GenerateMesh();
        RefreshPhysics(forcePhysics);
    }

        
    private void GenerateMesh()
    {
        ForceZeroZ();

        RageVertex[] outlineVerts = GenerateOutlineVerts();
        RageVertex[] fillVerts = GenerateFillVerts();
        RageVertex[] embossVerts = GenerateEmbossVerts();
        
        int vertexCount = outlineVerts.Length + fillVerts.Length + embossVerts.Length;
        Vector3[] verts = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        Color[] colors = new Color[vertexCount];

        int v = 0;
        for (int i = 0; i < fillVerts.Length; i++)
        {
            verts[v] = fillVerts[i].position;
            uvs[v] = fillVerts[i].uv;
            colors[v] = fillVerts[i].color;
            v++;
        }

        for (int i = 0; i < embossVerts.Length; i++)
        {
            verts[v] = embossVerts[i].position;
            uvs[v] = embossVerts[i].uv;
            colors[v] = embossVerts[i].color;
            v++;
        }
        for (int i=0; i < outlineVerts.Length; i++)
        {
            verts[v] = outlineVerts[i].position;
            uvs[v] = outlineVerts[i].uv;
            colors[v] = outlineVerts[i].color;
            v++;
        }

        int[] triangles = GenerateTriangles(fillVerts, embossVerts, outlineVerts);
        MeshFilter mFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
        
        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        DestroyImmediate(mFilter.sharedMesh);
        mFilter.sharedMesh = mesh;
    }

    private RageVertex[] GenerateOutlineVerts()
    {
        RageVertex[] outlineVerts = new RageVertex[0];

        if (outline != Outline.None && outlineVertexCount > 2)
        {
            RageVertex[] splits = null;
            if (outline == Outline.Loop)
            {
                splits = GetSplits(outlineVertexCount, 0f, 1f);
            }
            else
            {
                splits = GetSplits(outlineVertexCount, 0f, (float)(spline.points.Length - 1) / (float)spline.points.Length + 1f / (float)outlineVertexCount);
            }
            int vertsInBand = splits.Length;
            outlineVerts = new RageVertex[splits.Length * 4];
            for (int v = 0; v < splits.Length; v++)
            {
                Vector3 normal = new Vector3();
                if (v == splits.Length - 1 && outline == Outline.Free)
                {
                    normal = spline.GetNormal(splits[v-1].splinePosition, new Vector3(0f, 0f, -1f), true);
                }
                else
                {
                    normal = spline.GetNormal(splits[v].splinePosition, new Vector3(0f, 0f, -1f), true);
                }

                Vector3 scaledNormal = ScaleToLocal(normal);
                float edgeWidth = spline.GetWidth(splits[v].splinePosition) * OutlineWidth;

                if (fill == Fill.None)
                {
                    outlineVerts[v + 0 * vertsInBand].position = splits[v].position + scaledNormal * AntiAliasingWidth + normal * edgeWidth * 0.5f;
                    outlineVerts[v + 1 * vertsInBand].position = splits[v].position + normal * edgeWidth * 0.5f;
                    outlineVerts[v + 2 * vertsInBand].position = splits[v].position - normal * edgeWidth * 0.5f;
                    outlineVerts[v + 3 * vertsInBand].position = splits[v].position - scaledNormal * AntiAliasingWidth - normal * edgeWidth * 0.5f;
                }
                else
                {
                    outlineVerts[v + 0 * vertsInBand].position = splits[v].position + scaledNormal * AntiAliasingWidth + normal * edgeWidth;
                    outlineVerts[v + 1 * vertsInBand].position = splits[v].position + normal * edgeWidth;
                    outlineVerts[v + 2 * vertsInBand].position = splits[v].position;
                    outlineVerts[v + 3 * vertsInBand].position = splits[v].position - scaledNormal * AntiAliasingWidth;
                }

                outlineVerts[v + 0 * vertsInBand].color = outlineColor1 * new Color(1f, 1f, 1f, 0f);
                outlineVerts[v + 1 * vertsInBand].color = outlineColor1 * new Color(1f, 1f, 1f, 1f);
                outlineVerts[v + 2 * vertsInBand].color = outlineColor1 * new Color(1f, 1f, 1f, 1f);
                outlineVerts[v + 3 * vertsInBand].color = outlineColor1 * new Color(1f, 1f, 1f, 0f);

                outlineVerts[v + 0 * vertsInBand].uv = new Vector2(0f, 0f);
                outlineVerts[v + 1 * vertsInBand].uv = new Vector2(0f, 0f);
                outlineVerts[v + 2 * vertsInBand].uv = new Vector2(0f, 0f);
                outlineVerts[v + 3 * vertsInBand].uv = new Vector2(0f, 0f);
            }
        }
        else
        {
            outlineVerts = new RageVertex[0];
        }



        return outlineVerts;
    }

    private RageVertex[] GenerateFillVerts()
    {
        RageVertex[] fillVerts = new RageVertex[0];

        if (fill != Fill.None && fillVertexCount > 2)
        {
            RageVertex[] splits = GetSplits(fillVertexCount, 0f, 1f);

            if (outline == Outline.None)
            {
                fillVerts = new RageVertex[splits.Length * 2];
            }
            else
            {
                fillVerts = new RageVertex[splits.Length];
            }

            for (int v = 0; v < splits.Length; v++)
            {
                Vector3 normal = spline.GetNormal(splits[v].splinePosition, new Vector3(0f, 0f, -1f), true);
                Vector3 scaledNormal = ScaleToLocal(normal);
                if (outline == Outline.None)
                {
                    fillVerts[v].position = splits[v].position;
                    fillVerts[v + splits.Length].position = splits[v].position + scaledNormal * AntiAliasingWidth;

                    fillVerts[v].color = GetFillColor(fillVerts[v].position);
                    fillVerts[v + splits.Length].color = GetFillColor(fillVerts[v + splits.Length].position) * new Color(1f, 1f, 1f, 0f);

                    fillVerts[v].uv = GetFillUV(fillVerts[v].position, normal, splits[v].splinePosition);
                    fillVerts[v + splits.Length].uv = GetFillUV(fillVerts[v + splits.Length].position, normal, splits[v].splinePosition);

                }
                else
                {
                    fillVerts[v].position = splits[v].position + scaledNormal * (spline.GetWidth(splits[v].splinePosition) * OutlineWidth * 0.2f);
                    fillVerts[v].color = GetFillColor(fillVerts[v].position);
                    fillVerts[v].uv = GetFillUV(fillVerts[v].position, normal, splits[v].splinePosition);
                }

            }
        }
        return fillVerts;
    }

    private RageVertex[] GenerateEmbossVerts()
    {
        RageVertex[] splits = GetSplits(embossVertexCount, 0f, 1f);
        RageVertex[] embossVerts = new RageVertex[0];

        if (emboss != Emboss.None && embossVertexCount > 2)
        {
            int vertsInBand = splits.Length;
            embossVerts = new RageVertex[splits.Length * 4];
            Vector3 sunVector = RotatePoint2D_CCW(new Vector3(0f, -1f, 0f), embossAngle / (180f / Mathf.PI));
            Vector3[] embossVectors = new Vector3[splits.Length];
            Vector3[] normals = new Vector3[splits.Length];
            float[] dots = new float[splits.Length];
            float[] mags = new float[splits.Length];

            for (int v = 0; v < splits.Length; v++)
            {
                float p = (float)v / (float)splits.Length;
                normals[v] = spline.GetAvgNormal(p, new Vector3(0f, 0f, 1f), 0.05f, 3)*-1f;
                dots[v] = Vector3.Dot(sunVector, normals[v]);
                mags[v] = Mathf.Clamp01(Mathf.Abs(dots[v]) - embossOffset);
                if (dots[v] > 0f)
                {
                    embossVectors[v] = (sunVector+normals[v] * 2f).normalized * embossSize * mags[v];
                }
                else
                {
                    embossVectors[v] = (sunVector-normals[v]*2f).normalized * embossSize * mags[v] * -1f;
                }
            }

            for (int v = 0; v < splits.Length; v++)
            {
                Vector3 embossVector = new Vector3();
                for (int i = -Mathf.FloorToInt(embossCurveSmoothness); i <= Mathf.FloorToInt(embossCurveSmoothness)+1; i++)
                {
                    if (i != 0)
                    {
                        embossVector += embossVectors[mod(v - i, splits.Length)] * (1f - (float)Mathf.Abs(i) / (embossCurveSmoothness+1));
                    }
                    else
                    {
                        embossVector += embossVectors[mod(v - i, splits.Length)];
                    }
                }
                embossVector *= 1f / (Mathf.FloorToInt(embossCurveSmoothness)*2+1);

                embossVerts[v + 0 * vertsInBand].position = splits[v].position - normals[v] * AntiAliasingWidth;
                embossVerts[v + 1 * vertsInBand].position = splits[v].position;
                embossVerts[v + 2 * vertsInBand].position = splits[v].position + embossVector;
                embossVerts[v + 3 * vertsInBand].position = splits[v].position + embossVector + normals[v] * AntiAliasingWidth;

                if (embossVector.sqrMagnitude > 0.01f)
                {
                    if (dots[v] < 0f)
                    {
                        if (emboss == Emboss.Sharp)
                        {
                            embossVerts[v + 0 * vertsInBand].color = embossColor2 * new Color(1f, 1f, 1f, 0f);
                            embossVerts[v + 1 * vertsInBand].color = embossColor2 * new Color(1f, 1f, 1f, Mathf.Clamp01(mags[v]*4f));
                            embossVerts[v + 2 * vertsInBand].color = embossColor2 * new Color(1f, 1f, 1f, Mathf.Clamp01(mags[v] * 4f));
                            embossVerts[v + 3 * vertsInBand].color = embossColor2 * new Color(1f, 1f, 1f, 0f);
                        }
                        else
                        {
                            embossVerts[v + 0 * vertsInBand].color = embossColor2 * new Color(1f, 1f, 1f, 0f);
                            embossVerts[v + 1 * vertsInBand].color = embossColor2 * new Color(1f, 1f, 1f, Mathf.Clamp01(mags[v] * 4f));
                            embossVerts[v + 2 * vertsInBand].color = embossColor2 * new Color(1f, 1f, 1f, 0f);
                            embossVerts[v + 3 * vertsInBand].color = embossColor2 * new Color(1f, 1f, 1f, 0f);
                        }
                    }
                    else
                    {
                        if (emboss == Emboss.Sharp)
                        {
                            embossVerts[v + 0 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, 0f);
                            embossVerts[v + 1 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, Mathf.Clamp01(mags[v] * 4f));
                            embossVerts[v + 2 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, Mathf.Clamp01(mags[v] * 4f));
                            embossVerts[v + 3 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, 0f);
                        }
                        else
                        {
                            embossVerts[v + 0 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, 0f);
                            embossVerts[v + 1 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, Mathf.Clamp01(mags[v] * 4f));
                            embossVerts[v + 2 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, 0f);
                            embossVerts[v + 3 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, 0f);
                        }
                    }
                }
                else
                {
                    embossVerts[v + 0 * vertsInBand].position = splits[v].position - normals[v] * AntiAliasingWidth;
                    embossVerts[v + 1 * vertsInBand].position = splits[v].position;
                    embossVerts[v + 2 * vertsInBand].position = splits[v].position;
                    embossVerts[v + 3 * vertsInBand].position = splits[v].position;

                    embossVerts[v + 0 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, 0f);
                    embossVerts[v + 1 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, 0f);
                    embossVerts[v + 2 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, 0f);
                    embossVerts[v + 3 * vertsInBand].color = embossColor1 * new Color(1f, 1f, 1f, 0f);
                }
                
                embossVerts[v + 0 * vertsInBand].uv = new Vector2(1f, 1f);
                embossVerts[v + 1 * vertsInBand].uv = new Vector2(1f, 1f);
                embossVerts[v + 2 * vertsInBand].uv = new Vector2(1f, 1f);
                embossVerts[v + 3 * vertsInBand].uv = new Vector2(1f, 1f);
            }
        }
        else
        {
            embossVerts = new RageVertex[0];
        }

        return embossVerts;
    }

    private RageVertex[] GetSplits(int vertCount, float start, float end)
    {
        RageVertex[] splitsFixed = new RageVertex[vertCount];

        for (int v=0; v<vertCount; v++)
        {
            splitsFixed[v].splinePosition = ((float)v / (float)(vertCount)) * (end-start) + start;
            if (outline == Outline.Free && v == vertCount-1)
            {
                splitsFixed[v].position = spline.GetPoint(splitsFixed[v].splinePosition-0.2f/(float)vertCount);
            }
            else
            {
                splitsFixed[v].position = spline.GetPoint(splitsFixed[v].splinePosition);
            }
            splitsFixed[v].color = new Color(1f, 1f, 1f, 1f);
            splitsFixed[v].uv = new Vector2(0f, 0f);
        }
        return splitsFixed;
    }

    private int[] GenerateTriangles(RageVertex[] fillVerts, RageVertex[] embossVerts, RageVertex[] outlineVerts)
    {
        int[] tris = null;
        
        Vector2[] verts = new Vector2[0];
                
        if (outline == Outline.None)
        {
            verts = new Vector2[fillVerts.Length/2];
        }
        else
        {
            verts = new Vector2[fillVerts.Length];
        }

        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = new Vector2(fillVerts[i].position.x, fillVerts[i].position.y);
        }
           
        Triangulator triangulator = new Triangulator(verts);
        int t = 0;
        int[] fillTris = triangulator.Triangulate();

        if (outline == Outline.None)
        {
            tris = new int[((embossVerts.Length / 4) * 6 + (outlineVerts.Length / 4) * 6 + (fillVerts.Length / 2) * 2) * 3 + fillTris.Length];
        }
        else
        {
            if (outline == Outline.Loop)
            {
                tris = new int[((outlineVerts.Length / 4) * 6 + (embossVerts.Length / 4) * 6 + (outlineVerts.Length / 4) * 6) * 3 + fillTris.Length];
            }
            else
            {
                tris = new int[(((outlineVerts.Length-1) / 4) * 6 + (embossVerts.Length / 4) * 6 + (outlineVerts.Length / 4) * 6) * 3 + fillTris.Length];
            }
        } 
        
        for (int i = 0; i < fillTris.Length; i++)
        {
            tris[t++] = fillTris[i];
        }

        int vertsPerBand = 0;

        if (outline == Outline.None)
        {
            vertsPerBand = verts.Length;

            for (int v = 0; v < vertsPerBand; v++)
            {
                for (int b = 0; b < 1; b++)
                {
                    if (v < vertsPerBand - 1)
                    {
                        tris[t++] = v + b * vertsPerBand;
                        tris[t++] = v + (b + 1) * vertsPerBand + 1;
                        tris[t++] = v + (b + 1) * vertsPerBand;
                        tris[t++] = v + b * vertsPerBand;
                        tris[t++] = v + b * vertsPerBand + 1;
                        tris[t++] = v + (b + 1) * vertsPerBand + 1;
                    }
                    else
                    {
                        tris[t++] = v + b * vertsPerBand;
                        tris[t++] = (b + 1) * vertsPerBand;
                        tris[t++] = v + (b + 1) * vertsPerBand;
                        tris[t++] = v + b * vertsPerBand;
                        tris[t++] = b * vertsPerBand;
                        tris[t++] = (b + 1) * vertsPerBand;
                    }
                }
            }
        }
            
        vertsPerBand = embossVerts.Length / 4;

        for (int v = 0; v < vertsPerBand; v++)
        {
            for (int b = 0; b < 3; b++)
            {
                if (v < vertsPerBand - 1)
                {
                    tris[t++] = v + b * vertsPerBand + fillVerts.Length;
                    tris[t++] = v + (b + 1) * vertsPerBand + 1 + fillVerts.Length;
                    tris[t++] = v + (b + 1) * vertsPerBand + fillVerts.Length;
                    tris[t++] = v + b * vertsPerBand + fillVerts.Length;
                    tris[t++] = v + b * vertsPerBand + 1 + fillVerts.Length;
                    tris[t++] = v + (b + 1) * vertsPerBand + 1 + fillVerts.Length;
                }
                else
                {
                    tris[t++] = v + b * vertsPerBand + fillVerts.Length;
                    tris[t++] = (b + 1) * vertsPerBand + fillVerts.Length;
                    tris[t++] = v + (b + 1) * vertsPerBand + fillVerts.Length;
                    tris[t++] = v + b * vertsPerBand + fillVerts.Length;
                    tris[t++] = b * vertsPerBand + fillVerts.Length;
                    tris[t++] = (b + 1) * vertsPerBand + fillVerts.Length;
                }
            }
        }

        vertsPerBand = outlineVerts.Length / 4;

        for (int v = 0; v < vertsPerBand; v++)
        {
            for (int b = 0; b < 3; b++)
            {
                if (v < vertsPerBand - 1)
                {
                    tris[t++] = v + b * vertsPerBand + embossVerts.Length + fillVerts.Length;
                    tris[t++] = v + (b + 1) * vertsPerBand + 1 + embossVerts.Length + fillVerts.Length;
                    tris[t++] = v + (b + 1) * vertsPerBand + embossVerts.Length + fillVerts.Length;
                    tris[t++] = v + b * vertsPerBand + embossVerts.Length + fillVerts.Length;
                    tris[t++] = v + b * vertsPerBand + 1 + embossVerts.Length + fillVerts.Length;
                    tris[t++] = v + (b + 1) * vertsPerBand + 1 + embossVerts.Length + fillVerts.Length;
                }
                else
                {
                    if (outline == Outline.Loop)
                    {
                        tris[t++] = v + b * vertsPerBand + embossVerts.Length + fillVerts.Length;
                        tris[t++] = (b + 1) * vertsPerBand + embossVerts.Length + fillVerts.Length;
                        tris[t++] = v + (b + 1) * vertsPerBand + embossVerts.Length + fillVerts.Length;
                        tris[t++] = v + b * vertsPerBand + embossVerts.Length + fillVerts.Length;
                        tris[t++] = b * vertsPerBand + embossVerts.Length + fillVerts.Length;
                        tris[t++] = (b + 1) * vertsPerBand + embossVerts.Length + fillVerts.Length;
                    }
                }
            }
        }

        return tris;
    }

    public Color GetFillColor(Vector3 position)
    {
        switch (fill)
        {
            case Fill.Solid:
                return fillColor1;
             case Fill.Gradient:
                Vector3 middle = gradientOffset;
                Vector2 rotated = RotatePoint2D_CCW((position - middle), -gradientAngle / (180f / Mathf.PI)) * gradientScale * 0.5f;
                float v = rotated.y + 0.5f;
                return Mathf.Clamp(v, 0f, 1f) * fillColor1 + (1f - Mathf.Clamp(v, 0f, 1f)) * fillColor2;
        }
        return fillColor1;
    }

    public Vector2 GetFillUV(Vector3 position, Vector3 normal, float splinePoint)
    {

        Vector3 middle = textureOffset;
        Vector2 rotated = RotatePoint2D_CCW(position - middle, -textureAngle / (180f / Mathf.PI)) * textureScale;
        rotated += new Vector2(0.5f, 0.5f);
        return rotated;
    }

    public Vector2 RotatePoint2D_CCW(Vector3 p, float angle)
    {
        return new Vector2(p.x * Mathf.Cos(-angle) - p.y * Mathf.Sin(-angle), p.y * Mathf.Cos(-angle) + p.x * Mathf.Sin(-angle));
    }

    public float Vector2Angle_CCW(Vector2 normal)
    {
        Vector3 up = new Vector3(0f, 1f, 0f);
        if (normal.x < 0f)
        {
            return Mathf.Acos(up.x * normal.x + up.y * normal.y) * Mathf.Rad2Deg * -1f + 360f;
        }
        else
        {
            return (Mathf.Acos(up.x * normal.x + up.y * normal.y) * Mathf.Rad2Deg * -1f + 360f) * -1f + 360f;
        }
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public void RefreshPhysics(bool forcePhysics)
    {
        if (createPhysicsInEditor || Application.isPlaying)
        {
            if (boxColliders == null)
            {
                boxColliders = new BoxCollider[1];
            }
            if (physics != Physics.None && physicsColliderCount > 2)
            {
                if (lastPhysicsVertsCount != physicsColliderCount || boxColliders[0] == null || forcePhysics)
                {
                    DestroyPhysicsChildren();

                    RageVertex[] splits = new RageVertex[0];
                    if (outline == Outline.Free && fill == Fill.None)
                    {
                        splits = GetSplits(physicsColliderCount + 1, 0f, (float)(spline.points.Length - 1) / (float)spline.points.Length);
                    }
                    else
                    {
                        splits = GetSplits(physicsColliderCount, 0f, 1f);
                    }

                    boxColliders = new BoxCollider[splits.Length];
                    int t = 0;
                    if (outline == Outline.Free && fill == Fill.None)
                    {
                        t = splits.Length - 1;
                    }
                    else
                    {
                        t = splits.Length;
                    }

                    float outlineW = 0f;
                    if (outline != Outline.None)
                    {
                        if (fill != Fill.None)
                        {
                            outlineW = OutlineWidth;
                        }
                        else
                        {
                            outlineW = OutlineWidth*0.5f;
                        }
                    }

                    for (int i = 0; i < t; i++)
                    {
                        
                        GameObject newObj = new GameObject();
                        newObj.name = "ZZZ_" + gameObject.name + "_BoxCollider";
                        newObj.transform.parent = transform;
                        BoxCollider box = newObj.AddComponent(typeof(BoxCollider)) as BoxCollider;
                        box.material = physicsMaterial;

                        int i2 = mod(i + 1, splits.Length);

                        Vector3 norm = spline.GetNormal(splits[i].splinePosition, new Vector3(0f, 0f, -1f), true);
                        Vector3 norm2 = spline.GetNormal(splits[i2].splinePosition, new Vector3(0f, 0f, -1f), true);
                        Vector3 pos = splits[i].position + norm * (outlineW * spline.GetWidth(splits[i].splinePosition) + 0.5f + AntiAliasingWidth * 0.5f);
                        Vector3 pos2 = splits[i2].position + norm2 * (outlineW * spline.GetWidth(splits[i2].splinePosition) + 0.5f + AntiAliasingWidth * 0.5f);
                        newObj.transform.localPosition = (pos + pos2) * 0.5f - 0.5f * norm;
                        newObj.transform.LookAt(transform.TransformPoint(newObj.transform.localPosition + Vector3.Cross((pos2 - pos).normalized, new Vector3(0f, 0f, -1f))), new Vector3(1f, 0f, 0f));
                        newObj.transform.localScale = new Vector3(100f, (pos2 - pos).magnitude, 1f);
                        boxColliders[i] = box;
                    }
                }
                else
                {
                    float outlineW = 0f;
                    if (outline != Outline.None)
                    {
                        outlineW = OutlineWidth;
                    }

                    int i = 0;
                    RageVertex[] splits = new RageVertex[0];
                    if (outline == Outline.Free && fill == Fill.None)
                    {
                        splits = GetSplits(physicsColliderCount, 0f, (float)(spline.points.Length - 1) / (float)spline.points.Length + 1f / (float)physicsColliderCount);
                    }
                    else
                    {
                        splits = GetSplits(physicsColliderCount, 0f, 1f);
                    }

                    foreach (BoxCollider obj in boxColliders)
                    {
                        //obj.material = physicsMaterial;
                        obj.material = physicsMaterial;
                        int i2 = mod(i + 1, physicsColliderCount);

                        Vector3 norm = spline.GetNormal(splits[i].splinePosition, new Vector3(0f, 0f, -1f), true);
                        Vector3 norm2 = spline.GetNormal(splits[i2].splinePosition, new Vector3(0f, 0f, -1f), true);
                        Vector3 pos = splits[i].position + norm * (outlineW * spline.GetWidth(splits[i].splinePosition) + 0.5f + AntiAliasingWidth * 0.5f);
                        Vector3 pos2 = splits[i2].position + norm2 * (outlineW * spline.GetWidth(splits[i2].splinePosition) + 0.5f + AntiAliasingWidth * 0.5f);
                        obj.gameObject.transform.localPosition = (pos + pos2) * 0.5f - 0.5f * norm;
                        obj.gameObject.transform.LookAt(transform.TransformPoint(obj.gameObject.transform.localPosition + Vector3.Cross((pos2 - pos).normalized, new Vector3(0f, 0f, -1f))), new Vector3(1f, 0f, 0f));
                        obj.gameObject.transform.localScale = new Vector3(100f, (pos2 - pos).magnitude, 1f);
                        i++;
                    }

                    lastPhysicsVertsCount = physicsColliderCount;
                }
            }
            else
            {
                DestroyPhysicsChildren();
            }
        }
        else
        {
            DestroyPhysicsChildren();
        }
    }

    public void ForceZeroZ()
    {
        spline.ForceZeroZ();
    }

    void DestroyPhysicsChildren()
    {
        int i = 0;
        int safe = transform.childCount+1;
        while (transform.childCount > 0 && i < transform.childCount && safe > 0)
        {
            safe--;
            if (transform.GetChild(i).GetComponent(typeof(BoxCollider)) != null)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            else
            {
                i++;
            }
        }
    }

    public Vector3 ScaleToGlobal(Vector3 vec)
    {
        return new Vector3(vec.x * (transform.lossyScale.x), vec.y * (transform.lossyScale.y), vec.z * (transform.lossyScale.z));
                
    }

    public Vector3 ScaleToLocal(Vector3 vec)
    {
        return new Vector3(vec.x * (1f / transform.lossyScale.x), vec.y * (1f / transform.lossyScale.y), vec.z * (1f / transform.lossyScale.z));

    }

    public int GetNearestRageSplinePoint(Vector3 pos, float threshold)
    {
        float nearestDist = (pos - spline.points[0].point).sqrMagnitude;
        int nearestIndex = 0;

        if (!Mathf.Approximately(pos.z, 0f))
        {
            pos = new Vector3(pos.x, pos.y, 0f);
        }

        for (int i = 1; i < spline.points.Length; i++)
        {
            if ((pos - spline.points[i].point).sqrMagnitude < nearestDist)
            {
                nearestDist = (pos - spline.points[i].point).sqrMagnitude;
                nearestIndex = i;
            }
        }
        if ((pos - spline.points[nearestIndex].point).magnitude < threshold)
        {
            return nearestIndex;
        }
        else
        {
            return -1;
        }
    }

    private void CreateRandomSpline()
    {
        Vector3[] pts = new Vector3[2];
        Vector3[] ctrl = new Vector3[2 * 2];
        float[] width = new float[2];
        bool[] natural = new bool[2];


        pts[0] = new Vector3(
            0f,
            -Camera.main.orthographicSize*0.4f,
            0f
            );

        width[0] = Random.Range(0.5f, 2f);
        ctrl[0] = new Vector3(Camera.main.orthographicSize * 0.3f, 0f, 0f);
        ctrl[1] = new Vector3(Camera.main.orthographicSize * -0.3f, 0f, 0f);
        natural[0] = true;

        pts[1] = new Vector3(
            0f,
            Camera.main.orthographicSize * 0.4f,
            0f
            );

        width[1] = Random.Range(0.5f, 2f);
        ctrl[2] = new Vector3(Camera.main.orthographicSize * -0.3f, 0f, 0f);
        ctrl[3] = new Vector3(Camera.main.orthographicSize * 0.3f, 0f, 0f);
        natural[1] = true;
        
        spline = new RageCurve(pts, ctrl, natural, width);
    }
    
    private class Triangulator
    {
        private List<Vector2> m_points = new List<Vector2>();

        public Triangulator(Vector2[] points)
        {
            m_points = new List<Vector2>(points);
        }

        public int[] Triangulate()
        {
            List<int> indices = new List<int>();

            int n = m_points.Count;
            if (n < 3)
                return indices.ToArray();

            int[] V = new int[n];

            if (Area() > 0)
            {
                for (int v = 0; v < n; v++)
                    V[v] = v;
            }
            else
            {
                for (int v = 0; v < n; v++)
                    V[v] = (n - 1) - v;
            }
                
            int nv = n;
            int count = 2 * nv;
            for (int m = 0, v = nv - 1; nv > 2; )
            {
                if ((count--) <= 0)
                    return indices.ToArray();

                int u = v;
                if (nv <= u)
                    u = 0;
                v = u + 1;
                if (nv <= v)
                    v = 0;
                int w = v + 1;
                if (nv <= w)
                    w = 0;

                if (Snip(u, v, w, nv, V))
                {
                    int a, b, c, s, t;
                    a = V[u];
                    b = V[v];
                    c = V[w];
                    
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(c);

                    m++;
                    for (s = v, t = v + 1; t < nv; s++, t++)
                        V[s] = V[t];
                    nv--;
                    count = 2 * nv;
                }
            }

            indices.Reverse();
            return indices.ToArray();
        }

        private float Area()
        {
            int n = m_points.Count;
            float A = 0.0f;
            for (int p = n - 1, q = 0; q < n; p = q++)
            {
                Vector2 pval = m_points[p];
                Vector2 qval = m_points[q];
                A += pval.x * qval.y - qval.x * pval.y;
            }
            return (A * 0.5f);
        }

        private bool Snip(int u, int v, int w, int n, int[] V)
        {
            int p;
            Vector2 A = m_points[V[u]];
            Vector2 B = m_points[V[v]];
            Vector2 C = m_points[V[w]];
            if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
                return false;
            for (p = 0; p < n; p++)
            {
                if ((p == u) || (p == v) || (p == w))
                    continue;
                Vector2 P = m_points[V[p]];
                if (InsideTriangle(A, B, C, P))
                    return false;
            }
            return true;
        }

        private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
        {
            float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            float cCROSSap, bCROSScp, aCROSSbp;

            ax = C.x - B.x; ay = C.y - B.y;
            bx = A.x - C.x; by = A.y - C.y;
            cx = B.x - A.x; cy = B.y - A.y;
            apx = P.x - A.x; apy = P.y - A.y;
            bpx = P.x - B.x; bpy = P.y - B.y;
            cpx = P.x - C.x; cpy = P.y - C.y;

            aCROSSbp = ax * bpy - ay * bpx;
            cCROSSap = cx * apy - cy * apx;
            bCROSScp = bx * cpy - by * cpx;

            return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
        }
    }
}

