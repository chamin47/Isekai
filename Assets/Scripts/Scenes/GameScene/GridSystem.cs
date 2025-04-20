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

/// <summary>
/// 1. ���� ���� �� ������ ���� ������Ʈ �浹 Ȯ��
/// 2. ������ ������ġ ��ȯ
/// </summary>
public class GridSystem : MonoBehaviour
{
    [SerializeField] private Vector2 _referenceSize = new Vector2(5, 5);       // �׸����ϳ��� ũ��
    [SerializeField] private Transform _referenceTransform;                    // �׸����� ������ �Ǵ� ������Ʈ
    [SerializeField] private int _maxGridCount = 9;                            // �׸��� �� ���� 
    [SerializeField] private List<GridInfo> _gridInfos = new List<GridInfo>(); // �׸��� ���� ����Ʈ
    private List<int> _randomIndexList = new List<int>();                      // ���� ��ġ ��ȯ ����Ʈ

    public void Init(Transform referenceTransform)
    {
        this._referenceTransform = referenceTransform;

        // �׸��� ���� �ʱ�ȭ
        _gridInfos = new List<GridInfo>(_maxGridCount);
        for (int i = 0; i < _maxGridCount; i++)
        {
            _gridInfos.Add(new GridInfo());
        }

        if (_gridInfos.Count == 0) return;

        //���� �ε��� �ʱ�ȭ
        _randomIndexList = Enumerable.Range(0, _gridInfos.Count).ToList();

        for (int i = 0; i < _gridInfos.Count; i++)
        {
            _gridInfos[i].Size = _referenceSize;
        }

        int mid = _gridInfos.Count / 2;

        // �׸��� ��ġ ����
        Vector2 mainPosition = _referenceTransform.position;
        mainPosition.y = 0;
        _gridInfos[mid].Position = mainPosition;

        // ��
        for (int i = mid - 1; i >= 0; i--)
        {
            _gridInfos[i].Position = _gridInfos[i + 1].Position - new Vector2(_gridInfos[i].Size.x, 0);
        }

        // ��
        for (int i = mid + 1; i < _gridInfos.Count; i++)
        {
            _gridInfos[i].Position = _gridInfos[i - 1].Position + new Vector2(_gridInfos[i].Size.x, 0);
        }
    }

    /// <summary>
    /// �� ������ ������ 0, 0 �� ��ȯ�Ѵ�
    /// </summary>
    public bool TryGetEmptyPosition(out Vector2 position)
    {
        int index = FindEmptyRandomIndex();
        if (index == -1)
        {
            position = Vector2.zero;
            return false;
        }

        position = GetGridWorldPosition(index);
        return true;
    }

    /// <summary>
    /// �÷��̾��� ��ġ�� �������� �浹���� �ʴ� ������ ������Ʈ ����
    /// ��� ������ ������ ���������� �ʴ´�.
    /// </summary>
    /// <returns></returns>
    // 
    private int FindEmptyRandomIndex()
    {
        _randomIndexList.Shuffle();

        for (int i = 0; i < _randomIndexList.Count; i++)
        {
            int index = _randomIndexList[i];

            // �÷��̾ �̵��ϱ� ������ �̸� ���� Ȯ���� �ʿ�
            // ���ɿ� ������ ����� ������ minigame�� ��ġ�� ������ �̸� ���� �Ǵ�
            if (Physics2D.OverlapBox(GetGridWorldPosition(index), _gridInfos[index].Size, 0, LayerMask.GetMask("UI")) == null)
            {
                return index;
            }
        }

        return -1;
    }

    // �׸����� ���弼�� ��ǥ ��ȯ
    private Vector2 GetGridWorldPosition(int index)
    {
        return _gridInfos[index].Position + new Vector2(_referenceTransform.position.x, 0);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {        
        foreach (var gridInfo in _gridInfos)
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
#endif
}
