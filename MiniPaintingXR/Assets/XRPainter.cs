using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class XRPainter : MonoBehaviour
{
    public InputActionProperty triggerAction;
    public GameObject strokePrefab;
    public GameObject TipColor;
    public Transform tip;
    public Color currentColor;

    public float minDistance = 0.01f;

    private LineRenderer currentLine;
    private List<Vector3> points = new List<Vector3>();
    private bool isPainting = false;


    public InputActionReference colorButton;

    private Color[] colors = new Color[]
    {
        Color.white,
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.black
    };

    private int colorIndex = 0;

    void Start() {
        currentColor = colors[colorIndex];

        Renderer tipRenderer = TipColor.GetComponent<Renderer>();
        if (tipRenderer != null)
        {
            tipRenderer.material.color = currentColor;
        }
    }

    void Update()
    {
        float trigger = triggerAction.action.ReadValue<float>();

        if (colorButton.action.WasPressedThisFrame())
        {
            colorIndex = (colorIndex + 1) % colors.Length;
            currentColor = colors[colorIndex];
            
            // Update TipColor object
            if (TipColor != null)
            {
                Renderer tipRenderer = TipColor.GetComponent<Renderer>();
                if (tipRenderer != null)
                {
                    tipRenderer.material.color = currentColor;
                }
            }
        }

        if (trigger > 0.1f)
        {
            if (!isPainting)
                StartStroke();

            AddPoint();
        }
        else
        {
            if (isPainting)
                EndStroke();
        }
    }

    void StartStroke()
    {
        isPainting = true;

        GameObject stroke = Instantiate(strokePrefab);
        currentLine = stroke.GetComponent<LineRenderer>();

        // FORCE unique material instance
        currentLine.material = new Material(currentLine.material);

        // Apply color
        currentLine.material.color = currentColor;

        // Optional (still useful)
        currentLine.startColor = currentColor;
        currentLine.endColor = currentColor;

        points.Clear();
    }
    

    void AddPoint()
    {
        Vector3 position = tip.position;

        if (points.Count == 0 || Vector3.Distance(points[^1], position) > minDistance)
        {
            points.Add(position);

            currentLine.positionCount = points.Count;
            currentLine.SetPositions(points.ToArray());
        }
    }

    void EndStroke()
    {
        isPainting = false;
        currentLine = null;
    }
}