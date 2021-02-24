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
    public Cell[] neighbors;

    public List<Cell> GetCellNeighbors(int size = 1, bool findNeighbors = true) {
        Collider[] others = Physics.OverlapBox(this.transform.transform.position, (Vector3.one * size) * SimulationManager.Instance.cellSizeInWorld, Quaternion.identity, SimulationManager.Instance.cellLm);

        List<Cell> ns = new List<Cell>();

        foreach(Collider other in others) {
            if (other.gameObject != this.gameObject) {
                ns.Add(other.GetComponent<Cell>());
            }
        }

        if(findNeighbors)neighbors = GetCellNeighbors(size + 1, false).ToArray();
        return ns;

    }
}
