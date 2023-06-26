using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int x; // x-coordinate of the node in the grid
    public int y; // y-coordinate of the node in the grid
    public Node[] neighbors; // array of neighboring nodes (up, down, left, right)

    // Variables for A*
    public int cost; // cost from the start node to this node
    public int heuristic; // heuristic estimate of the cost from this node to the end node
    public Node parent; // parent node for backtracking the path

    // Method to check if the next node is visitable from the current node using raycasts
    public bool IsNextNodeVisitable(Node nextNode)
    {
        // Grab the direction from current node and next node
        Vector3 direction = (nextNode.transform.position - transform.position).normalized;

        // Perform raycast in the specified direction to check for obstacles
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, NodeGrid.Instance.unitDistance, LayerMask.GetMask("Walls")))
        {
            // If the raycast hits an obstacle, the next node is not visitable
            return false;
        }

        return true;
    }


    // Method to calculate the total cost of the node (g + h)
    public int GetTotalCost()
    {
        return cost + heuristic;
    }
}
