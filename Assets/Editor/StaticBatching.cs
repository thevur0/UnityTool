using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StaticBatching {

    
    class BatchData
    {
        public BatchData(Material mat, int iLightMapIndex)
        {
            material = mat;
            LightMapID = iLightMapIndex;
        }

        static public string GetKey(Material mat, int iLightMapIndex)
        {
            return string.Format("{0}:{1}", mat.GetInstanceID(), iLightMapIndex);
        }
        public string Key { get { return GetKey(material,LightMapID); } }
        public Material material;
        private int LightMapID = 0;
        List<KeyValuePair<MeshFilter, int>> meshlist = new List<KeyValuePair<MeshFilter, int>>();

        public void AddMesh(MeshFilter meshFilter,int iSubMesh)
        {
            meshlist.Add(new KeyValuePair<MeshFilter, int>(meshFilter, iSubMesh));
        }

        public void Batching()
        {
            if (meshlist.Count == 0)
                return;

            var list = meshlist;
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            Vector3 pos = Vector3.zero;
            MeshFilter meshFilter;
            list.ForEach((item) =>
            {
                meshFilter = item.Key;
                pos += meshFilter.transform.position;
            });
            pos /= list.Count;

            list.ForEach((item) =>
            {
                meshFilter = item.Key;
                CombineInstance ci = new CombineInstance();
                ci.mesh = meshFilter.sharedMesh;
                ci.transform = Matrix4x4.Translate(meshFilter.transform.position) * Matrix4x4.Translate(-pos) * Matrix4x4.Rotate(meshFilter.transform.rotation) * Matrix4x4.Scale(meshFilter.transform.localScale);

                //ci.transform = meshFilter.transform.localToWorldMatrix;// *( Matrix4x4.Translate(-pos) * Matrix4x4.Rotate(meshFilter.transform.rotation));
                
                ci.subMeshIndex = item.Value;
                combineInstances.Add(ci);
                
            });
            
            GameObject gameObject = new GameObject("New");
            gameObject.transform.position = pos;
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combineInstances.ToArray());
            mesh.name = material.name;
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;
            AssetDatabase.CreateAsset(mesh, string.Format("Assets/{0}.mesh",mesh.name));
        }
    }
    [MenuItem("Tools/StaticBatching",false)]
    static void DoStaticBatching()
    {
        Dictionary<string, BatchData> CombineMap = new Dictionary<string, BatchData>();
        Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();
        for (int i = 0;i<renderers.Length;i++)
        {
            Renderer renderer = renderers[i];
            if (GameObjectUtility.AreStaticEditorFlagsSet(renderer.gameObject,StaticEditorFlags.BatchingStatic))
            {
                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Material[] materials = renderer.sharedMaterials;
                    Mesh mesh = meshFilter.sharedMesh;

                    for(int j = 0;j< materials.Length; j++)
                    {
                        var key = BatchData.GetKey(materials[j],renderer.lightmapIndex);
                        BatchData batchData;
                        if (!CombineMap.TryGetValue(key, out batchData))
                        {
                            batchData = new BatchData(materials[j], renderer.lightmapIndex);
                            CombineMap.Add(key, batchData);
                        }
                        batchData.AddMesh(meshFilter,j);
                    }
                }
            }
        }
        var itor = CombineMap.GetEnumerator();
        while (itor.MoveNext())
        {
            var batchData = itor.Current.Value;
            batchData.Batching();
        }

    }
}
