using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDrag : ItemDragConroller
{
    public override void InitOccupy()
    {
        x = new int[] { 0,0,1,1 };
        y = new int[] { 0,1,0,1 };
        occupy = 4;
    }
}
