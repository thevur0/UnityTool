using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreatePrefab : ScriptableWizard {


    public GameObject hierarchyObject;
    public string savePath = "Assets/ScenePrefab/DanZhou";
    // Use this for initialization


    void OnWizardUpdate () {
        
        isValid = !(hierarchyObject == null || !AssetDatabase.IsValidFolder(savePath));

    }
	
	// Update is called once per frame
	void OnWizardCreate () {

        listPrefab.Clear();
        WalkTranform(hierarchyObject.transform);
        DealWith();
    }

    List<Transform> listPrefab = new List<Transform>();
    void WalkTranform(Transform tran)
    {
        if (!tran.gameObject.activeInHierarchy)
            return;

        if (IsNeedCreatePrefab(tran))
        {
            listPrefab.Add(tran);
        }
        else
        {
            for (int i = 0; i < tran.childCount; i++)
            {
                WalkTranform(tran.GetChild(i));
            }
        }
    }

    void DealWith()
    {
        for(int i = 0;i< listPrefab.Count;i++)
        {
            Transform item = listPrefab[i];
            string sTitle = string.Format( "CreatePrefab({0}/{1})",i, listPrefab.Count);
            EditorUtility.DisplayProgressBar(sTitle,item.name,(float)i/listPrefab.Count);
            item.name = TransName(item.name);
            string sPrefabPath = string.Format("{0}/{1}.prefab", savePath, item.name);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(sPrefabPath);

            var pos = item.position;
            var rot = item.rotation;
            var scale = item.localScale;

            if (prefab == null)
            {
                prefab = PrefabUtility.CreatePrefab(sPrefabPath, item.gameObject);
                prefab.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                prefab.transform.localScale = Vector3.one;
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        for (int i = 0; i < listPrefab.Count; i++)
        {
            Transform item = listPrefab[i];
            string sTitle = string.Format("ConnectGameObject({0}/{1})", i, listPrefab.Count);
            EditorUtility.DisplayProgressBar(sTitle, item.name, (float)i / listPrefab.Count);
            item.name = TransName(item.name);
            string sPrefabPath = string.Format("{0}/{1}.prefab", savePath, item.name);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(sPrefabPath);

            var pos = item.position;
            var rot = item.rotation;
            var scale = item.localScale;

            if (prefab != null)
            {
                GameObject go = PrefabUtility.ConnectGameObjectToPrefab(item.gameObject, prefab);
                go.transform.SetPositionAndRotation(pos, rot);
                go.transform.localScale = scale;
            }
        }
    }

    bool IsNeedCreatePrefab(Transform tran)
    {
        if (tran.name.IndexOf("Object_") == 0)
            return true;

        if (tran.GetComponent<Animator>() || tran.GetComponent<LODGroup>() || tran.GetComponent<MeshFilter>())
            return true;

        return false;
    }
    
    string TransName(string sName)
    {
        string resName = sName.Replace(":",string.Empty).Replace("(Clone)",string.Empty);
        int iPos = resName.IndexOf(" (");
        if (iPos > 0)
            resName = resName.Substring(0, iPos);
        return resName;
    }

    [MenuItem("Tools/生成Prefab", false, 49)]
    static void Replace()
    {
        ScriptableWizard.DisplayWizard("CreatePrefab", typeof(CreatePrefab), "Create");
    }


    [MenuItem("GameObject/SceneTool/解除Prefab关联", false, 49)]
    static void LightmapStatic()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        foreach (var obj in gameObjects)
        {
            PrefabUtility.DisconnectPrefabInstance(obj);
        }
    }
}
