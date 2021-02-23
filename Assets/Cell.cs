using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;

public class Cell : MonoBehaviour
{
    public Vector3Int coords;
    public bool occupied = false;
    public AbstractAgent occupier;
    public Tunnel tunnel;
    public Room room;
    public Cell[] neighbors;

    public void GetCellNeighbors(AgentController agentController) {
        Collider[] others = Physics.OverlapBox(this.transform.transform.position, Vector3.one*agentController.cellSizeInWorld, Quaternion.identity, agentController.cellLm);
    
        List<Cell> ns = new List<Cell>();

        foreach(Collider other in others) {
            if (other.gameObject != this.gameObject) {
                ns.Add(other.GetComponent<Cell>());
            }
        }
        neighbors = ns.ToArray();
    }
}
