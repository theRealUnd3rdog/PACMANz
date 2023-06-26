using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingExample : MonoBehaviour
{
    public PathFinder pathFinder; // Reference to the PathFinder component
    public Node[,] grid; // 2D array representing the grid of nodes
    public Node startNode; // Start node
    public Node endNode; // End node
    
    public List<Node> shortestPath;

    private void Start()
    {
        // Call the A* algorithm to find the shortest path
        shortestPath = pathFinder.FindShortestPath(startNode, endNode);

        // Print the shortest path (for testing purposes)
        if (shortestPath != null)
        {
            foreach (Node node in shortestPath)
            {
                Debug.Log("Node: " + node.x + ", " + node.y);
            }
        }
        else
        {
            Debug.Log("No path found!");
        }
    }

    private void Update()
    {
        VisualizePath();
    }

    private void VisualizePath()
    {
        if (shortestPath != null)
        {
            for (int i = 0; i < shortestPath.Count - 1; i++)
            {
                Node currentNode = shortestPath[i];
                Node nextNode = shortestPath[i + 1];

                Vector3 startPosition = new Vector3(currentNode.transform.position.x, 0f, currentNode.transform.position.z);
                Vector3 endPosition = new Vector3(nextNode.transform.position.x, 0f, nextNode.transform.position.z);

                Debug.DrawLine(startPosition, endPosition, Color.red, 2f);
            }
        }
    }
}
