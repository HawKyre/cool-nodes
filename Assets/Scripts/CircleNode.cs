using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleNode : MonoBehaviour
{
    private bool isActivated = false;
    private GameObject child;
    private SpriteRenderer sr;
    MapGenerator mapGenerator;

    private Vector2Int mapPos;
    private bool isClicked;
    private Vector2Int connectedTo;

    public Vector2Int MapPos { get => mapPos; set => mapPos = value; }
    public bool IsClicked { get => isClicked; private set => isClicked = value; }
    public bool IsActivated { get => isActivated; private set => isActivated = value; }
    public Vector2Int ConnectedTo { get => connectedTo; set => connectedTo = value; }

    void Awake()
    {
        child = transform.GetChild(0).gameObject;
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1, 1, 1, 0.2f);
        this.transform.parent = GameObject.FindGameObjectWithTag("CircleContainer").transform;
        mapGenerator = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<MapGenerator>();
    }

    public bool Click()
    {
        if (!IsClicked && IsActivated)
        {
            IsClicked = true;
            child.SetActive(true);
        }
        else return false;
        return true;
    }

    public bool Activate()
    {
        if (!IsActivated)
        {
            IsActivated = true;
            sr.color = Color.white;
        }
        else return false;
        return true;
    }

    private void OnMouseDown()
    {
        if (Click())
        {
            mapGenerator.OnClickNode(mapPos);
        }
    }
}
