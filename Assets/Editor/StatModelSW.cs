using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class StatModelSW : ScriptableWizard {

    public string m_StatPath = string.Empty;
    void OnWizardCreate()// : 点击确定按钮调用此事件
    {
        string[] files = Directory.GetFiles(m_sAsset,"*.*",SearchOption.AllDirectories);
        List<KeyValuePair<string, Mesh>> m_MeshList = new List<KeyValuePair<string, Mesh>>();
        int iIndex = 0;
        foreach (var item in files)
        {
            EditorUtility.DisplayProgressBar("LoadModel", item, (float)iIndex/files.Length);
            string sAssetPath = item.Substring(item.IndexOf("Assets\\"));
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(sAssetPath);
            if (mesh != null)
            {
                m_MeshList.Add(new KeyValuePair<string, Mesh>(sAssetPath, mesh));
            }
            iIndex++;
        }

        m_MeshList.Sort( (item1,item2)=>
        {
            Mesh mesh1 = item1.Value;
            Mesh mesh2 = item2.Value;
            int iMax1 = mesh1.triangles.Length;
            int iMax2 = mesh2.triangles.Length;
            return iMax2 - iMax1;
        });
        string sSavePath = Path.Combine(Application.dataPath, "Output.txt");
        StreamWriter sw = new StreamWriter(sSavePath, false);
        m_MeshList.ForEach((item) =>
        {
            sw.WriteLine("Mesh:{0},Triangles:{1}.", item.Key,item.Value.triangles.Length/3);
        });
        sw.Close();
        sw.Dispose();
        System.Diagnostics.Process.Start("notepad.exe", sSavePath);
    }

    string m_sAsset = string.Empty;
    void OnWizardUpdate()// : 当编辑器向导更新时调用
    {
        m_sAsset = Path.Combine(Application.dataPath, m_StatPath);
        helpString = m_sAsset;
        if (Directory.Exists(m_sAsset))
        {
            isValid = true;
        }
        else
        {
            isValid = false;
        }
    }

    [MenuItem("Tools/StatMeshSW", false)]
    static public void Open()
    {
        ScriptableWizard.DisplayWizard<StatModelSW>("StatMeshSW", "Stat");
        
    }
}
