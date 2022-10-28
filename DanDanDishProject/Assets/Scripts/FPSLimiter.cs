using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FPSLimiter : MonoBehaviour
{ 
    void Start()
    {
        Application.targetFrameRate = 25;
    }
}
