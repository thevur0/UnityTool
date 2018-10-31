using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
namespace AssetBundleUntar
{
    public class AssetBundleUntarWindow : UnityEditor.EditorWindow
    {
        [MenuItem("Tools/AB Untar Texture")]
        static void ShowWindow()
        {
            Type t = typeof(EditorWindow).Assembly.GetType("UnityEditor.Toolbar");
            EditorWindow.GetWindow<AssetBundleUntarWindow>("AB Untar Texture", true, typeof(EditorWindow).Assembly.GetType("UnityEditor.MainView"));
        }
        string m_OpenPath;
        string m_SavePath;
        List<string> m_ListABName = new List<string>();
        HashSet<string> m_SetSelABName = new HashSet<string>();
        bool m_bSelect;
        Vector2 m_ScrollPos = new Vector2();
        GUIStyle m_LabelStyle = GUIStyle.none;  
        private void Awake()
        {
            //m_LabelStyle.border = new RectOffset(1,1,1,1);
            //m_LabelStyle.margin = new RectOffset(5, 5, 5, 5);
            //m_LabelStyle.padding = new RectOffset(5, 5, 5, 5);
            //m_LabelStyle.alignment = TextAnchor.MiddleLeft;
        }

        private void OnGUI()
        {
            if (m_LabelStyle == GUIStyle.none)
            {
                m_LabelStyle = new GUIStyle(GUI.skin.box);
                m_LabelStyle.alignment = TextAnchor.MiddleLeft;
            }
            GUILayout.BeginVertical();
            GUILayout.Label("Open Path", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_OpenPath, m_LabelStyle, GUILayout.ExpandWidth(true));
            
            if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
            {
                OnOpenBrowse();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Save Path", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( m_SavePath, m_LabelStyle,GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
            {
                OnSaveBrowse();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("_________________________________________________________________________________________________________________________________________________________________________");
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Inverse"))
            {
                OnInverse();
            }
            if (GUILayout.Button("Select All"))
            {
                OnSelectAll();
            }
            EditorGUILayout.EndHorizontal();
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            for (int i = 0;i< m_ListABName.Count;i++)
            {
                EditorGUILayout.BeginHorizontal();
                string sABName = TranABName(m_ListABName[i]);
                bool bSel = m_SetSelABName.Contains(sABName);
                if (GUILayout.Toggle(bSel, sABName))
                {
                    m_SetSelABName.Add(sABName);
                }
                else
                {
                    m_SetSelABName.Remove(sABName);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Untar"))
            {
                OnUntar();
            }
            GUILayout.EndVertical();
        }
        void RefreshFileList()
        {
            string[] sFiles = System.IO.Directory.GetFiles(m_OpenPath,"*.*", System.IO.SearchOption.AllDirectories);
            m_ListABName.Clear();
            for (int i = 0;i<sFiles.Length;i++)
            {
                string sExt = System.IO.Path.GetExtension(sFiles[i]);
                if (!sExt.ToLower().Equals(".meta") && !sExt.ToLower().Equals(".manifest"))
                {
                    m_ListABName.Add(sFiles[i]);
                }
            }
        }
        void OnInverse()
        {
            HashSet<string> hashSet = new HashSet<string>(m_SetSelABName);
            m_SetSelABName.Clear();
            for (int i = 0; i < m_ListABName.Count; i++)
            {
                string sABName = TranABName(m_ListABName[i]);
                if (!hashSet.Contains(sABName))
                {
                    m_SetSelABName.Add(sABName);
                }
            }
        }
        void OnSelectAll()
        {
            m_SetSelABName.Clear();
            for (int i = 0;i<m_ListABName.Count;i++)
            {
                m_SetSelABName.Add(TranABName(m_ListABName[i]));
            }
        }
        string TranABName(string sPath)
        {
            try
            { 
                return sPath.Substring(m_OpenPath.Length + 1);
            }
            catch(System.Exception ex)
            {
                Debug.LogError(sPath);
                return string.Empty;
            }
        }
        void OnOpenBrowse()
        {
            string sPath = EditorUtility.SaveFolderPanel("Path to Open Path", m_OpenPath, "");   //打开保存文件夹面板
            if (!string.IsNullOrEmpty(sPath))
            {
                m_OpenPath = sPath;
                RefreshFileList();

            }
        }
        void OnSaveBrowse()
        {
            string sPath = EditorUtility.SaveFolderPanel("Path to Save Images", m_SavePath, "");   //打开保存文件夹面板
            if (!string.IsNullOrEmpty(sPath))
            {
                m_SavePath = sPath;
            }
        }
        void OnUntar()
        {
            if (string.IsNullOrEmpty(m_OpenPath))
            {
                OnSaveBrowse();
                return;
            }
            if (string.IsNullOrEmpty(m_SavePath))
            {
                OnSaveBrowse();
                return;
            }
            EditorUtility.DisplayProgressBar("Untar","please wait...", 0);
            string sPath = string.Empty;
            for (int i = 0;i<m_ListABName.Count;i++)
            {
                sPath = m_ListABName[i];
                string sABName = TranABName(sPath);
                if (m_SetSelABName.Contains(sABName))
                {
                    EditorUtility.DisplayProgressBar("Untar", "Untar "+sABName+"...", 0);
                    AssetBundle ab = AssetBundle.LoadFromFile(sPath);
                    if (ab != null && !ab.isStreamedSceneAssetBundle)
                    {
                        try
                        {
                            Texture2D[] texAlls = ab.LoadAllAssets<Texture2D>();
                            string sFolder = Path.GetFileNameWithoutExtension(ab.name);
                            for (int j = 0; j < texAlls.Length; j++)
                            {
                                Texture2D tex = texAlls[j];
                                EditorUtility.DisplayProgressBar("Untar", "Save " + tex.name + "...", (float)j / texAlls.Length);
                                SaveTexture(tex, sFolder);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError(ex.Message);
                        }
                        finally
                        {
                            ab.Unload(true);
                        }
                    }
                }
                
            }
            EditorUtility.ClearProgressBar();
        }
        //void SaveTexture(Texture2D tex)
        //{
        //    Texture2D texSave = new Texture2D((int)tex.width, (int)(tex.height), TextureFormat.RGBA32, false);
        //    for (int i = 0;i<tex.width;i++)
        //    {
        //        for (int j = 0;j<tex.height;j++)
        //        {
        //            Color color = tex.GetPixel(i, j);
        //            texSave.SetPixel(i, j, color);
        //        }
        //    }
        //    texSave.Apply();
        //    System.IO.File.WriteAllBytes(m_SavePath + "/" + tex.name, texSave.EncodeToPNG());
        //}

        void SaveTexture(Texture2D tex,string sFolder)
        {
            Shader shader = Shader.Find("UI/Default");
            if (shader != null)
            {
                SaveRenderTextureToPNG(tex, shader, m_SavePath+"/"+ sFolder, tex.name);
            }
        }

        public bool SaveRenderTextureToPNG(Texture inputTex, Shader outputShader, string contents, string pngName)
        {
            //RenderTexture temp = RenderTexture.GetTemporary(inputTex.width, inputTex.height, 0, RenderTextureFormat.ARGB32);
            RenderTexture temp = new RenderTexture(inputTex.width, inputTex.height, 0, RenderTextureFormat.ARGB32);
            Material mat = new Material(outputShader);
            Graphics.Blit(inputTex, temp, mat);
            bool ret = SaveRenderTextureToPNG(temp, contents, pngName);
            //RenderTexture.ReleaseTemporary(temp);
            return ret;

        }

        //将RenderTexture保存成一张png图片  
        public bool SaveRenderTextureToPNG(RenderTexture rt, string contents, string pngName)
        {
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            byte[] bytes = png.EncodeToPNG();
            if (!Directory.Exists(contents))
                Directory.CreateDirectory(contents);
            FileStream file = File.Open(contents + "/" + pngName + ".png", FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(bytes);
            file.Close();
            Texture2D.DestroyImmediate(png);
            png = null;
            RenderTexture.active = prev;
            return true;
        }
    }
}