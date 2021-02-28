using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABMU.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Agent : AbstractAgent
{
    [Header("Movement Parameters")]
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

    [Header("Instatiation Parameters")]
    public GameObject instantiatePf;
    [Range(0f, 1f)]
    public float reproduceChance = 0.1f;
    public bool reproduceSameLife;
    [HideInInspector]
    public int children = 0;
    public GameObject reproducePf;

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
        if (!alive) {
            print("test");
            this.enabled = false;
            return;
        }
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

#if UNITY_EDITOR
[CustomEditor(typeof(Agent))]
public class AgentEditor : Editor {
    Agent agent;
    SerializedObject objTarget;
    bool evenMovement = false;
    int movementChance = 50;

    void OnEnable() {
        agent = (Agent)target;
        objTarget = new SerializedObject(agent);
    }
    
    public override void OnInspectorGUI() {
        objTarget.Update();

        SerializedProperty overlap = objTarget.FindProperty("overlap");
        EditorGUILayout.PropertyField(overlap);

        SerializedProperty life = objTarget.FindProperty("life");
        EditorGUILayout.PropertyField(life);

        evenMovement = EditorGUILayout.Toggle("Even Movement", evenMovement);
        agent.distMod = evenMovement ? 2 : 1;
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        movementChance = EditorGUILayout.IntSlider("XZ / Y Movement %", movementChance, 1, 100);
        agent.xzChance = (float)movementChance / 100;
        agent.yChance = 1 - ((float)movementChance / 100);

        EditorGUILayout.Space();

        SerializedProperty xzDist = objTarget.FindProperty("xzDist");
        EditorGUILayout.PropertyField(xzDist, new GUIContent("X/Z Movement"));
        EditorGUILayout.BeginHorizontal();
        SerializedProperty xzDistFloor = objTarget.FindProperty("xzDistFloor");
        EditorGUILayout.PropertyField(xzDistFloor, new GUIContent("Floor"));
        SerializedProperty xzDistCeil = objTarget.FindProperty("xzDistCeil");
        EditorGUILayout.PropertyField(xzDistCeil, new GUIContent("Ceiling"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        SerializedProperty yDist = objTarget.FindProperty("yDist");
        EditorGUILayout.PropertyField(yDist, new GUIContent("Y Movement"));
        EditorGUILayout.BeginHorizontal();
        SerializedProperty yDistFloor = objTarget.FindProperty("yDistFloor");
        EditorGUILayout.PropertyField(yDistFloor, new GUIContent("Floor"));
        SerializedProperty yDistCeil = objTarget.FindProperty("yDistCeil");
        EditorGUILayout.PropertyField(yDistCeil, new GUIContent("Ceiling"));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        SerializedProperty instantiatePf = objTarget.FindProperty("instantiatePf");
        EditorGUILayout.PropertyField(instantiatePf, new GUIContent("Path prefab"));
        SerializedProperty reproducePf = objTarget.FindProperty("reproducePf");
        EditorGUILayout.PropertyField(reproducePf, new GUIContent("Reproduction prefab"));
        SerializedProperty reproduceChance = objTarget.FindProperty("reproduceChance");
        EditorGUILayout.PropertyField(reproduceChance, new GUIContent("Reproduce chance"));
        SerializedProperty reproduceSameLife = (objTarget.FindProperty("reproduceSameLife"));
        EditorGUILayout.PropertyField(reproduceSameLife, new GUIContent("Child has same life"));



        objTarget.ApplyModifiedProperties();
    }
}
#endif
