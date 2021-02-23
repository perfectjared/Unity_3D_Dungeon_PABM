using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;

public class Room : MonoBehaviour
{
    public Cell cell;
    public int size;

    public void Init(Cell cell, float scale)
    {
        //base.Init();
        this.cell = cell;
        cell.occupied = true;
        cell.room = this;
        this.size = 3;

        this.transform.localScale = Vector3.one * scale;

        GetComponent<Renderer>().material.color = Color.green;
    }
}
