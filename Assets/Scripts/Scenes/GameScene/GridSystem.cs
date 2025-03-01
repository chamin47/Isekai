using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[Serializable]
public class GridInfo
{
    public Vector2 Position;
    public float sizeX;
    public float sizeY;

    public Vector2 Size
    {
        get
        {
            return new Vector2(sizeX, sizeY);
        }
        set
        {
            sizeX = value.x;
            sizeY = value.y;
        }
    }
}

public class GridSystem : MonoBehaviour
{
    public Vector2 referenceSize = new Vector2(5, 5);
    public Transform referenceTransform;
    public int maxGridCount = 9;
    public List<GridInfo> gridInfos = new List<GridInfo>();
    public List<int> randomIndexList = new List<int>();

    [ContextMenu("Test")]
    public void Test()
    {
        Init(referenceTransform);
        Debug.Log("Test");
    }

    public void Init(Transform referenceTransform)
    {
        this.referenceTransform = referenceTransform;

        Vector2 mainPosition = referenceTransform.position;
        
        // y��ǥ �ʱ�ȭ
        mainPosition.y = 0;

        // �׸��� ���� �ʱ�ȭ
        gridInfos = new List<GridInfo>(maxGridCount);
        for(int i = 0; i < maxGridCount; i++)
        {
            gridInfos.Add(new GridInfo());
        }

        if (gridInfos.Count == 0) return;

        //���� �ε��� �ʱ�ȭ
        randomIndexList = Enumerable.Range(0, gridInfos.Count).ToList();

        for (int i = 0; i < gridInfos.Count; i++)
        {
            gridInfos[i].Size = referenceSize;
        }


        int mid = gridInfos.Count / 2;


        gridInfos[mid].Position = mainPosition;

        for (int i = mid - 1; i >= 0; i--)
        {
            gridInfos[i].Position = gridInfos[i + 1].Position - new Vector2(gridInfos[i].Size.x, 0);
        }

        for (int i = mid + 1; i < gridInfos.Count; i++)
        {
            gridInfos[i].Position = gridInfos[i - 1].Position + new Vector2(gridInfos[i].Size.x, 0);
        }
    }

    /// <summary>
    /// �� ������ ������ 0, 0 ��ǥ�� �����Ѵ�
    /// </summary>
    /// <returns></returns>
    public bool TryGetEmptyPosition(out Vector2 position)
    {
        int index = FindEmptyRandomIndex();
        if (index == -1)
        {
            Debug.Log("��� ������ á���ϴ�.");
            position = Vector2.zero;
            return false;
        }

        position = GetRealGridPosition(index);
        return true;
    }

    // �÷��̾��� ��ġ�� �������� �浹���� �ʴ� ������ ������Ʈ ����
    private int FindEmptyRandomIndex()
    {
        randomIndexList.Shuffle();

        for (int i = 0; i < randomIndexList.Count; i++)
        {
            int index = randomIndexList[i];

            if (Physics2D.OverlapBox(GetRealGridPosition(index), gridInfos[index].Size, 0, LayerMask.GetMask("UI")) == null)
            {
                return index;
            }
        }

        // ��� ������ ������ ���
        // ����Ʈ�� �������� �ø��� ������ Ȯ���Ѵ�.
        // ���� �ڵ忡���� Init()���� ũ�⸦ �ø��� �ִ�.

        return -1;
    }

    private Vector2 GetRealGridPosition(int index)
    {        
        return gridInfos[index].Position + new Vector2(referenceTransform.position.x, 0);
    }

    private void OnDrawGizmos()
    {
        foreach (var gridInfo in gridInfos)
        {
            DrawGrid(gridInfo);
        }

        DrawCamera();
    }

    private void DrawGrid(GridInfo gridInfo)
    {
        Vector3 position = new Vector3(gridInfo.Position.x, gridInfo.Position.y, 0); 
        Vector3 size = new Vector3(gridInfo.Size.x, gridInfo.Size.y, 0.1f); 

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(position, size);
    }

    private void DrawCamera()
    {
        Gizmos.color = Color.red;
        float height = Camera.main.orthographicSize * 2f;
        float width = height * Camera.main.aspect;
        Gizmos.DrawWireCube(Camera.main.transform.position, new Vector3(width - 0.1f, height - 0.1f, 0.1f));
    }
}