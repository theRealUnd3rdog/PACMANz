using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletManager : MonoBehaviour
{
    // managers
    public static PelletManager Instance;
    private NodeGrid _nodeGrid;
    
    [Header("Pellet")]
    [SerializeField] private GameObject _pelletObj;
    public AudioSource audioSource;
    public List<Pellet> pellets = new List<Pellet>();

    private void Awake()
    {
        Instance = this;

        ScatterPellets();
    }

    private void ScatterPellets()
    {
        // loop through each node in grid
        foreach (Node nodes in NodeGrid.Instance.gridNodes)
        {
            // skip ghostnodes or nodes that the player cannot go through
            if (nodes.type == NodeType.GhostNode || nodes.type == NodeType.StartNode)
                continue;

            GameObject pelletObj = Instantiate(_pelletObj, nodes.transform.position, Quaternion.identity);
            pelletObj.transform.SetParent(this.transform);
        
            Pellet pellet = pelletObj.GetComponent<Pellet>();

            pellets.Add(pellet);
        }
    }
}
