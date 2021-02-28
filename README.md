# ABM_map
###### Unity 3D Implementation of parameterized agent-based modeling for 3D "Metroidvania"-style map design

## Use 
1. Open the ABM_map project using Unity 2018
2. Open the PABM.unity scene
3. In the project explorer, navigate to the Prefabs folder
4. This folder contains room, roomAgent, tunnel, and tunnelAgent
5. room and tunnel are *Cells*, you can edit their (few) parameters by clicking on them and changing the values in the Node component
  * Size: Change the number times that a Cell expands outward
  * Color: The color of the cell
7. roomAgent and tunnelAgent are *Agents*, you can editor their paramaters by clicking on them and changing the values in the Agent component
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
8. In the Hierarchy, navigate to Managers
9. Managers contains three components: Simulation Manager, Agent Controller, and Agent Spawn List
10. Simulation Manager contains simulation variables, including the random seed used, the environmental cells to be instantiated, the size of the simulation environment in cells, and how large the cells are
  * Random Seed: What seed does the random generation use to generate
  * Random Start: Does the program choose a random seed upon starting
  * Environment Cell Pf: The cell prefab the simulation manager populates the environment with
  * Cell Lm: The LayerMask of cells (unused)
  * Environment Extents: The X, Y, and Z size of the simulation environment
  * Cell Size in World: Size of the world
11. Agent Controller: Maintains a list of Agent Spawns (The adjacent Agent Spawn List) and spawns and controls them accordingly
  * Is Simulation Paused: Self-explanatory
  * End Frame: (unused)
12. Agent Spawn List: Contains a list of agents to spawn in order, as well as parameters for their spawning
  * Agent Pf: Prefab containing the agent to be spawned
  * Layer Mask: The layerMask of this agent (unused)
  * Iterations: How many times to spawn this agent before moving to the next
## Screenshots

## Overview
