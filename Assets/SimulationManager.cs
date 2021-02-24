using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;

public class SimulationManager : Singleton<SimulationManager>
{
   [Header("Simulation Variables")]
   [Range(int.MinValue + 64, int.MaxValue - 64)]
   public int randomSeed = 0;
   public bool randomStart;

[Header("Environment Variables")]
public GameObject environmentCellPf;
public LayerMask cellLm;
public Vector3Int environmentExtents;
public float cellSizeInWorld = 20f;
[HideInInspector]
public Cell[] cells;

AgentController agentController;

   void Awake() {
       if (randomStart) randomSeed = Random.Range(int.MinValue + 64, int.MaxValue - 64);
       GenerateEnvironment();
       agentController = GameObject.FindObjectOfType<AgentController>().GetComponent<AgentController>();
   }

    void GenerateEnvironment() {
        List<Cell> cellsList = new List<Cell>();
        for (int i = 0; i < environmentExtents.y; i++) {
            for (int j = 0; j < environmentExtents.x; j++) {
                for (int k = 0; k < environmentExtents.z; k++) {
                    Vector3 pos = new Vector3(j, i, k) * SimulationManager.Instance.cellSizeInWorld;
                    pos -= new Vector3(environmentExtents.x/2f, environmentExtents.y/2f, environmentExtents.z/2f) * SimulationManager.Instance.cellSizeInWorld;
                    GameObject go = Instantiate(environmentCellPf, pos, Quaternion.identity, this.transform);
                    go.GetComponent<Cell>().coords = new Vector3Int(j, i, k);
                    cellsList.Add(go.GetComponent<Cell>());
                }
            }
        }
        cells = cellsList.ToArray();
        foreach (Cell cell in cells) {
            cell.GetCellNeighbors();
        }
    }

    public void AgentReproduce(Agent agent, GameObject pfChangs) {
        agentController.AgentReproduce(agent, pfChangs);
    }
}
