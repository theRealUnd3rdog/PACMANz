using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Chasing,
    Random,
}

public abstract class Enemy : MonoBehaviour , ICollidePlayer
{
    public EnemyState State;

    protected Rigidbody _rigidbody;

    [Header("Path Finding")]
    [SerializeField] protected PathFinder pathFinder; // Reference to the PathFinder component
    [SerializeField] protected Node _spawnNode; // Node that the enemy spawns in

    [Tooltip("Max distance for enemy to move to a random node")]
    [Range(1f, 40f)] [SerializeField] protected float _maxDistanceForRandomNode;
    [Range(2f, 10f)] [SerializeField] protected float _durationToFollowPlayer; 
    protected float _timerOnFollow; // current timer to following the player
    [Range(0.1f, 0.7f)] [SerializeField] protected float _probabilityToFollow;

    protected PlayerMovement _playerMovement;
    protected Node _currentNode; // Current node the enemy is in
    protected Coroutine _traversalCoroutine; // Coroutine that follows the path
    protected List<Node> _currentPath; // Current path being traversed
    protected Node _traversalDestination; // Traversal destination node

    [Header("Movement")]
    [SerializeField] private bool _trapped; // change this bool if the enemy is supposed to be trapped initially

    public bool Trapped
    {
        get {return _trapped;}
        set
        {
            if (_trapped != value)
            {
                _trapped = value;
                OnTrappedChanged(_trapped);
            }
        }
    }

    public event System.Action<bool> TrappedChange; // event that runs whenever the player gets trapped

    [SerializeField] protected float _movementSpeed = 2f;

    protected Coroutine _movementCoroutine; // Coroutine that is being used to move

    // detection
    public bool collidedWithPlayer {get; set;} = false;

    private void OnTrappedChanged(bool newTrappedState)
    {
        TrappedChange?.Invoke(newTrappedState);
    }

    public virtual void OnTrappedStateChanged(bool newTrappedState)
    {
        Debug.Log($"Trapped state changed: {newTrappedState}");

        // Perform actions or logic based on the new trapped state
    }

