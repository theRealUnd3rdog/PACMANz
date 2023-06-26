using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    // Implementation of the A* algorithm
    public List<Node> FindShortestPath(Node startNode, Node endNode)
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        // Add the start node to the open list
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // Find the node with the lowest total cost in the open list
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].GetTotalCost() < currentNode.GetTotalCost())
                {
                    currentNode = openList[i];
                }
            }

            // Move the current node from the open list to the closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // If the current node is the end node, path has been found
            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            // Check each neighbor of the current node
            foreach (Node neighbor in currentNode.neighbors)
            {
                if (neighbor == null)
                    continue;

                // Skip neighbors that are already in the closed list or are not visitable
                if (closedList.Contains(neighbor) || !currentNode.IsNextNodeVisitable(neighbor))
                {
                    continue;
                }

                // Calculate the cost to reach the neighbor from the current node
                int cost = currentNode.cost + CalculateDistance(currentNode, neighbor);

                // If the neighbor is not in the open list, add it
                if (!openList.Contains(neighbor))
                {
                    neighbor.cost = cost;
                    neighbor.heuristic = CalculateDistance(neighbor, endNode);
                    neighbor.parent = currentNode;
                    openList.Add(neighbor);
                }
                else
                {
                    // If the new cost is lower than the neighbor's current cost, update it
                    if (cost < neighbor.cost)
                    {
                        neighbor.cost = cost;
                        neighbor.parent = currentNode;
                    }
                }
            }
        }

        // No path found
        return null;
    }

    // Helper method to calculate the Manhattan distance between two nodes
    private int CalculateDistance(Node nodeA, Node nodeB)
    {
        return Mathf.Abs(nodeA.x - nodeB.x) + Mathf.Abs(nodeA.y - nodeB.y);
    }

    // Helper method to generate the final path from start to end
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }
}
