using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ABMU.Core;

//RN: move generate environment to SimulationManager

public class AgentController : AbstractController
{
    [Header("Agent Variables")]
    public GameObject agentPf;
    public LayerMask agentLm;
    List<Agent> agentsList = new List<Agent>();

    [Header("Simulation Variables")]
    private int randomSeed;
    [Range(1, 100)]
    public int iterations = 5;
    public System.Random rand;
    Cell[] cells;

    void Start() {
        cells = SimulationManager.Instance.cells;

        randomSeed = SimulationManager.Instance.randomSeed;
        rand = new System.Random(randomSeed);

        GenerateAgent();
    }

    public override void Step() {
        int agentsDone = 0;
        base.Step();

        foreach(Agent agent in agentsList) {
            if (!agent.alive) agentsDone++;
        }


        if (agentsDone == agentsList.Count) {
            if (iterations == 0) return;
            GenerateAgent();
            //EditorApplication.isPaused = true;
        }
    }

    void GenerateAgent() {
        GameObject pf = Instantiate(agentPf);
        Agent agent = pf.GetComponent<Agent>();
        Cell cell = FindFreeCell();
        agent.Init(cell, agentsList.Count + 1);
        agentsList.Add(agent);
        iterations--;
    }

    public void AgentReproduce(Agent parent, GameObject pfChangs) {
        Agent agent = pfChangs.GetComponent<Agent>();
        Cell cell = parent.lastCell;
        if (agent.reproduceSameLife) agent.life = parent.life;
        agent.Init(cell, parent.id + (parent.children * .1));
        agentsList.Add(agent);
    }

    public Cell FindFreeCell() {
        Cell c = cells[rand.Next(cells.Length)];

        while (c.occupied) {
            c = cells[rand.Next(cells.Length)];
        }

        return c;
    }
}
