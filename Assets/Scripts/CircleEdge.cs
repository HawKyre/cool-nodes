using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleEdge : MonoBehaviour
{
    CircleNode n1, n2;

    void Awake()
    {
        this.transform.parent = GameObject.FindGameObjectWithTag("EdgeContainer").transform;
    }
}
