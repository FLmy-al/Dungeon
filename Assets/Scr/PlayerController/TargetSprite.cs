using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSprite : MonoBehaviour
{
    public static TargetSprite Instance {  get; private set; }

    private void Start()
    {
        Instance = this;
    }
}
