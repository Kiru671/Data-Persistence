using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour
{
    public UnityEvent<int> onDestroyed;
    
    private int pointValue;
    public int PointValue 
    { 
        get => pointValue;
        set
        {
            pointValue = value;
            UpdateColor();
        }
    }

    void Start()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer == null) return;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        switch (pointValue)
        {
            case 1:
                block.SetColor("_BaseColor", Color.green);
                break;
            case 2:
                block.SetColor("_BaseColor", Color.yellow);
                break;
            case 5:
                block.SetColor("_BaseColor", Color.blue);
                break;
            default:
                block.SetColor("_BaseColor", Color.red);
                break;
        }
        renderer.SetPropertyBlock(block);
    }

    private void OnCollisionEnter(Collision other)
    {
        onDestroyed.Invoke(pointValue);
        
        //slight delay to be sure the ball have time to bounce
        Destroy(gameObject, 0.1f);
    }
}