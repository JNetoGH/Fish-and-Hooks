using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _point1;
    [SerializeField] private Transform _point2;

    // Update is called once per frame
    void Update()
    {
        _lineRenderer.SetPosition(0, _point1.position);
        _lineRenderer.SetPosition(1, _point2.position);
    }
}
