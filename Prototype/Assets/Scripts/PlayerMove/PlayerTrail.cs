using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerTrail : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float trailDuration = 2.0f;
    private List<TrailPoint> trailPositions = new List<TrailPoint>();
    
    // Start is called before the first frame update
    private void Start()
    {
        SetupLineRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTrail();
    }
    private void SetupLineRenderer()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }
    private void UpdateTrail()
    {
        trailPositions.Add(new TrailPoint(transform.position, Time.time));
        trailPositions.RemoveAll(point => Time.time - point.time > trailDuration);

        lineRenderer.positionCount = trailPositions.Count;
        lineRenderer.SetPositions(trailPositions.Select(point => point.position).ToArray());
    }
}



public class TrailPoint
{
    public Vector3 position;
    public float time;

    public TrailPoint(Vector3 position, float time)
    {
        this.position = position;
        this.time = time;
    }
}

