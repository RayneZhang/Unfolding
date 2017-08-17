﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    public Material originalMaterial;
    public Material DashedLineMaterial;
    bool clicked = false;
    void OnMouseEnter()
    {
        transform.GetComponent<Renderer>().material.color = new Color(1f, 1f, 0f);
    }

    void OnMouseExit()
    {
        if(!clicked)
            transform.GetComponent<Renderer>().material = originalMaterial;
    }

    /// <summary>
    /// Get the position of current Cube.
    /// </summary>
    /// <returns>transform.position</returns>
    public Vector3 getPosition()
    {
        return transform.position;
    }

    public void destroyLine()
    {
        DestroyObject(gameObject);
    }

    public void ChangeToGray()
    {
        transform.GetComponent<Renderer>().material.color = Color.gray;
        clicked = true;
    }

    public void ChangeToDashedLine()
    {
        transform.GetComponent<Renderer>().material = DashedLineMaterial;
        clicked = true;
    }
}
