using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRotate : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.Rotate(0, 0.1f, 0);
    }
}
