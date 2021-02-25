using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;

//todo: the prefab thing
//"continuity" - how likely is an agent to be created by splitting off another agent?
// this is to create intersections !
// perhaps at certain turns (not up/down) add to a list of potential split points?
// another idea: a parameter making it so that it can only move in even or odd numbers
// ^ this will affect adjacency! evens mean nothing right next to each other !
// ^ YOU HAVE TO MAKE SPAWNING ONLY HAPPEN ON EVENS/ODDS TOO !!!
// have branches able to join w another path & then die!
//but first: you have to take care of yourself. eat ! and splash your face! wash some dishes!
//i love you !!

public class Agent : AbstractAgent
{
    [Header("Parameters")]
    public bool overlap;
    [Range(1, 500)]
    public int life = 20;
    [Range(1, 9)]
    public int distMod = 2;
    [Range(0f, 1.0f)]
    public float xzChance;
    public AnimationCurve xzDist;
    public int xzDistFloor;
    public int xzDistCeil;
    int xzAmplitude;
    [Range(0f, 1.0f)]
    public float yChance;
    public AnimationCurve yDist;
    public int yDistFloor;
    public int yDistCeil;
    int yAmplitude;
    float dirChance;

    [Header("Instatiate")]
    public GameObject instantiatePf;
    [Range(0f, 1f)]
    public float reproduceChance = 0.1f;
    public bool reproduceSameLife;
    [HideInInspector]
    public int children = 0;
    public GameObject reproducePf;

   //[Header("Agent-Environment Interactions")]

   // [Header("Agent-Agent Interactions")]

   [Header("Runtime")]
   public bool alive = true;
    [HideInInspector]
    public double id;

    Cell[] neighbors = new Cell[6];

    [HideInInspector]
    public GameObject pathParent;
    [HideInInspector]
    public List<Node> path;

    [HideInInspector]
    public Cell currentCell;
    [HideInInspector]
    public Cell lastCell;

    AgentUtilities.Dir dir;

    int randomSeed;
    System.Random random;

    public void Init(Cell cell, double id) {
        base.Init();

        currentCell = cell;
        this.transform.position = cell.transform.position;
        SnapSpawn();
        currentCell.occupied = true;
        currentCell.occupier = this;

        this.id = id;

        //Parameters
        xzAmplitude = xzDistCeil - xzDistFloor;
        yAmplitude = yDistCeil - yDistFloor;
        dirChance = xzChance + yChance;

        randomSeed = SimulationManager.Instance.randomSeed;
        random = new System.Random((int)(randomSeed / id));

        this.transform.localScale = Vector3.one * SimulationManager.Instance.cellSizeInWorld;
        GetComponent<Renderer>().material.color = Color.red;

        //Code
        pathParent = new GameObject();
        pathParent.name = "path" + id;
        this.name = "agent" + id;

        CreateStepper(Move);
        CreateStepper(Reproduce);
    }

    void Reproduce() {
        if (!alive) return;
        float reproduce = (float)random.NextDouble();
        if (reproduce <= reproduceChance) {
            children++;
            GameObject go = Instantiate(reproducePf, this.transform.position, Quaternion.identity);
            if (go.GetComponent<Node>()) go.GetComponent<Node>().Init(currentCell, dir);
            if (go.GetComponent<Agent>()) go.GetComponent<Agent>().Init(currentCell, this.id + 1);
            SimulationManager.Instance.AgentReproduce(this, go);
        }
    }

    void Move() {
        if (!alive) return;
        float moveAxis = (float)random.NextDouble() * dirChance;
        int moveDistance = 0;
        AgentUtilities.Dir moveDirection = 0;

        if (moveAxis < yChance) {
            moveDistance = (yAmplitude == 0) ? yDistFloor : ((int)yDist.Evaluate((float)random.NextDouble() * yAmplitude) + yDistFloor);
            moveDirection = (AgentUtilities.Dir)random.Next(2);
        } else {
            moveDistance = (yAmplitude == 0) ? xzDistFloor : ((int)(xzDist.Evaluate((float)random.NextDouble() * xzAmplitude)) + xzDistFloor);
            moveDirection = (AgentUtilities.Dir)random.Next(4) + 2;
        }

        //Make sure that the agent is moving in multiples of distMod
        moveDistance = (int)Mathf.Round(moveDistance / distMod) * distMod;

        Cell moveNext = currentCell;
        List<Cell> movePath = new List<Cell>();

        lastCell = currentCell;
        for (int i = 0; i < moveDistance; i++) {
            moveNext = MoveDir(moveDirection);
            if (moveNext == null) {
                ReduceLife();
                movePath.Clear();
                MoveTo(lastCell);
                Move();
                break;
            } else {
                movePath.Add(moveNext);
                MoveTo(moveNext);
            }
            ReduceLife();
        }
        foreach (Cell cell in movePath) {
            PlaceNode(cell);
        }
    }

