using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawCircle : MonoBehaviour
{

    public float theta_scale = 0.01f;        //Set lower to add more points
    public int size; //Total number of points in circle
    public float radius = 3f;
    LineRenderer lineRenderer;

    void Awake()
    {
        float sizeValue = (2.0f * Mathf.PI) / theta_scale;
        size = (int)sizeValue;
        size++;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetWidth(0.1f, 0.1f); //thickness of line
        lineRenderer.SetVertexCount(size);
    }

    void Update()
    {
        Vector3 pos;
        float theta = 0f;
        for (int i = 0; i < size; i++)
        {
            theta += (2.0f * Mathf.PI * theta_scale);
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            x += gameObject.transform.position.x;
            y += gameObject.transform.position.y;
            pos = new Vector3(x, y, 0);
            lineRenderer.SetPosition(i, pos);
        }
    }
}

