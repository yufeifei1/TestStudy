using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    int drawStartX = -6;
    int drawEndX = 6;
    int drawStartY = -6;
    int drawEndY = 6;
    float gap = 0.5f;

    int startI = 1;
    int startJ = 1;



    public GameObject startPerfab;
    public GameObject endPerfab;

    List<GridNode> openList = new List<GridNode>();
    List<GridNode> closeList = new List<GridNode>();

    bool isShowLine = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        InputAbout();
    }
    private void OnDrawGizmos()
    {
        float myX = drawStartX;
        float myY = drawStartY;
        while (myX <= drawEndX)
        {
            Debug.DrawLine(new Vector3(myX, drawStartY, 0), new Vector3(myX, drawEndY, 0), Color.red);
            myX = myX + gap;
        }
        while (myY <= drawEndY)
        {
            Debug.DrawLine(new Vector3(drawStartX, myY, 0), new Vector3(drawEndX, myY, 0), Color.red);
            myY = myY + gap;
        }

        if (isShowLine)
        {
            Vector3 starPos = Vector3.zero;
            Vector3 endPos = Vector3.zero;
            for (int i = 0; i < closeList.Count; i++)
            {
                if (i == 0)
                {
                    starPos = closeList[i].pos;
                }
                else
                {
                    endPos = closeList[i].pos;
                    Debug.DrawLine(starPos, endPos, Color.green);
                    starPos = endPos;
                }
                //Debug.Log(string.Format("i={0},x={1},y={2}",i, closeList[i].pos.x, closeList[i].pos.y));
            }
        }

    }

    private void InputAbout()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AStarCount();
        }
    }

    private void AStarCount()
    {
        isShowLine = false;
        openList.Clear();
        closeList.Clear();
        Vector3 startPos = startPerfab.transform.position;
        Vector3 endPos = endPerfab.transform.position;


        GridNode startNode = new GridNode(startI, startJ, startPerfab.transform.position, null, endPos);


        AddToOpenList(startNode);

        while (openList.Count > 0)
        {
            GridNode tempNode = openList[0];
            AddToCloseList(tempNode);
            RemoveFromOpenList(tempNode);
            if (tempNode.IsEndGrid)
            {
                Debug.Log("结束");
                tempNode.pos = endPos;
                isShowLine = true;
                break;
            }
            else
            {
                int x = tempNode.i;
                int y = tempNode.j;

                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        GridNode node = GetGridFromOpenList(i, j);
                        if (node == null)
                        {
                            node = new GridNode(i, j, new Vector3(startPos.x + i * gap, startPos.y + j * gap, 0), tempNode, endPos);
                            AddToOpenList(node);
                        }
                        else
                        {
                            node.SetParent(tempNode);
                        }

                    }
                }

                openList.Sort((a, b) => { return a.fn.CompareTo(b.fn); });
            }
        }

    }

    private void AddToOpenList(GridNode node)
    {
        if (openList.Contains(node) == false)
        {
            openList.Add(node);
        }


    }
    private GridNode GetGridFromOpenList(int i, int j)
    {
        int pos = -1;
        for (int k = 0; k < openList.Count; k++)
        {
            GridNode item = openList[k];
            if (item.i == i && item.j == j)
            {
                pos = k;
                break;
            }
        }
        if (pos == -1)
        {
            return null;
        }
        return openList[pos];


    }
    private void RemoveFromOpenList(GridNode node)
    {
        openList.Remove(node);
    }
    private void AddToCloseList(GridNode node)
    {
        if (closeList.Contains(node) == false)
        {
            closeList.Add(node);
        }
    }
}
public class GridNode
{
    public int i;
    public int j;
    public Vector3 pos;
    public GridNode parentNode;
    public float fn;//总代价
    public float gn; //到起点的代价   
    public float hn;//到终点的代价
    public bool IsEndGrid;//是否为终点格子
    public float xtemp;
    public float ytemp;
    public float gap = 0.5f;

    public GridNode(int i, int j, Vector3 pos, GridNode parentNode, Vector3 endPos)
    {
        this.i = i;
        this.j = j;
        this.parentNode = parentNode;
        this.pos = pos;
        IsEndGrid = (Mathf.Abs(pos.x - endPos.x) + Mathf.Abs(pos.y - endPos.y) <= gap) ? true : false;
        float dis = ((parentNode == null) || (parentNode != null && (i == parentNode.i || j == parentNode.j))) ? gap : Mathf.Sqrt(gap * gap + gap * gap);
        gn = (parentNode == null) ? dis : (parentNode.gn + dis);
        hn = Mathf.Abs(endPos.x - pos.x) + Mathf.Abs(endPos.y - pos.y);
        fn = gn + hn;

    }
    public void SetParent(GridNode parent)
    {
        if (parentNode == null)
        {
            parentNode = parent;
        }
        else if (parentNode != null && parentNode.gn > parent.gn)
        {
            parentNode = parent;
        }
    }

}



