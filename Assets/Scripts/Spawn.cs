using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Spawn : MonoBehaviour {
    public GameObject agentPf;
    public LayerMask layerMask;
    [Range(1, 100)]
    public int  iterations;

    [Range(0, 100)]
    public int step;
    public bool wait;

    public SpawnTypes spawnType;

    public Vector3Int point;

    public enum SpawnTypes {
        Random,
        Last,
        Point
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Spawn))]
[CanEditMultipleObjects]
public class SpawnEditor : Editor {

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();
        Spawn spawn = (Spawn)target;
        if (spawn == null) return;
        
        spawn.agentPf = (GameObject)EditorGUILayout.ObjectField("Agent Prefab", spawn.agentPf, typeof(GameObject), false);
        spawn.layerMask = EditorGUILayout.LayerField("Layer Mask", spawn.layerMask);
        spawn.iterations = EditorGUILayout.IntSlider("Iterations", spawn.iterations, 0, 100);
        spawn.step = EditorGUILayout.IntSlider("Step", spawn.step, 1, 100);
        spawn.wait = EditorGUILayout.Toggle("Wait", spawn.wait);
        spawn.spawnType = (Spawn.SpawnTypes)EditorGUILayout.EnumPopup("Spawn Type", spawn.spawnType);
        if (spawn.spawnType != Spawn.SpawnTypes.Random) spawn.point = EditorGUILayout.Vector3IntField("Spawn Point", spawn.point);

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif