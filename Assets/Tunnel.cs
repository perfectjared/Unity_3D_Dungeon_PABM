using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;

public class Tunnel : MonoBehaviour
{
    public Cell cell;

    public void Init(Cell cell, float scale) {
        //base.Init();
        this.cell = cell;
        cell.occupied = true;
        cell.tunnel = this;

        this.transform.localScale = Vector3.one * scale;

        GetComponent<Renderer>().material.color = Color.blue;
    }
}
