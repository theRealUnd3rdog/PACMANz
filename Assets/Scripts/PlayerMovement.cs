using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum State
    {
        Moving,
        Stopped,
    }

    public State MovementState;

    // components
    private NodeGrid _nodeGrid;
    private Rigidbody _rigidbody;

    [Header("Movement Variables")]
    [SerializeField] private float _movementSpeed;
    private Vector3 _currentMoveDirection = Vector3.forward; // Current direction the player would move in
    private Vector3 _previousMoveDirection = Vector3.forward;

    [Header("Visual")]
    [SerializeField] private GameObject _visualObj;
    
    // inputs
    private float _horizontalInput = 0f;
    private float _verticalInput = 0f;

    // wall detection
    [Header("Wall detection")]
    [SerializeField] private LayerMask _wallMask;

    [Tooltip("This adds an added ray distance on top of the unit distance for minor errors")]
    [Range(0f, 2f)]
    [SerializeField] private float _addedRayDistance;

    // privates
    private Transform _currentNode; // Current node the player is on
    private Coroutine _movementCor; // Coroutine to run the whole movement
    private Coroutine _nodeCor; // Coroutine to run the node movement

    private void Awake()
    {
        _nodeGrid = FindObjectOfType<NodeGrid>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        // Start from start node position
        if (_nodeGrid.startNode != null)
        {
            _rigidbody.MovePosition(_nodeGrid.startNode.position);
            _currentNode = _nodeGrid.startNode;
        }  
        else
            Debug.LogWarning("You have not set a start node position");

        _movementCor = StartCoroutine(RunMovement());         
    }

    // Update is called once per frame
    private void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        UpdateMoveDirection(_horizontalInput, _verticalInput);
        RespondToOppositeDirection();
    }

    private void RespondToOppositeDirection()
    {
        float dotDirection = Vector3.Dot(_currentMoveDirection, _previousMoveDirection);

        // Directions are opposite therefore cancel current coroutine and run it again
        if (Mathf.Approximately(dotDirection, -1f))
        {
            Debug.Log("Stop coroutine and change direction immediately!");

            // stop both the movement coroutine and the node coroutine
            if (_movementCor != null && _nodeCor != null)
            {
                StopCoroutine(_nodeCor);
                StopCoroutine(_movementCor);
            }

            _movementCor = StartCoroutine(RunMovement());

            // rotate the player here
            RotatePlayer();

            // update the previous direction
            _previousMoveDirection = _currentMoveDirection;
        }
        else
        {
            if (MovementState == State.Stopped)
                RotatePlayer();
        }
    }

    private void RotatePlayer()
    {
        // Update the visual direction of the object
        if (_visualObj != null)
            _visualObj.transform.rotation = Quaternion.LookRotation(_currentMoveDirection, Vector3.up);
    }

    private void UpdateMoveDirection(float horizontal, float vertical)
    {
        if (horizontal == 0f && vertical == 0f)
            return;

        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            // Check if horizontal input is greater, move along the x-axis
            _currentMoveDirection = new Vector3(Mathf.Sign(horizontal), 0, 0);
        }
        else
        {
            // Check if vertical input is greater, move along the z-axis
            _currentMoveDirection = new Vector3(0f, 0f, Mathf.Sign(vertical));
        }

        if (IsWall())
        {
            _currentMoveDirection = _previousMoveDirection;
        }
    }

    public Transform GetNextNodeInDirection(NodeGrid nodeGrid, Transform currentPlayerNode, Vector3 direction)
    {
        Transform nextNode = null;
        float shortestDistance = float.MaxValue;

        foreach (Transform node in nodeGrid.gridNodes)
        {
            if (node != currentPlayerNode)
            {
                Vector3 nodeToPlayer = node.position - currentPlayerNode.position;
                float dotProduct = Vector3.Dot(nodeToPlayer.normalized, direction);
                if (dotProduct > 0 && nodeToPlayer.magnitude < shortestDistance)
                {
                    shortestDistance = nodeToPlayer.magnitude;
                    nextNode = node;
                }
            }
        }

        return nextNode;
    }

    private IEnumerator RunMovement()
    {
        while (true)
        {
            // Get the next node in the calculated move direction
            Transform nextNode = GetNextNodeInDirection(_nodeGrid, _currentNode, _currentMoveDirection);

            // Check if there is a wall

            if (nextNode != null && !IsWall())
            {
                MovementState = State.Moving;
                
                // Update the currentPlayerNode with the nextNode
                _currentNode = nextNode;

                _nodeCor = StartCoroutine(MoveToNode(_nodeGrid, _currentNode));
                yield return _nodeCor;
            }
            else
            {
                MovementState = State.Stopped;

                // No next node, wait for input
                yield return null;
            }
        }
    }

    private IEnumerator MoveToNode(NodeGrid nodeGrid, Transform targetNode)
    {
        float elapsedTime = 0f;
        float normalizedTime = 0f;

        float distance = Vector3.Distance(_rigidbody.position, targetNode.position);
        float duration = distance / _movementSpeed;

        Vector3 initialPos = _rigidbody.position;
        _previousMoveDirection = _currentMoveDirection; // for direction checking when opposite

        do
        {
            elapsedTime += Time.deltaTime;
            normalizedTime = elapsedTime / duration;

            _rigidbody.MovePosition(Vector3.Lerp(initialPos, targetNode.position, normalizedTime));

            yield return null;
        }
        while (elapsedTime < duration);

        _rigidbody.MovePosition(targetNode.position);

        // Update the visual direction of the object
        RotatePlayer();
    }

    private bool IsWall()
    {
        // 4 directional
        Vector3[] directions = {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
        RaycastHit[] hits = new RaycastHit[directions.Length];

        // check all directions
        for (int i = 0; i < directions.Length; i++)
        {
            if (_currentMoveDirection == directions[i])
            {
                if (Physics.Raycast(_rigidbody.position, directions[i], out hits[i], (_nodeGrid.unitDistance + _addedRayDistance), _wallMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Vector3[] directions = {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};

        NodeGrid nodeGrid = FindObjectOfType<NodeGrid>();
        float rayDistance = nodeGrid.unitDistance;

        for (int i = 0; i < directions.Length; i++)
        {
            Debug.DrawLine(transform.position, transform.position + (directions[i] * (rayDistance + _addedRayDistance)), Color.yellow);
        }
    }
}