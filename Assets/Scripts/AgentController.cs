using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(AgentSpawnList))]
public class AgentController : AbstractController
{
    List<AgentSpawnList.AgentSpawn> agentSpawnList;
    List<Agent> agentsList = new List<Agent>();
    int randomSeed;
    System.Random rand;
    Cell[] cells;
    int step;


    void Start() {
        agentSpawnList = GetComponent<AgentSpawnList>().agentSpawnList;
        cells = SimulationManager.Instance.cells;

        randomSeed = SimulationManager.Instance.randomSeed;
        rand = new System.Random(randomSeed);

        GenerateAgent();
    }

    public override void Step() {
        int agentsDone = 0;
        base.Step();
        if (step >= agentSpawnList.Count) return;

        foreach(Agent agent in agentsList) {
            if (!agent.alive) agentsDone++;
        }


        if (agentsDone == agentsList.Count) {
            if (agentSpawnList[step].iterations == 0) step++;
            else {
                GenerateAgent();
                agentSpawnList[step].iterations--;
            }
        }
    }

    void GenerateAgent() {
        GameObject pf = Instantiate(agentSpawnList[step].agentPf);
        Agent agent = pf.GetComponent<Agent>();
        Cell cell = FindFreeCell();
        agent.Init(cell, agentsList.Count + 1);
        agentsList.Add(agent);
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