    Cell MoveDir(AgentUtilities.Dir direction) {
        FindNeighbors();
        Cell cell = neighbors[(int)direction];
        return cell;
    }

    void MoveTo(Cell cell) {
        currentCell = cell;
        this.transform.position = cell.transform.position;
        currentCell.occupied = true;
        currentCell.occupier = this;
    }

    void FindNeighbors() {
        Array.Clear(neighbors, 0, 6);
        if (currentCell.neighbors.Length == 0) {
            ReduceLife();
            return;
        }
        foreach (Cell neighbor in currentCell.neighbors) {
            if (!neighbor.occupied || overlap) {
                Vector3Int coords = neighbor.coords - currentCell.coords;
                switch (coords) {
                    case Vector3Int v when v.Equals(Vector3Int.up):
                        dir = AgentUtilities.Dir.UP;
                        neighbors[(int)dir] = neighbor;
                        break;
                    case Vector3Int v when v.Equals(Vector3Int.down):
                        dir = AgentUtilities.Dir.DOWN;
                        neighbors[(int)dir] = neighbor;
                        break;
                    case Vector3Int v when v.Equals(Vector3Int.left):
                        dir = AgentUtilities.Dir.LEFT;
                        neighbors[(int)dir] = neighbor;
                        break;
                    case Vector3Int v when v.Equals(Vector3Int.right):
                        dir = AgentUtilities.Dir.RIGHT;
                        neighbors[(int)dir] = neighbor;
                        break;
                    case Vector3Int v when v.Equals(new Vector3Int(0, 0, 1)):
                        dir = AgentUtilities.Dir.FORWARD;
                        neighbors[(int)dir] = neighbor;
                        break;
                    case Vector3Int v when v.Equals(new Vector3Int(0, 0, -1)):
                        dir = AgentUtilities.Dir.BACKWARD;
                        neighbors[(int)dir] = neighbor;
                        break;
                }
            }
        }
    }

    void PlaceNode(Cell cell) {
        GameObject go = Instantiate(instantiatePf, cell.transform.position, Quaternion.identity, pathParent.transform);
        Node tunnel = go.GetComponent<Node>();
        tunnel.Init(cell, dir);
        path.Add(tunnel);
    }

    void ReduceLife() {
        life--;
        if (life == 0) alive = false;
    }

    void SnapSpawn() {
        SimulationManager sM = SimulationManager.Instance;
        Vector3 currPos = currentCell.transform.position;
        int distMod = (int) (this.distMod * sM.cellSizeInWorld);

        //Check if spawned on valid point given distMod
        if (currPos.x % distMod == 0 && currPos.y % distMod == 0 && currPos.z % distMod == 0) return;


        //If not, determine what the closest valid one is
        int[] idealPos = { (int)Mathf.Round(currPos.x / distMod) * distMod, (int)Mathf.Round(currPos.y / distMod) * distMod, (int)Mathf.Round(currPos.z / distMod) * distMod };


        //Constrain the snapped value appropriately
        for (int i = 0; i < 3; i++) {
            int bound = 0;

            switch(i) {
                case 0: bound = sM.environmentExtents.x;
                break;
                case 1: bound = sM.environmentExtents.y;
                break;
                case 2: bound = sM.environmentExtents.z;
                break;
            }

            bound = (int)Mathf.Round(bound / distMod) * distMod;

            idealPos[i] = Mathf.Max(0, idealPos[i]);
            idealPos[i] = Mathf.Min(bound, idealPos[i]);
        }

        //Find the cell at this location
        Vector3 newPos = new Vector3(idealPos[0], idealPos[1], idealPos[2]);
        foreach (Cell cell in sM.cells) {
            if (cell.transform.position == newPos) currentCell = cell;
        }
        this.transform.position = newPos;
    }

}

public class AgentUtilities {
    public enum Dir {
        UP, DOWN, LEFT, RIGHT, FORWARD, BACKWARD
    }

}
