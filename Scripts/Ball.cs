using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
    public Material baseMaterial;
    public Material hightlightMaterial;
    private MeshRenderer mr;
    public UnityEvent OnBallDropped;

    public void Drop()
    {
        OnBallDropped.Invoke();
    }
    private void Start()
    {
        mr = GetComponent < MeshRenderer>();
    }


    public void Highlight()
    {
        Material[] mats = mr.materials;
        mats[1] = hightlightMaterial;
        mr.materials = mats;
    }

    public void UnHighlight()
    {
        Material[] mats = mr.materials;
        mats[1] = baseMaterial;
        mr.materials = mats;
    }

}
