using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ScalePNG : ScriptableWizard {

    public Texture2D m_Texture = null;
    void OnWizardCreate()// : 点击确定按钮调用此事件
    {
        ExportTo();
        AssetDatabase.Refresh();
    }

    void ExportTo()
    {
        Texture2D texture = m_Texture;
        int iWidth = texture.width*2;
        int iHeight = texture.height*2;
        Texture2D texture2D = new Texture2D(iWidth, iHeight, TextureFormat.RGBA32, false);
        for (int i = 0;i< texture.width; i++)
        {
            for (int j= 0;j< texture.height; j++)
            {
                Color color = texture.GetPixel(i, j);
                texture2D.SetPixel(i*2,j*2,color);
                texture2D.SetPixel(i * 2, j * 2+1, color);
                texture2D.SetPixel(i * 2+1, j * 2, color);
                texture2D.SetPixel(i * 2+1, j * 2+1, color);
            }
        }
        texture2D.Apply();
        string sAssetPath = AssetDatabase.GetAssetPath(texture.GetInstanceID());
        sAssetPath = sAssetPath.Substring(0, sAssetPath.IndexOf('.'));
        sAssetPath = Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets")+1) + sAssetPath;
        byte[] bytes = texture2D.EncodeToPNG();
        string sFileName = string.Format("{0}_Scale2.png", sAssetPath);
        File.WriteAllBytes(sFileName, bytes);
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

    [MenuItem("Tools/放大成PNG", false)]
    static public void Open()
    {
        ScriptableWizard.DisplayWizard<ScalePNG>("缩放成PNG", "缩放成PNG");
    }
}
