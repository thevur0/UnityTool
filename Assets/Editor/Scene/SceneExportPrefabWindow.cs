using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
public class SceneExportPrefabWindow : EditorWindow {

    static List<string> m_PathList = new List<string>();

    [MenuItem("GameObject/生成场景Prefab",false,49)]
    static void SceneExportPrefab()
    {
        m_PathList.Clear();
        GameObject curGameObject = Selection.activeGameObject;
        if (curGameObject == null)
            return;

        EditorUtility.DisplayProgressBar("导出环境配置", "正在导出", 0);
        //var EC = GameObject.FindObjectOfType<EnvironmentController>();
        //if (EC != null && EC.Profile != null)
        //{
        //    string srcProfilePath = AssetDatabase.GetAssetPath(EC.Profile);
        //    string destProfilePath = string.Format("Assets/ScenePrefab/{0}.asset", curGameObject.name + "_Environment_Profile");

        //    if (File.Exists(destProfilePath))
        //    {
        //        File.Delete(destProfilePath);
        //    }
        //    File.Copy(srcProfilePath, destProfilePath);
        //}
        EditorUtility.ClearProgressBar();

        string sPrefab = string.Format("Assets/ScenePrefab/{0}.prefab", curGameObject.name);
        GameObject sAsset = PrefabUtility.CreatePrefab(sPrefab,curGameObject);
        
        Transform tran = sAsset.transform;
        string sHierarchyPath = string.Empty;
        EditorUtility.DisplayProgressBar("检查不规范资源", "正在检查...", 0);
        ms_PrefabCount = 0;
        RecursiveTran(tran, sHierarchyPath,true);
        EditorUtility.ClearProgressBar();


        EditorUtility.DisplayProgressBar("查找依赖资源", "正在查找", 0);
        string[] sPaths = AssetDatabase.GetDependencies(sPrefab);

        int i = 0;
        foreach (var sPath in sPaths)
        {
            EditorUtility.DisplayProgressBar("检测依赖资源", string.Format("正在检测..."), (float)i / sPaths.Length);
            //string sPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!sPath.ToLower().Contains("/rawres/") && !sPath.Equals(sPrefab) && !Path.GetExtension(sPath).Equals(".cs"))
            {
                m_PathList.Add(string.Format("{0}-----文件未放到指定目录", sPath));
            }
        }
        EditorUtility.ClearProgressBar();

        if (m_PathList.Count == 0)
        {
            CleanupMissingScripts(sAsset);
            EditorUtility.DisplayDialog("生成成功", string.Format("成功生成{0},内网会产生{1}个Prefab。", sPrefab, ms_PrefabCount),"确定");
        }
        else
        {
            string sMsg = string.Format("共{0}个。\r\n", m_PathList.Count);
            string sBoxMsg = string.Empty;
            int index = 0;
            m_PathList.ForEach((item)=> {
        
                sMsg += item;
                sMsg += "\r\n";
                if (index < 10)
                {
                    sBoxMsg = sMsg;
                }

                index++;
            });

            string sSavePath = Application.dataPath + "/../output.txt";
            StreamWriter sw = new StreamWriter(sSavePath, false);
            sw.Write(sMsg);
            sw.Close();
            sw.Dispose();
            System.Diagnostics.Process.Start("notepad.exe", sSavePath);
            EditorUtility.DisplayDialog("生成失败", sBoxMsg, "确定");
            
            AssetDatabase.DeleteAsset(sPrefab);
        }
    }
    static void CreateFolder(string sFolder)
    {
        if (!AssetDatabase.IsValidFolder(sFolder))
        {
            string[] sDirs = sFolder.Split('/');
            string sParent = sDirs[0];
            for (int i = 1; i < sDirs.Length; i++)
            {
                string sSon = sDirs[i];
                string sDestDir = Path.Combine(sParent, sSon);
                if (!AssetDatabase.IsValidFolder(sDestDir))
                {
                    AssetDatabase.CreateFolder(sParent, sSon);
                }
                sParent = sDestDir;
            }
        }
    }

    static int ms_PrefabCount = 0;
    static void RecursiveTran(Transform tran, string sHierarchyPath,bool bCount)
    {
        sHierarchyPath += "/";
        sHierarchyPath += tran.name;

        if (tran.name.Contains(":") || tran.name.Contains("?") || tran.name.Contains("<") || tran.name.Contains(">") || tran.name.Contains("="))
        {
            m_PathList.Add(sHierarchyPath + "-----命名不正确");
        }
        if(!tran.gameObject.activeSelf)
            m_PathList.Add(sHierarchyPath + "-----被设置为隐藏");

        var script = tran.GetComponent<MonoBehaviour>();
        if (script != null)
        {
            //if(script.GetType()!= typeof(MaterialSound))
                //m_PathList.Add(sHierarchyPath + "-----存在脚本");
        }
        
        if (bCount)
        {
            MeshCollider collider = tran.GetComponent<MeshCollider>();
            if (collider != null)
            {
                MeshFilter parentMeshFilter = tran.parent.GetComponent<MeshFilter>();
                MeshRenderer meshRenderer = tran.GetComponent<MeshRenderer>();
                if (parentMeshFilter == null && meshRenderer == null)
                {
                    m_PathList.Add(sHierarchyPath + "-----MeshCollider要作为MeshFilter子节点。");
                }
                else if (parentMeshFilter == null && meshRenderer != null && meshRenderer.enabled == false)
                {
                    m_PathList.Add(sHierarchyPath + "-----MeshCollider要作为MeshFilter子节点。");
                }
            }

            MeshFilter meshFilter = tran.GetComponent<MeshFilter>();
            LODGroup lODGroup = tran.GetComponent<LODGroup>();
            if (meshFilter != null || lODGroup != null || tran.name.IndexOf("Object_") == 0)
            {
                //if (Mathf.Abs(tran.localScale.x) < 0.1f ||Mathf.Abs(tran.localScale.y) < 0.1f ||Mathf.Abs(tran.localScale.z) < 0.1f)
                    //m_PathList.Add(sHierarchyPath + "-----缩放系数太低");

                ms_PrefabCount++;
                bCount = false;
            }
            else
            {
                bCount = true;
            }
        }

        for (int i = 0; i < tran.childCount; i++)
        {
            RecursiveTran(tran.GetChild(i), sHierarchyPath, bCount);
        }
    }

    static void CleanupMissingScripts(GameObject gameObject)
    {
        // We must use the GetComponents array to actually detect missing components
        var components = gameObject.GetComponents<Component>();

        // Create a serialized object so that we can edit the component list
        var serializedObject = new SerializedObject(gameObject);
        // Find the component list property
        var prop = serializedObject.FindProperty("m_Component");

        // Track how many components we've removed
        int r = 0;

        // Iterate over all components
        for (int j = 0; j < components.Length; j++)
        {
            // Check if the ref is null
            if (components[j] == null)
            {
                // If so, remove from the serialized component array
                prop.DeleteArrayElementAtIndex(j - r);
                // Increment removed count
                r++;
            }
        }

        // Apply our changes to the game object
        serializedObject.ApplyModifiedProperties();
    }
}
