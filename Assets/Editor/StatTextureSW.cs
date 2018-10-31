using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class StatTextureSW : ScriptableWizard {

    public string m_StatPath = string.Empty;
    void OnWizardCreate()// : 点击确定按钮调用此事件
    {
        string[] files = Directory.GetFiles(m_sAsset,"*.*",SearchOption.AllDirectories);
        List<KeyValuePair<string, Texture2D>> m_TextureList = new List<KeyValuePair<string, Texture2D>>();
        int iIndex = 0;
        foreach (var item in files)
        {
            EditorUtility.DisplayProgressBar("LoadTexture", item, (float)iIndex/files.Length);
            string sAssetPath = item.Substring(item.IndexOf("Assets\\"));
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(sAssetPath);
            if (texture != null)
            {
                m_TextureList.Add(new KeyValuePair<string, Texture2D>(sAssetPath, texture));
            }
            iIndex++;
        }
        
        m_TextureList.Sort( (item1,item2)=>
        {
            Texture2D tex1 = item1.Value;
            Texture2D tex2 = item2.Value;
            int iMax1 = Mathf.Max(tex1.width,tex1.height);
            int iMax2 = Mathf.Max(tex2.width, tex2.height);
            if (iMax1 != iMax2)
            {
                return iMax2 - iMax1;
            }
            else
            {
                int iMin1 = Mathf.Min(tex1.width, tex1.height);
                int iMin2 = Mathf.Min(tex2.width, tex2.height);
                return iMin2 - iMin1;
            }
        });
        string sSavePath = Path.Combine(Application.dataPath, "Output.txt");
        StreamWriter sw = new StreamWriter(sSavePath, false);
        m_TextureList.ForEach((item) =>
        {
            sw.WriteLine("Texture:{0},Width:{1},Height:{2}.",item.Key,item.Value.width,item.Value.height);
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

    [MenuItem("Tools/StatTextureSW", false)]
    static public void Open()
    {
        ScriptableWizard.DisplayWizard<StatTextureSW>("StatTextureSW", "Stat");
        
    }
}
