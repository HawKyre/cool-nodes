using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    public AssetReference circle;
    public AssetReference edge;

    private Dictionary<Vector2Int, GameObject> circleMap;
    private Dictionary<Vector2Int, GameObject> activatedCircleMap;

    private float distance = 1.8f;

    private static int[] circularArrayY = { 1, 1, 1, 0, 0, 0, -1, -1, -1 };
    private static int[] circularArrayX = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
    private static int[] crossArrayY = { 1, 0, 0, -1 };
    private static int[] crossArrayX = { 0, -1, 1, 0 };

    NodeClicker nC;
    int clickedNodes;

    public Text visitedText, activeText;

    // Start is called before the first frame update
    void Start()
    {
        clickedNodes = 0;
        nC = new NodeClicker(this);
        circleMap = new Dictionary<Vector2Int, GameObject>();
        activatedCircleMap = new Dictionary<Vector2Int, GameObject>();

        Vector2Int vv = Vector2Int.zero;
        for (int i = 0; i < 9; i++)
        {
            Vector2Int v = new Vector2Int(circularArrayX[i], circularArrayY[i]);
            circleMap.Add(v, null);
            circle.InstantiateAsync(distance * new Vector3(v.x, v.y), Quaternion.identity).Completed
                += (AsyncOperationHandle<GameObject> a) => { EndGenerateNode(a, Vector2Int.zero, v); };
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            nC.ClickR();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            nC.ClickB();
        }

        visitedText.text = ("Visited nodes: " + clickedNodes);
        activeText.text = ("Active nodes: " + activatedCircleMap.Count);
    }

    private void EndGenerateNode(AsyncOperationHandle<GameObject> obj, Vector2Int from, Vector2Int to)
    {
        circleMap[to] = obj.Result;
        obj.Result.GetComponent<CircleNode>().MapPos = to;

        if (to == Vector2Int.zero) ActivateNode(obj.Result.GetComponent<CircleNode>());
    }

    private void ActivateNode(CircleNode circleNode)
    {
        circleNode.Activate();
        activatedCircleMap.Add(circleNode.MapPos, circleNode.gameObject);
    }

    public void OnClickNode(Vector2Int cV)
    {
        clickedNodes++;
        CircleNode cN = circleMap[cV].GetComponent<CircleNode>();

        activatedCircleMap.Remove(cN.MapPos);
        GenerateEdge(cV, cN.ConnectedTo);
        bool[] nextNodes = GetRandArray(cV);
        for (int i = 0; i < 9; i++)
        {
            Vector2Int v = new Vector2Int(cV.x + circularArrayX[i], cV.y + circularArrayY[i]);
            if (i % 2 == 1 && circleMap[v] != null && !circleMap[v].GetComponent<CircleNode>().IsActivated)
            {
                if (nextNodes[i / 2])
                {
                    circleMap[v].GetComponent<CircleNode>().ConnectedTo = cV;
                    ActivateNode(circleMap[v].GetComponent<CircleNode>());
                    UnlockNodes(v);
                }
            }
        }


    }

    private bool[] GetRandArray(Vector2Int cV)
    {
        bool[] r = new bool[4];
        bool[] available = new bool[4] {true, true, true, true};
        int availableCount = 0;

        for (int i = 0; i < 4; i++)
        {
            Vector2Int v = cV + new Vector2Int(crossArrayX[i], crossArrayY[i]);
            available[i] = circleMap[v] != null && !circleMap[v].GetComponent<CircleNode>().IsActivated;
            if (available[i]) availableCount++;
        }

        if (availableCount < 3) return available;
        int rand = UnityEngine.Random.Range(0, 4);
        if (cV == Vector2Int.zero)
        {
            rand = 2;
        }

        for (int i = 0; i < rand; i++)
        {
            int ix = UnityEngine.Random.Range(0, 4);
            while (!available[ix] || r[ix])
            {
                ix = UnityEngine.Random.Range(0, 4);
            }
            r[ix] = true;
        }
        return r;
    }

    public void UnlockNodes(Vector2Int cV)
    {
        for (int i = 0; i < 9; i++)
        {
            Vector2Int v = new Vector2Int(cV.x + circularArrayX[i], cV.y + circularArrayY[i]);
            if (!circleMap.ContainsKey(v))
            {
                circleMap.Add(v, null);
                circle.InstantiateAsync(distance * new Vector3(v.x, v.y), Quaternion.identity).Completed
                += (AsyncOperationHandle<GameObject> a) => { EndGenerateNode(a, cV, v); };
            }
        }
    }

    private void GenerateEdge(Vector2Int src, Vector2Int dest)
    {
        if (src == dest) return;

        float radius = 0.36f;
        float scaleWide = distance - 2 * radius;
        const float scaleThin = 0.05f;
        float sX = 0, sY = 0;
        float tX = 0, tY = 0;

        var dV = (dest - src);
        if (dV.x < 0 && dV.y == 0)
        {
            // New node is to the left
            sX = -scaleWide;
            sY = scaleThin;
            tX = src.x * distance - distance / 2;
            tY = src.y * distance;
        } else if (dV.x > 0 && dV.y == 0)
        {
            // New node is to the right
            sX = scaleWide;
            sY = scaleThin;
            tX = src.x * distance + distance / 2;
            tY = src.y * distance;
        }
        else if (dV.y < 0 && dV.x == 0)
        {
            // New node is down
            sX = scaleThin;
            sY = -scaleWide;
            tX = src.x * distance;
            tY = src.y * distance - distance / 2;
        }
        else if (dV.y > 0 && dV.x == 0)
        {
            // New node is up
            sX = scaleThin;
            sY = scaleWide;
            tX = src.x * distance;
            tY = src.y * distance + distance / 2;
        }
        else
        {
            Debug.Log(src + " -- " + dest);
        }

        edge.InstantiateAsync().Completed += (AsyncOperationHandle<GameObject> a) => { EndGenerateEdge(a, sX, sY, tX, tY); };
    }

    private void EndGenerateEdge(AsyncOperationHandle<GameObject> obj, float scaleX, float scaleY, float transformX, float transformY)
    {
        GameObject g = obj.Result;
        g.transform.localScale = new Vector3(scaleX, scaleY);
        g.transform.position = new Vector3(transformX, transformY);
    }

    public void ClickRandomNode()
    {
        int r = UnityEngine.Random.Range(0, activatedCircleMap.Count);
        CircleNode cN = activatedCircleMap.ElementAt(r).Value.GetComponent<CircleNode>();
        cN.Click();
        // Removal is handled in here
        OnClickNode(cN.MapPos);
    }

    public void ClickRandomNodeBatch()
    {
        int rep = activatedCircleMap.Count;
        while (rep-- > 0)
        {
            int r = UnityEngine.Random.Range(0, activatedCircleMap.Count);
            CircleNode cN = activatedCircleMap.ElementAt(r).Value.GetComponent<CircleNode>();
            cN.Click();
            OnClickNode(cN.MapPos);
        }
    }
}