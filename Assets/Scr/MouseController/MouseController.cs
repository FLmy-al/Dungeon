using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public static MouseController Instance;

    public GridController onGrid;
    public Item onItem;
    public bool isOnDrag;
    public Item currentDragItem;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            isOnDrag = false;
        }

        onGrid = null;
        onItem = null;
    }
}
