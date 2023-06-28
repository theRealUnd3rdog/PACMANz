using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    [SerializeField] private Enemy _enemy; // Reference to the Enemy component

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Get the click position
            Vector3 clickPosition = Input.mousePosition;
            clickPosition.z = Camera.main.transform.position.y; // Set the Z-coordinate to the camera's Y-coordinate
            clickPosition = Camera.main.ScreenToWorldPoint(clickPosition);

            // Find the closest node to the click position
            Node closestNode = _enemy.FindClosestNode(clickPosition);

            // Follow the path to the closest node
            if (closestNode != null)
            {
                _enemy.FollowPath(closestNode);
            }
        }
    }
}

