using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ABMU.Core;

public class AgentController : AbstractController
{
    [Header("Environment Variables")]
    public GameObject environmentCellPf;
    public Vector3Int environmentExtents;
    public Cell[] cells;
    public float cellSizeInWorld = 20f;

    [Header("Agent Variables")]
    public GameObject agentPf;
    public LayerMask agentLm;
    public LayerMask cellLm;
    List<Agent> agentsList = new List<Agent>();
    
    [Header("Simulation Variables")]
    [Range(int.MinValue + 64, int.MaxValue - 64)]
    public int randomSeed = 0;
    //[Range(0f, 1.0f)]
    //public float agentDensity = 0.2f;
    [Range(1, 100)]
    public int iterations = 5;
    public System.Random rand;

    public override void Init() {
        base.Init();

        rand = new System.Random(randomSeed);

        GenerateEnvironment();
        GenerateAgent();
    }

    public override void Step() {
        int agentsDone = 0;
        base.Step();

        foreach(Agent agent in agentsList) {
            if (!agent.alive) agentsDone++;
        }


        if (agentsDone == agentsList.Count) {
            if (iterations < 0) return;
            GenerateAgent();
            //EditorApplication.isPaused = true;
        }
    }

    void GenerateEnvironment() {
        List<Cell> cellsList = new List<Cell>();
        for (int i = 0; i < environmentExtents.y; i++) {
            for (int j = 0; j < environmentExtents.x; j++) {
                for (int k = 0; k < environmentExtents.z; k++) {
                    Vector3 pos = new Vector3(j, i, k) * cellSizeInWorld;
                    pos -= new Vector3(environmentExtents.x/2f, environmentExtents.y/2f, environmentExtents.z/2f) * cellSizeInWorld;
                    GameObject go = Instantiate(environmentCellPf, pos, Quaternion.identity);
                    go.GetComponent<Cell>().coords = new Vector3Int(j, i, k);
                    cellsList.Add(go.GetComponent<Cell>());
                }
            }
        }
        cells = cellsList.ToArray();

        foreach (Cell cell in cells) {
            cell.GetCellNeighbors(this);
        }
    }

    void GenerateAgent() {
        //int numAgents = Mathf.CeilToInt(agentDensity * ((environmentExtents.x * environmentExtents.y * environmentExtents.z) / 10));
        GameObject p = Instantiate(agentPf);
        Agent a = p.GetComponent<Agent>();
        Cell c = FindFreeCell();
        a.Init(c, randomSeed);
        agentsList.Add(a);
        iterations--;
    }

    public Cell FindFreeCell() {
        Cell c = cells[rand.Next(cells.Length)];

        while (c.occupied) {
            c = cells[rand.Next(cells.Length)];
        }

        return c;
    }
}
