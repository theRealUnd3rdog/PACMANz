using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[InitializeOnLoad]
public class NodeGrid : MonoBehaviour
{
    private const string _gridName = "GridContainer";

    [HideInInspector]
    [SerializeField]
    [Range(1, 40)]
    private int _rowCount; // Number of rows in the grid

    [HideInInspector]
    [SerializeField]
    [Range(1, 40)]
    private int _columnCount; // Number of columns in the grid

    [HideInInspector]
    [SerializeField]
    [Range(0.25f, 6f)]
    private float _unitDistance; // Distance between nodes

    public float unitDistance
    {
        get { return _unitDistance; }
    }

    [HideInInspector]
    [SerializeField]
    private float _xOffset; // Offset on the x axis of the grid container

    [HideInInspector]
    [SerializeField]
    private float _zOffset; // Offset on the z axis of the grid container

    [HideInInspector]
    [SerializeField]
    [Range(-2f, 2f)]
    private float _yOffset; // Offset on the y axis of the grid container    

    [HideInInspector]
    [SerializeField]
    private float _gizmoRadius; // Gizmo radius

    private Transform _gridContainer; // Parent object to hold the nodes
    public List<Transform> gridNodes; // List of nodes
    public Transform startNode {get; set;} // the start node

    private int _previousRowCount;
    private int _previousColumnCount;
    private float _previousUnitDistance;
    private float _previousXOffset;
    private float _previousZOffset;
    private float _previousYOffset;
    private float _previousGizmoRadius;


#if UNITY_EDITOR
    static NodeGrid()
    {
        EditorApplication.update += CheckNodeGrid;
    }
#endif

    private void Awake()
    {
        foreach (Transform node in gridNodes)
        {
            if (node.CompareTag("StartNode"))
            {
                startNode = node;
                break;
            }
        }
    }

    private void Start()
    {
        // Store the initial parameter values
        _previousRowCount = _rowCount;
        _previousColumnCount = _columnCount;
        _previousUnitDistance = _unitDistance;
        _previousXOffset = _xOffset;
        _previousZOffset = _zOffset;
        _previousYOffset = _yOffset;
        _previousGizmoRadius = _gizmoRadius;
    }

    private bool HasParameterChanged()
    {
        return _rowCount != _previousRowCount ||
               _columnCount != _previousColumnCount ||
               _unitDistance != _previousUnitDistance ||
               _xOffset != _previousXOffset ||
               _zOffset != _previousZOffset ||
               _yOffset != _previousYOffset ||
               _gizmoRadius != _previousGizmoRadius;
    }

    public bool IsGridAlreadyGenerated()
    {
        if (this.transform.childCount == 0)
            return false;

        if (this.transform.GetChild(0).name == _gridName)
            return true;

        return false;
    }

#if UNITY_EDITOR
    private static void CheckNodeGrid()
    {
        NodeGrid nodeGrid = FindObjectOfType<NodeGrid>();

        if (nodeGrid != null && nodeGrid.HasParameterChanged())
        {
            Debug.Log("Change in values!");
            nodeGrid.GenerateGrid();

            // Update the previous parameter values
            nodeGrid._previousRowCount = nodeGrid._rowCount;
            nodeGrid._previousColumnCount = nodeGrid._columnCount;
            nodeGrid._previousUnitDistance = nodeGrid._unitDistance;
            nodeGrid._previousXOffset = nodeGrid._xOffset;
            nodeGrid._previousZOffset = nodeGrid._zOffset;
            nodeGrid._previousYOffset = nodeGrid._yOffset;
            nodeGrid._previousGizmoRadius = nodeGrid._gizmoRadius;
        }
    }
#endif

    /// <Summary>
    /// This generates a grid
    /// </Summary>
    public void GenerateGrid()
    {
        // Clear previous grid
        ClearGrid();

        // Create a new container for the grid
        if (!IsGridAlreadyGenerated())
            _gridContainer = new GameObject(_gridName).transform;
        else
            _gridContainer = this.transform.GetChild(0);

        _gridContainer.transform.SetParent(this.transform);
        _gridContainer.localPosition = new Vector3(_xOffset, _yOffset, _zOffset);

        Vector3 startPosition = _gridContainer.localPosition + new Vector3(-(_columnCount - 1) * _unitDistance * 0.5f, 0f, -(_rowCount - 1) * _unitDistance * 0.5f);

        for (int row = 0; row < _rowCount; row++)
        {
            for (int column = 0; column < _columnCount; column++)
            {
                Vector3 spawnPosition = startPosition + new Vector3(column * _unitDistance, 0f, row * _unitDistance);
                GameObject newNode = new GameObject("Node");
                newNode.AddComponent<Node>();
                newNode.transform.position = spawnPosition;
                newNode.transform.parent = _gridContainer;

                // Add nodes to the list
                gridNodes.Add(newNode.transform);
            }
        }
    }

    private void ClearGrid()
    {
        if (_gridContainer != null)
        {
            DestroyImmediate(_gridContainer.gameObject);
            gridNodes.Clear();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Node[] nodes = GetComponentsInChildren<Node>();

        foreach (Node node in nodes)
        {
            Gizmos.DrawWireSphere(node.transform.position, _gizmoRadius);
        }
    }

    public Transform GetNextClosestNode(Transform currentPlayerNode)
    {
        float shortestDistance = float.MaxValue;
        Transform nextNode = null;

        foreach (Transform node in gridNodes)
        {
            if (node != currentPlayerNode)
            {
                float distance = Vector3.Distance(currentPlayerNode.position, node.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nextNode = node;
                }
            }
        }

        return nextNode;
    }
}
