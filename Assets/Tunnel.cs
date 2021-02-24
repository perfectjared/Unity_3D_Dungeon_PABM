using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;

public class Tunnel : MonoBehaviour
{
    [HideInInspector]
    public Cell cellOrigin;
    [HideInInspector]
    public List<Cell> cells;
    [HideInInspector]
    AgentUtilities.Dir dir;
    //room size parameter//
    [Range(1, 4)]
    public int size;
    public Color color;


    public void Init(Cell cell, AgentUtilities.Dir dir) {
        //base.Init();
        this.cellOrigin = cellOrigin;
        this.cells = cell.GetCellNeighbors(size);
        this.dir = dir;
        cell.occupied = true;
        cell.tunnel = this;

        this.transform.localScale = (Vector3.one * size)* SimulationManager.Instance.cellSizeInWorld;

        GetComponent<Renderer>().material.color = color;
    }
}
