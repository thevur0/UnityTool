using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ExportPNG : ScriptableWizard {

    public Texture2D m_Texture = null;
    public int m_CutRow = 1;
    public int m_CutCol = 1;
    void OnWizardCreate()// : 点击确定按钮调用此事件
    {
        
        for (int i = 0;i< m_CutRow* m_CutCol;i++ )
        {
            ExportTo(i/m_CutRow,i% m_CutCol);
        }
        AssetDatabase.Refresh();
    }

    void ExportTo(int iRow,int iCol)
    {
        Texture2D texture = m_Texture;
        int iWidth = texture.width/ m_CutRow;
        int iHeight = texture.height/ m_CutCol;
        Texture2D texture2D = new Texture2D(iWidth, iHeight, TextureFormat.RGBA32, false);
        for (int i = 0;i< iWidth;i++)
        {
            for (int j= 0;j< iHeight;j++)
            {
                Color color = texture.GetPixel(i+iCol* iWidth, j+ iRow*iHeight);
                texture2D.SetPixel(i,j,color);
            }
        }
        texture2D.Apply();
        string sAssetPath = AssetDatabase.GetAssetPath(texture.GetInstanceID());
        sAssetPath = sAssetPath.Substring(0, sAssetPath.IndexOf('.'));
        sAssetPath = Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")+1) + sAssetPath;
        byte[] bytes = texture2D.EncodeToPNG();
        string sFileName = string.Format("{0}_{1}_{2}.png", sAssetPath, iRow,iCol);
        File.WriteAllBytes(sFileName, bytes);
    }
    protected override bool DrawWizardGUI()
    {
        EditorGUILayout.BeginHorizontal();
        Texture2D newTex = EditorGUILayout.ObjectField(m_Texture, typeof(Texture2D), false, GUILayout.Width(72), GUILayout.Height(72)) as Texture2D;
        if (newTex != m_Texture)
            m_Texture = newTex;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal(GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        m_CutRow = EditorGUILayout.IntField("切割行数",m_CutRow);
        m_CutCol = EditorGUILayout.IntField("切割列数",m_CutCol);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        return true;
    }
    void OnWizardUpdate()// : 当编辑器向导更新时调用
    {
        if (m_Texture!=null)
        {
            string sAssetPath = AssetDatabase.GetAssetPath(m_Texture.GetInstanceID());
            TextureImporter textureImporter = AssetImporter.GetAtPath(sAssetPath) as TextureImporter;
            if (textureImporter.isReadable == false)
            {
                isValid = false;
                errorString = "请设置图片Read/Write Enable.";
            }
            else if (!m_Texture.format.Equals(TextureFormat.RGBA32))
            {
                isValid = false;
                errorString = "请设置图片RGBA32格式";
            }
            else if (m_CutCol < 1 && m_CutRow < 1)
            {
                isValid = false;
            }
            else
            {
                errorString = "";
                isValid = true;
            }
            
        }
        else
        {
            isValid = false;
        }
    }

    [MenuItem("Tools/导出PNG", false)]
    static public void Open()
    {
        ScriptableWizard.DisplayWizard<ExportPNG>("导出PNG", "导出PNG");
    }
}
