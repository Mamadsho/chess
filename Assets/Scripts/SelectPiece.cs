using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Execution order after BoardMove?
public class SelectPiece : MonoBehaviour
{
    public Material selectMaterial;
    public Material deselectMaterial;
    public Renderer [] selectables;
    public int selectedIndex = 0;
    private Vector3[] selectablesInClipSpace;
    private float minDistance = 999;
    private Vector3 mousePos;
    private int n_selectables;
    private float d;


    void Start()
    {
        n_selectables = selectables.Length;
        selectablesInClipSpace = new Vector3[n_selectables];
        OnEnable();
    }

    void OnEnable()
    {
        for (int i = 0; i < n_selectables; i++)
        {
            Vector3 center = selectables[i].bounds.center;
            selectablesInClipSpace[i] = Camera.main.WorldToScreenPoint(center);
            selectablesInClipSpace[i].Scale(new Vector3(1, 1, 0));
        }
    }

    void Update()
    {
        mousePos = Input.mousePosition;
        for(int i = 0; i < n_selectables; i++)
        {
            d = Vector3.Distance(mousePos, selectablesInClipSpace[i]);
            if (minDistance > d)
            {
                minDistance = d;
                selectedIndex = i;
            }
        }
        minDistance = 999;

        for (int i = 0; i < n_selectables; i++)
        {
            selectables[i].material = i == selectedIndex ? selectMaterial : deselectMaterial;
        }
    }
}
