using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReplacePrefab : ScriptableWizard {

    public GameObject[] hierarchyObjects;
    public GameObject prefabObject;
    // Use this for initialization


    void OnWizardUpdate () {
        
        hierarchyObjects = Selection.gameObjects;
        
        isValid = !(hierarchyObjects == null || prefabObject == null || hierarchyObjects.Length == 0);

    }
	
	// Update is called once per frame
	void OnWizardCreate () {

        foreach (var obj in hierarchyObjects)
        {
            Vector3 pos = obj.transform.position;
            Quaternion quaternion = obj.transform.rotation;
            Vector3 scale = obj.transform.localScale;
            GameObject newObject = PrefabUtility.ConnectGameObjectToPrefab(obj, prefabObject);
            newObject.transform.SetPositionAndRotation(pos, quaternion);
            newObject.transform.localScale = scale;
        }

    }
    [MenuItem("Tools/替换Prefab", false, 49)]
    static void Replace()
    {
        ScriptableWizard.DisplayWizard("ReplacePrefab", typeof(ReplacePrefab), "Replace");
    }
}
