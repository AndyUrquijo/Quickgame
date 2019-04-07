using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PathCreator))]
public class PathCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PathCreator obj = (PathCreator)target;
        if (GUILayout.Button("Make Path"))
        {
            obj.MakePath();
        }
    }
}
#endif


public class PathCreator : MonoBehaviour
{
    [PlayOnly] public float Speed = 1;
    [PlayOnly] public float Distance;


    [Header("Generation settings")]
    public float Step = 1;
    public int N = 10;
    public int subSteps = 4;
    public float downOffset = 2;

    [Header("Progression settings")]

    [Range(0,1), PlayOnly,ReadOnly]
    public float Progression;

    public AnimationCurve WidthCurve;
    public AnimationCurve WidthVarCurve;
    public AnimationCurve SpeedCurve;
    public AnimationCurve TwistCurve;
    public Gradient BackgroundGradient;

    [Header("HUD Refs")]
    public Text CounterText;



    Mesh mesh;
    MeshRenderer meshRend;
    MeshFilter meshFilter;

    Material material;

    EdgeCollider2D edgeA;
    EdgeCollider2D edgeB;

    Vector2[] positions;
    float[] widths;
    int positionIndex;

    Vector2[] edgeAPoints;
    Vector2[] edgeBPoints;
    Vector3[] vertices;
    int[] tris;

    public bool StopCounter;

    void Start()
    {
        MakePath();
    }

    public void MakePath()
    {
        meshRend = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        material = meshRend.material;

        var edges = GetComponents<EdgeCollider2D>();
        edgeA = edges[0];
        edgeB = edges[1];

        mesh = new Mesh();
        mesh.name = "ProcMesh";

        meshFilter.mesh = mesh;

        positions = new Vector2[N];
        widths = new float[N];
        positionIndex = 0;

        int M = (N-2)*subSteps;
        edgeAPoints = new Vector2[M];
        edgeBPoints = new Vector2[M];
        vertices = new Vector3[M*2];

        mesh.vertices = vertices;
        tris = new int[M*6];

        for (int i = 0; i < M-1; i++)
        {
            tris[6*i+0] = 0 + i*2;
            tris[6*i+1] = 2 + i*2;
            tris[6*i+2] = 1 + i*2;
            tris[6*i+3] = 1 + i*2;
            tris[6*i+4] = 2 + i*2;
            tris[6*i+5] = 3 + i*2;
        }
        mesh.triangles = tris;

        for (int i = 1; i < N; i++)
        {
            positions[i] = GetNewPos(positions[i-1]);
            widths[i] = GetNewWidth();
        }

        UpdateComponents();
    }

    Vector3 GetNewPos(Vector3 lastPos)
    {
        Vector3 mainDir = Vector3.up;
        Vector3 offset = Vector3.right;
        Vector3 newPos = lastPos;
        float twist = TwistCurve.Evaluate(Distance);

        newPos.y += Step;
        newPos.x +=Random.Range(-twist, twist);
        newPos.x = Mathf.Clamp(newPos.x, -1.0f, 1.0f);
        if (Distance >1000)
            newPos.x = 0;

        return newPos;
    }

    float GetNewWidth()
    {
        float width = WidthCurve.Evaluate(Distance);
        float widthVar = WidthVarCurve.Evaluate(Distance);
        return Random.Range(width - widthVar, width + widthVar);
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        float dX = Speed*dt;
        transform.position += Vector3.down*dX;

        if (!StopCounter)
            Distance+= dX;

        Progression = Distance/1000f;
        Speed = SpeedCurve.Evaluate(Distance);

        CounterText.text = (int)Distance + "m";
        Vector2 lowPos = positions[positionIndex];
        if (transform.position.y < -Step - downOffset)
        {
            for (int i = 0; i < N; i++)
                positions[i] += Vector2.down*Step;

            transform.position += Vector3.up*Step;

            int lastIndex = (positionIndex-1+N)%N;
            int newIndex = positionIndex;
            positions[newIndex] = GetNewPos(positions[lastIndex]);
            widths[newIndex] = GetNewWidth();

            positionIndex = (positionIndex+1)%N;
            UpdateComponents();
        }
        material.color = BackgroundGradient.Evaluate(Distance/1000f);
    }

    void UpdateComponents()
    {
        for (int i = 1; i < N-1; i++)
        {
            int index = (positionIndex+i)%N;

            Vector2 pos = positions[index];
            Vector2 prevPos = positions[(index+N-1)%N];
            Vector2 nextPos = positions[(index+1)%N];

            prevPos = 0.5f*(pos+prevPos);
            nextPos = 0.5f*(pos+nextPos);

            for (int j = 0; j < subSteps; j++)
            {
                float t = (float)j/subSteps;

                Vector2 currPos = BezierCurve.QuadraticBezier(prevPos, pos, nextPos, t);

                Vector2 lPos = currPos + Vector2.left*widths[index];
                Vector2 rPos = currPos + Vector2.right*widths[index];

                int m = (i-1)*subSteps + j;
                edgeAPoints[m] = lPos;
                edgeBPoints[m] = rPos;
                vertices[2*m] = lPos;
                vertices[2*m + 1] = rPos;
            }
        }

        edgeA.points = edgeAPoints;
        edgeB.points = edgeBPoints;

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = tris;
    }


    public void ResetScene()
    {
        SceneManager.LoadScene(gameObject.scene.buildIndex);
    }
}
