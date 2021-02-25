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


        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif