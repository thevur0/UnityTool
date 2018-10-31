using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
public class SceneStatic : EditorWindow {
    

    [MenuItem("GameObject/BakeTool/Lightmap Static", false,49)]
    static void LightmapStatic()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        foreach (var obj in gameObjects)
        {
            RecursiveTran(obj.transform, SetLightStatic);
        }
    }
    [MenuItem("GameObject/BakeTool/UnStatic", false, 49)]
    static void UnStatic()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        foreach (var obj in gameObjects)
        {
            obj.isStatic = false;
        }
    }

    [MenuItem("GameObject/BakeTool/Light放大100倍", false, 49)]
    static void LightZoonIn()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        foreach (var obj in gameObjects)
        {
            RecursiveTran(obj.transform, SetLightZoonIn);
        }
    }
    [MenuItem("GameObject/BakeTool/Light缩小100倍", false, 49)]
    static void LightZoonOut()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        foreach (var obj in gameObjects)
        {
            RecursiveTran(obj.transform, SetLightZoonOut);
        }
    }

    static void SetLightZoonIn(GameObject gameObject)
    {
        Light light = gameObject.GetComponent<Light>();
        if (light != null)
        {
            light.range = light.range * 100.0f;
        }
    }

    static void SetLightZoonOut(GameObject gameObject)
    {
        Light light = gameObject.GetComponent<Light>();
        if (light != null)
        {
            light.range = light.range * 0.01f;
        }
    }

    static void SetLightStatic(GameObject gameObject)
    {
        GameObjectUtility.SetStaticEditorFlags(gameObject, StaticEditorFlags.LightmapStatic | StaticEditorFlags.ReflectionProbeStatic);

    }
    static void RecursiveTran(Transform tran,Action<GameObject> action)
    {
        action(tran.gameObject);
        for (int i = 0; i < tran.childCount; i++)
        {
            RecursiveTran(tran.GetChild(i), action);
        }
    }


}