    public virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerMovement = FindObjectOfType<PlayerMovement>();
    }

    public virtual void Start()
    {
        TrappedChange += OnTrappedStateChanged;

        State = EnemyState.Idle;

        _currentNode = _spawnNode;
        _maxDistanceForRandomNode += NodeGrid.Instance.unitDistance; // It has to be more than the node distance for sure though

        _rigidbody.position = _spawnNode.transform.position;
        _movementCoroutine = StartCoroutine(FollowRandomPathWithPlayer());

        // Use PlayerMovement event to follow player whenever player's current node changes
        //_playerMovement.OnCurrentNodeChanged += FollowPlayer;
    }

    public virtual void Destroy()
    {
        TrappedChange -= OnTrappedStateChanged;
        //_playerMovement.OnCurrentNodeChanged -= FollowPlayer;
    }

    public virtual void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !collidedWithPlayer)
        {
            Debug.Log($"Collided with Player!");
            IKillable killable = collider.GetComponent<IKillable>();

            if (killable != null)
                killable.Kill();

            collidedWithPlayer = true;
        }
    }

    public virtual void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player") && collidedWithPlayer)
        {
            Debug.Log($"Exitted with Player!");
            collidedWithPlayer = false;
        }
    }

    public void StopMovement()
    {
        State = EnemyState.Idle;
        Trapped = true;
        
        if (_movementCoroutine != null)
            StopCoroutine(_movementCoroutine);

        if (_traversalCoroutine != null)
            StopCoroutine(_traversalCoroutine);
    }

    public void FollowPlayer(Node playerNode)
    {
        State = EnemyState.Chasing;

        Debug.Log("Following player");
        FollowPath(playerNode);
    }

    // Test method to find the closest node
    public Node FindClosestNode(Vector3 position)
    {
        // Ignore the Y-axis of the click position
        position.y = 0f;

        Node closestNode = null;
        float closestDistance = Mathf.Infinity;

        // Iterate through all nodes in the grid and find the closest one
        foreach (Node node in NodeGrid.Instance.gridNodes)
        {
            // Calculate the distance between the click position and the node's position
            float distance = Vector3.Distance(position, node.transform.position);

            // Update the closest node if a closer one is found
            if (distance < closestDistance)
            {
                closestNode = node;
                closestDistance = distance;
            }
        }

        return closestNode;
    }

    public void FollowPath(Node destinationNode)
    {
        if (_traversalCoroutine != null)
            StopCoroutine(_traversalCoroutine);
        
        _currentPath = pathFinder.FindShortestPath(_currentNode, destinationNode);

        if (_currentPath == null || _currentPath.Count == 0)
        {
            Debug.Log("No path available");
            return;
        }

        // Set the traversal destination to the last node in the path
        _traversalDestination = destinationNode;

        _traversalCoroutine = StartCoroutine(TraversePath(_currentPath, _movementSpeed));
    }

    public void FollowRandomPath(float maxDistance)
    {
        State = EnemyState.Random;

        
        // Find a random node within the specified distance from the current node
        Node randomNode = GetRandomNodeWithinDistance(_currentNode, maxDistance);

        // If a valid random node is found, follow the path to it
        if (randomNode != null)
        {
            FollowPath(randomNode);
            Debug.Log($"Following random path at {randomNode.transform.position}");
        }
    }

    private Node GetRandomNodeWithinDistance(Node currentNode, float maxDistance)
    {
        // Create a list to store the valid nodes within distance
        List<Node> validNodes = new List<Node>();

        // Iterate through all nodes in the grid
        foreach (Node node in NodeGrid.Instance.gridNodes)
        {
            // Calculate the distance between the current node and the iterated node
            float distance = Vector3.Distance(currentNode.transform.position, node.transform.position);

            // Check if the distance is within the specified range and the node is not the current node
            if (distance <= maxDistance && node != currentNode)
            {
                validNodes.Add(node);
            }
        }

        // If no valid nodes are found, return null
        if (validNodes.Count == 0)
        {
            return null;
        }

        // Select a random node from the valid nodes list
        int randomIndex = Random.Range(0, validNodes.Count);
        return validNodes[randomIndex];
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.GetChild(0).rotation = targetRotation;
    }

    private IEnumerator FollowRandomPathWithPlayer()
    {
        if (Trapped)
            yield return new WaitForSeconds(EnemyManager.Instance.gateDuration);

        Trapped = false;

        bool isFollowingPlayer = false; // Flag to track if the enemy is following the player

        while (true)
        {
            // Follow a random path
            FollowRandomPath(_maxDistanceForRandomNode);

            // Wait until the destination node of the random path is reached
            while (_currentNode != null && _currentNode != _traversalDestination)
            {
                yield return null;
            }

            // Randomly decide whether to follow the player or not
            bool shouldFollowPlayer = Random.value < _probabilityToFollow; // Adjust the probability as needed

            if (shouldFollowPlayer)
            {
                // Check if the enemy is already following the player
                if (!isFollowingPlayer)
                {
                    isFollowingPlayer = true; // Set the flag to indicate that the enemy is now following the player

                    // Start a timer to track the duration of following the player
                    _timerOnFollow = 0f;
                    StartCoroutine(WaitForTimer());

                    while (_timerOnFollow < _durationToFollowPlayer)
                    {
                        // Get the player's current node
                        Node playerNode = _playerMovement.GetCurrentNode();

                        // Follow the player
                        FollowPlayer(playerNode);

                        // Check if the player's current node has changed
                        yield return new WaitUntil(() => playerNode != _playerMovement.GetCurrentNode() || _timerOnFollow > _durationToFollowPlayer);
                    }

                    // Reset the flag if the duration has elapsed
                    isFollowingPlayer = false;
                }
            }
            else
            {
                isFollowingPlayer = false; // Reset the flag if the enemy is not following the player
            }
        }
    }

    private IEnumerator WaitForTimer()
    {
        _timerOnFollow = 0f;

        do
        {
            _timerOnFollow += Time.deltaTime;

            yield return null;
        }
        while (_timerOnFollow < _durationToFollowPlayer);
    }

    private IEnumerator TraversePath(List<Node> path, float speed)
    {
        int currentNodeIndex = 0;

        while (currentNodeIndex < path.Count)
        {
            Node currentNode = path[currentNodeIndex];
            _currentNode = FindClosestNode(_rigidbody.position);
            Vector3 targetPosition = currentNode.transform.position;

            RotateTowardsTarget(targetPosition);

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            // Make sure the object is positioned exactly at the target node
            _rigidbody.position = targetPosition;

            // Check if the enemy has moved to the next node
            if (currentNodeIndex < path.Count - 1)
            {
                currentNodeIndex++;
            }
            else
            {
                break; // Reached the end of the path
            }
        }

        // Set the final node in the path as the _currentNode
        _currentNode = path[path.Count - 1];
    }


    private void VisualizePath(List<Node> path)
    {
        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Node currentNode = path[i];
                Node nextNode = path[i + 1];

                Vector3 startPosition = new Vector3(currentNode.transform.position.x, 0f, currentNode.transform.position.z);
                Vector3 endPosition = new Vector3(nextNode.transform.position.x, 0f, nextNode.transform.position.z);

                Debug.DrawLine(startPosition, endPosition, Color.white, 6f);
            }
        }
    }
}
