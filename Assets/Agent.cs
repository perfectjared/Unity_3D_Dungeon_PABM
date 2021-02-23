using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;

public class Agent : AbstractAgent
{
    [Header("Parameters")]
    public bool overlap;
    [Range(1, 500)]
    public int life = 20;

   // [Header("Actions")]

   [Header("Agent-Environment Interactions")]
    public GameObject tunnelPf;
    public GameObject roomPf;
   // [Header("Agent-Agent Interactions")]

   [Header("Runtime")]
   [Range(int.MinValue + 64, int.MaxValue - 64)]
   public int randomSeed; //TODO: make randomSeed a global var in a singleton
   public System.Random rand;
    public AgentController agentController;
    public List<Cell> neighbors;
    public List<Room> chamber;
    public List<Tunnel> path;
    Cell currentCell;
    public bool alive = true;
    public bool roomPlaced = false;

    public void Init(Cell cell, int seed) {
        base.Init();
        agentController = GameObject.FindObjectOfType<AgentController>();

        randomSeed = seed;
        rand = new System.Random(randomSeed);

        currentCell = cell;
        this.transform.position = cell.transform.position;
        currentCell.occupied = true;
        currentCell.occupier = this;

        this.transform.localScale = Vector3.one * agentController.cellSizeInWorld;

        GetComponent<Renderer>().material.color = Color.green;


        if(roomPlaced){
          GetComponent<Renderer>().material.color = Color.red;
        }

        CreateStepper(Move);
    }

    void placeRoom()
    {
        GameObject make = Instantiate(roomPf, currentCell.transform.position, Quaternion.identity );
        Room r = make.GetComponent<Room>();
        for(int i = 0; i < r.size; i++){
          GetComponent<Renderer>().material.color = Color.green;


        }
        chamber.Add(r);
        Debug.Log("placing room");
    }

    void PlaceTunnel() {
        GameObject go = Instantiate(tunnelPf, currentCell.transform.position, Quaternion.identity);
        Tunnel t = go.GetComponent<Tunnel>();
        t.Init(currentCell, agentController.cellSizeInWorld);
        path.Add(t);
        placeRoom();
        //Debug.Log("placing tunne");
    }



    void FindNeighbors() {
        neighbors.Clear();
        foreach (Cell neighbor in currentCell.neighbors) {
            if (!neighbor.occupied || overlap) neighbors.Add(neighbor);
        }
    }

    Cell MoveTo() {
        Cell cell = neighbors[rand.Next(neighbors.Count)];
        return cell;
    }

    // Cell ExpandRoom(int index){
    //   Cell cell = neighbors[index + 1];
    // }



    void Move() {
        if (alive) {
            FindNeighbors();
            PlaceTunnel();
            roomPlaced = true;
            Cell cell = MoveTo();
            currentCell = cell;
            this.transform.position = cell.transform.position;
            currentCell.occupied = true;
            currentCell.occupier = this;
            life--;
            if (life == 0) {
                alive = !alive;
                print("dead");
            }
        } else {

        }
    }

}

public class AgentUtilities {
    public enum agentType {
        RED, GREEN
    }
}
