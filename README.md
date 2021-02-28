# ABM_map
###### Unity 3D Implementation of parameterized agent-based modeling for 3D "Metroidvania"-style map design

## How to Use 
1. Open the ABM_map project using Unity 2018
2. Open the PABM.unity scene
3. To test behavior withiout changing any parameters, simply press Play
4. Enter scene view to view the generated design and change camera angle/height etc.
5. In the project explorer, navigate to the Prefabs folder
6. This folder contains room, roomAgent, tunnel, and tunnelAgent
7. room and tunnel are *Cells*, you can edit their (few) parameters by clicking on them and changing the values in the Node component
  * Size: Change the number times that a Cell expands outward
  * Color: The color of the cell
8. roomAgent and tunnelAgent are *Agents*, you can editor their paramaters by clicking on them and changing the values in the Agent component
  * Overlap: Whether or not this agent will draw overlapping paths
  * Life: How much "life" the agent has (steps before it will die)
  * Even Movement: Whether the agent moves on 1s or 2s
  * XZ / Y Movement % : The percentage chance that on a given move the agent will move in the XZ direction, or the Y direction
  * X/Z Movement, Floor, Ceiling: The distance the agent moves in the XZ direction, according to a curve, floor, and ceiling
  * Y Movement, Floor, Ceiling: The distance the agent moves in the Y direction, according to a curve, floor, and ceiling
  * Path prefab: The type of cell the agent instantiates as traces a path
  * Reproduction prefab: The type of agent the agent instantiates when it reproduces
  * Reproduce chance: The likelihood (0 - 1) on a given move that the agent will reproduce and instantiate a new agent adjacently
    * Child has same life: Does the reproduced agent start with full life, or the same amount of life the agent who produced it has
9. In the Hierarchy, navigate to Managers
10. Managers contains three components: Simulation Manager, Agent Controller, and Agent Spawn List
11. Simulation Manager contains simulation variables, including the random seed used, the environmental cells to be instantiated, the size of the simulation environment in cells, and how large the cells are
  * Random Seed: What seed does the random generation use to generate?
  * Random Start: Does the program choose a random seed upon starting?
  * Environment Cell Pf: The cell prefab the simulation manager populates the environment with
  * Cell Lm: The LayerMask of cells (unused)
  * Environment Extents: The X, Y, and Z size of the simulation environment
  * Cell Size in World: Size of the world
12. Agent Controller: Maintains a list of Agent Spawns (The adjacent Agent Spawn List) and spawns and controls them accordingly
  * Is Simulation Paused: Self-explanatory
  * End Frame: (unused)
13. Agent Spawn List: Contains a list of agents to spawn in order, as well as parameters for their spawning
  * Agent Pf: Prefab containing the agent to be spawned
  * Layer Mask: The layerMask of this agent (unused)
  * Iterations: How many times to spawn this agent before moving to the next
14. In order to create custom agent/room interactions, define *cell* or *agent* prefabs based on the cell and agent prefabs in Prefabs/base
15. Create a composition of instantiation: Which agent prefabs does the Agent Manager instantiate, which are instantiated by reproduction, which agents produce which types of cells, etc. By customizing the parameterization of agents, the Agent Manager, and cells, you are able to build custom dungeon designs.

## Screenshots
![image](https://user-images.githubusercontent.com/26883388/109430855-32505180-79b8-11eb-93e1-c00abe4a48a9.png)
![image](https://user-images.githubusercontent.com/26883388/109430871-42683100-79b8-11eb-82db-aeddf6cee88b.png)
![image](https://user-images.githubusercontent.com/26883388/109430932-996e0600-79b8-11eb-836d-36cda4b84d8a.png)
![image](https://user-images.githubusercontent.com/26883388/109430943-ab4fa900-79b8-11eb-9d9c-f3b54f74d927.png)

## Overview
https://user-images.githubusercontent.com/26883388/109431332-e357eb80-79ba-11eb-998d-5d58526e2657.mp4
