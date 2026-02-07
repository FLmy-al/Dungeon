using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool isUsing;
    public int pos_x;
    public int pos_y;
    private GridController parent;

    private void Awake()
    {
        isUsing = false;
    }
}
