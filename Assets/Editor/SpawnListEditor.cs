using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnList))]
[CanEditMultipleObjects]
public class SpawnListEditor : Editor
{
    void OnEnable() {

    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
    }
}