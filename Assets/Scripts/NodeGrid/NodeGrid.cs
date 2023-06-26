using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
public class NodeGrid : MonoBehaviour
{
    public static NodeGrid Instance;
    private const string _gridName = "GridContainer";

    [HideInInspector]
    [SerializeField]
    [Range(1, 40)]
    private int _rowCount; // Number of rows in the grid

    public int rowCount
    {
        get {return _rowCount;}
    }

    [HideInInspector]
    [SerializeField]
    [Range(1, 40)]
    private int _columnCount; // Number of columns in the grid

    public int columnCount
    {
        get {return _columnCount;}
    }

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
    public bool stopGenerating;
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
        Instance = this;

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

        if (nodeGrid != null && nodeGrid.HasParameterChanged() && !nodeGrid.stopGenerating)
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
                Node nodeComponent = newNode.AddComponent<Node>();

                nodeComponent.x = column; // Assign x-coordinate based on the column index
                nodeComponent.y = row; // Assign y-coordinate based on the row index
                nodeComponent.cost = int.MaxValue; // Set initial cost to maximum value (unreachable)
                nodeComponent.heuristic = 0; // Set initial heuristic to 0
                nodeComponent.parent = null; // Set initial parent to null

                newNode.transform.position = spawnPosition;
                newNode.transform.parent = _gridContainer;

                // Add nodes to the list
                gridNodes.Add(newNode.transform);
            }
        }

        // Connect neighbors
        ConnectNeighbors();
    }

    private void ConnectNeighbors()
    {
        for (int row = 0; row < _rowCount; row++)
        {
            for (int column = 0; column < _columnCount; column++)
            {
                Node currentNode = gridNodes[row * _columnCount + column].GetComponent<Node>();
                currentNode.neighbors = new Node[4];

                // Connect left neighbor
                if (column > 0)
                    currentNode.neighbors[0] = gridNodes[row * _columnCount + column - 1].GetComponent<Node>();

                // Connect right neighbor
                if (column < _columnCount - 1)
                    currentNode.neighbors[1] = gridNodes[row * _columnCount + column + 1].GetComponent<Node>();

                // Connect up neighbor
                if (row > 0)
                    currentNode.neighbors[2] = gridNodes[(row - 1) * _columnCount + column].GetComponent<Node>();

                // Connect down neighbor
                if (row < _rowCount - 1)
                    currentNode.neighbors[3] = gridNodes[(row + 1) * _columnCount + column].GetComponent<Node>();
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
