using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class StatModelMat : ScriptableWizard {

    public string m_StatPath = string.Empty;
    void OnWizardCreate()// : 点击确定按钮调用此事件
    {
        string[] files = Directory.GetFiles(m_sAsset,"*.prefab",SearchOption.AllDirectories);
        List<KeyValuePair<string, List<string>>> m_RenderList = new List<KeyValuePair<string, List<string>>>();
        int iIndex = 0;
        foreach (var item in files)
        {
            EditorUtility.DisplayProgressBar("LoadPrefab", item, (float)iIndex/files.Length);
            string sAssetPath = item.Substring(item.IndexOf("Assets\\"));
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(sAssetPath);
            if (prefab != null)
            {
                List<Renderer> listRenderers = new List<Renderer>();
                Renderer[] renderers = prefab.GetComponents<Renderer>();
                listRenderers.AddRange(renderers);
                renderers = prefab.GetComponents<Renderer>();
                listRenderers.AddRange(renderers);
                List<string> warninglist = new List<string>();
                listRenderers.ForEach((renderer) => {
                    HashSet<int> hashmat = new HashSet<int>();
                    Material[] materials = renderer.sharedMaterials;
                    for (int i = 0;i< materials.Length;i++)
                    {
                        if (materials[i] != null)
                        {
                            if (hashmat.Contains(materials[i].GetInstanceID()))
                            {
                                warninglist.Add(renderer.name);
                            }
                            else
                            {
                                hashmat.Add(materials[i].GetInstanceID());
                            }
                        }
                    }
                });
                if(warninglist.Count!=0)
                    m_RenderList.Add(new KeyValuePair<string, List<string>>(sAssetPath, warninglist));
            }
            iIndex++;
        }

        m_RenderList.Sort( (item1,item2)=>
        {
            List<string> warning1 = item1.Value;
            List<string> warning2 = item2.Value;
            int iMax1 = warning1.Count;
            int iMax2 = warning2.Count;
            return iMax2 - iMax1;
        });
        string sSavePath = Path.Combine(Application.dataPath, "Output.txt");
        StreamWriter sw = new StreamWriter(sSavePath, false);
        m_RenderList.ForEach((item) =>
        {
            sw.WriteLine("File:{0}", item.Key);
            item.Value.ForEach((str) => {
                sw.WriteLine("\tName:{0}", str);
            });
            sw.WriteLine("------------------------------------------------------------------------------");
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

    [MenuItem("Tools/StatModelMat", false)]
    static public void Open()
    {
        ScriptableWizard.DisplayWizard<StatModelMat>("StatModelMat", "Stat");
        
    }
}
