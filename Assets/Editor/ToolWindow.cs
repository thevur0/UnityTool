using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GUISytleUtil
{
    static GUISytleUtil()
    {
        m_ButtonStyle = new GUIStyle(GUI.skin.button);
        m_BoxStyle = new GUIStyle(GUI.skin.box);
    }
    static GUIStyle m_ButtonStyle = null;
    static GUIStyle m_BoxStyle = null;
    static public GUIStyle GetButtonStyle() { return m_ButtonStyle; }
    static public GUIStyle GetBoxStyle() { return m_BoxStyle; }
}
public class ToolWindow : UnityEditor.EditorWindow
{
    [MenuItem("Tools/ToolWindow")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow<ToolWindow>("ToolWindow", true, typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow"));
    }

    private void OnGUI()
    {
        ShowAll();
    }
    class MySerialized
    {
        Vector3 vec = Vector3.zero;
        Bounds bou = new Bounds(Vector3.zero, Vector3.one);
    }

    bool m_bInspectorTitlebar = false;
    Rect m_RectArea = new Rect(0,0,100,100);
    void ShowAll()
    {
        //GUI.contentColor = new Color(1, 1, 1, 0.7f);
        
        m_ScrollViewPos = GUILayout.BeginScrollView(m_ScrollViewPos, true, false);
        Button();
        ToolBar();
        Box();
        Lable();
        Bar();
        GUILayout.Space(10);
        SelectionGrid();
        Text();
        Toggle();
        EditorGUILayout.Space();
        BoundsField();
        ColorField();
        CurveField();
        ValueField();
        DelayedValueField();
        DropdownButton();
        Enum();
        HelpBox();
        EditorGUILayout.Separator();
        InspectorTitlebar();
        
        //EditorGUILayout.PropertyField()
        RectField();
        GUILayout.FlexibleSpace();//沉底
        Vector();
        GUILayout.Space(50);
        GUILayout.EndScrollView(); 
    }
    Vector2 m_Vector2 = Vector2.zero;
    Vector2Int m_Vector2Int = Vector2Int.zero;
    Vector3 m_Vector3 = Vector3.zero;
    Vector3Int m_Vector3Int = Vector3Int.zero;
    Vector4 m_Vector4 = Vector4.zero;
    void Vector()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        m_Vector2 = EditorGUILayout.Vector2Field("Vector2Field", m_Vector2);
        m_Vector2Int = EditorGUILayout.Vector2IntField("Vector2IntField", m_Vector2Int);
        m_Vector3 = EditorGUILayout.Vector3Field("Vector3Field", m_Vector3);
        m_Vector3Int = EditorGUILayout.Vector3IntField("Vector3IntField", m_Vector3Int);
        m_Vector4 = EditorGUILayout.Vector4Field("Vector4Field", m_Vector4);
        EditorGUILayout.EndVertical();
    }
    string m_sTag = string.Empty;
    Rect m_Rect = new Rect();
    RectInt m_RectInt = new RectInt();
    void RectField()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        m_Rect = EditorGUILayout.RectField("RectField", m_Rect);
        m_RectInt = EditorGUILayout.RectIntField("RectIntField", m_RectInt);
        EditorGUILayout.EndVertical();
    }
    
    MySerialized m_MySerialized = new MySerialized();
    SerializedProperty m_SerProp = null;
    int m_iMask = 0;
    int m_iLayer = 0;
    float m_fKnob = 0.0f;
    void InspectorTitlebar()
    {
        
        if (Selection.activeGameObject)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GameObject go = Selection.gameObjects[0];
            m_bInspectorTitlebar = EditorGUILayout.InspectorTitlebar(m_bInspectorTitlebar, go);
            if (m_bInspectorTitlebar)
            {
                EditorGUILayout.Vector3Field("Position", go.transform.position);
            }
            EditorGUILayout.EndVertical();
        }
        
    }


    void HelpBox()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.HelpBox("HelpBox None", MessageType.None);
        EditorGUILayout.HelpBox("HelpBox Info", MessageType.Info);
        EditorGUILayout.HelpBox("HelpBox Warning", MessageType.Warning);
        EditorGUILayout.HelpBox("HelpBox Error", MessageType.Error);
        EditorGUILayout.EndVertical();
    }

    EnumFlagsType m_EnumFlagsField;
    EnumFlagsType m_EnumFlagsPopup;
    enum EnumFlagsType
    {
        FlagsType1,
        FlagsType2,
    }
    void Enum()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        m_EnumFlagsField = (EnumFlagsType)EditorGUILayout.EnumFlagsField("EnumFlagsField", m_EnumFlagsField);
        m_EnumFlagsPopup = (EnumFlagsType)EditorGUILayout.EnumPopup("EnumPopup", m_EnumFlagsPopup);
        m_iIntPopup = EditorGUILayout.IntPopup("IntPopup", m_iIntPopup, new string[] { "text1", "text2" }, new int[] { 1, 2 });
        m_iLayer = EditorGUILayout.LayerField("LayerField", m_iLayer);
        m_iMask = EditorGUILayout.MaskField("MaskField", m_iMask, new String[] { "Mask1", "Mask2", "Mask3" });
        m_iIntPopup = EditorGUILayout.Popup("Popup", m_iIntPopup, new string[] { "text1", "text2" });
        m_sTag = EditorGUILayout.TagField("TagField", m_sTag);
        EditorGUILayout.EndVertical();
    }
    void DropdownButton()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.DropdownButton(new GUIContent("DropdownButton Keyboard", "DropdownButton Tips"), FocusType.Keyboard);
        EditorGUILayout.DropdownButton(new GUIContent("DropdownButton Passive", "DropdownButton Tips"), FocusType.Passive);
        EditorGUILayout.EndVertical();
    }
    bool m_bFoldOut = false;
    bool m_bFoldOutDelayed = false;
    float m_fFloat = 0.0f;
    double m_fDouble = 0.0f;
    int m_iInt = 0;
    int m_iIntPopup = 0;
    int m_iIntSlider = 0;
    float m_fFloatSlider = 0;
    string m_sString = string.Empty;
    float m_fSliderMinValue = 2.0f;
    float m_fSliderMaxValue = 8.0f;
    float m_fSliderMin = 0.0f;
    float m_fSliderMax = 10.0f;
    long m_iLong = 0;
    void ValueField()
    {
        EditorGUILayout.BeginVertical(GUI.skin.button);
        m_bFoldOut = EditorGUILayout.Foldout(m_bFoldOut, "Foldout", true);
        if (m_bFoldOut)
        {
            m_fDouble = EditorGUILayout.DoubleField("DoubleField", m_fDouble);
            m_fFloat = EditorGUILayout.FloatField("FloatField", m_fFloat);
            m_iInt = EditorGUILayout.IntField("IntField", m_iInt);
            m_iLong = EditorGUILayout.LongField("LongField", m_iLong);
            m_sString = EditorGUILayout.TextField("TextField", m_sString);
            
        }
        EditorGUILayout.EndVertical();
    }
    Color m_SaveColor = Color.white;
    private void BeginBgColor(Color color)
    {
        m_SaveColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
    }
    private void EndBgColor()
    {
        GUI.backgroundColor = m_SaveColor;
    }

    float m_fDelayedFloat = 0.0f;
    double m_fDelayedDouble = 0.0f;
    int m_iDelayedInt = 0;
    string m_sDelayedString = string.Empty;
    void DelayedValueField()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        m_bFoldOutDelayed = EditorGUILayout.Foldout(m_bFoldOutDelayed, "Foldout", true);
        if (m_bFoldOutDelayed)
        {
            m_fDelayedDouble = EditorGUILayout.DelayedDoubleField("DelayedDoubleField", m_fDelayedDouble);
            m_fDelayedFloat = EditorGUILayout.DelayedFloatField("DelayedFloatField", m_fDelayedFloat);
            m_iDelayedInt = EditorGUILayout.DelayedIntField("DelayedIntField", m_iDelayedInt);
            m_sDelayedString = EditorGUILayout.DelayedTextField("DelayedTextField", m_sDelayedString);
        }
        EditorGUILayout.EndVertical();
    }
    AnimationCurve m_AnimationCurve = new AnimationCurve();
    void CurveField()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        m_AnimationCurve = EditorGUILayout.CurveField("CurveField", m_AnimationCurve);
        EditorGUILayout.EndVertical();
    }
    Color m_Color = new Color();
    void ColorField()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        m_Color = EditorGUILayout.ColorField("ColorField", m_Color);
        EditorGUILayout.EndVertical();
    }
    Bounds m_Bounds = new Bounds();
    BoundsInt m_BoundsInt = new BoundsInt();
    void BoundsField()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        m_Bounds = EditorGUILayout.BoundsField("BoundsField", m_Bounds);
        m_BoundsInt = EditorGUILayout.BoundsIntField("BoundsIntField", m_BoundsInt);
        EditorGUILayout.EndVertical();
    }
    bool m_Toggle = false;
    bool m_TogleGroup = false;
    void Toggle()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        m_Toggle = GUILayout.Toggle(m_Toggle, "Toggle");
        m_TogleGroup = EditorGUILayout.BeginToggleGroup("ToggleGroup", m_TogleGroup);
        m_Toggle = EditorGUILayout.Toggle("Toggle",m_Toggle);
        m_Toggle = EditorGUILayout.ToggleLeft("ToggleLeft", m_Toggle);
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.EndVertical();
    }

    string m_PasswordField = "PasswordField";
    string m_TextArea = "TextArea\nTextArea";
    string m_TextField = "TextField";
    void Text()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        m_TextArea = GUILayout.TextArea(m_TextArea, GUILayout.Height(50));
        m_TextField = GUILayout.TextField(m_TextField);
        m_TextField = GUILayout.TextField(m_TextField, GUILayout.Height(50));
        m_PasswordField = GUILayout.PasswordField(m_PasswordField, '*');
        m_PasswordField = EditorGUILayout.PasswordField("PasswordField",m_PasswordField);

        m_TextArea = EditorGUILayout.TextArea(m_TextArea, GUILayout.Height(50));
        m_TextField = EditorGUILayout.TextField("TextField", m_TextField);
        EditorGUILayout.EndVertical();
    }
    int m_iSelGrid = 0;
    void SelectionGrid()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        m_iSelGrid = GUILayout.SelectionGrid(m_iSelGrid, new string[] { "SelectionGrid1", "SelectionGrid2", "SelectionGrid3", "SelectionGrid4", "SelectionGrid5", "SelectionGrid6" }, 4);
        EditorGUILayout.EndVertical();
    }
    float m_fHScrollBar = 0;
    float m_fVScrollBar = 0;
    float m_fHSlider = 0;
    float m_fVSlider = 0;
    void Bar()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        m_fHScrollBar = GUILayout.HorizontalScrollbar(m_fHScrollBar, 0.2f, 0, 1);
        m_fVScrollBar = GUILayout.VerticalScrollbar(m_fVScrollBar, 0.2f, 0, 1);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        m_fHSlider = GUILayout.HorizontalSlider(m_fHSlider, 0, 1);
        m_fVSlider = GUILayout.VerticalSlider(m_fVSlider, 0, 1, GUILayout.Height(20));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        m_fFloatSlider = EditorGUILayout.Slider("Slider", m_fFloatSlider,0,100);
        m_iIntSlider = EditorGUILayout.IntSlider("IntSlider", m_iIntSlider, 0, 10);
        EditorGUILayout.MinMaxSlider("MinMaxSlider", ref m_fSliderMinValue, ref m_fSliderMaxValue, m_fSliderMin, m_fSliderMax);
        m_fKnob = EditorGUILayout.Knob(Vector2.one * 100, m_fKnob, 0, 1, "Knob", Color.black, Color.red, true);
        EditorGUILayout.EndVertical();
    }
    void Lable()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.BeginVertical();
        
        GUIStyle stylefont = new GUIStyle(GUI.skin.label);
        stylefont.fontStyle = FontStyle.Bold;
        stylefont.alignment = TextAnchor.MiddleLeft;
        GUILayout.Label("Label1", stylefont);
        stylefont.fontStyle = FontStyle.Italic;
        GUILayout.Label("Label2", stylefont);
        stylefont.fontStyle = FontStyle.BoldAndItalic;
        GUILayout.Label("Label3", stylefont);
        EditorGUILayout.EndVertical();

        GUIStyle styleAnchor = new GUIStyle(GUI.skin.label);
        EditorGUILayout.BeginVertical();
        styleAnchor.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Label4", styleAnchor);
        GUILayout.Label("Label5", styleAnchor);
        GUILayout.Label("Label6", styleAnchor);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        styleAnchor.alignment = TextAnchor.MiddleRight;
        GUILayout.Label("Label7", styleAnchor);
        GUILayout.Label("Label8", styleAnchor);
        GUILayout.Label("Label9", styleAnchor);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("LabelField");
        EditorGUILayout.PrefixLabel("PrefixLabel");
        EditorGUILayout.SelectableLabel("SelectableLabel");
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }
    Vector2 m_ScrollViewPos = new Vector2();
    void Box()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("Box");
        GUILayout.Box("Box", GUILayout.Width(100));
        GUILayout.Box("Box", GUILayout.Height(30));
        GUILayout.Box("Box");
        GUILayout.Box("Box", GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();
        GUILayout.Box("Box", GUILayout.ExpandWidth(true));
        EditorGUILayout.EndVertical();
    }
    void Button()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Button("Button1");
        GUILayout.Button("Button2");
        GUILayout.Button("Button3");
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Button("Button1");
        GUILayout.Button("Button2");
        GUILayout.Button("Button3");
        
        GUILayout.RepeatButton("RepeatButton");
        EditorGUILayout.EndVertical();
    }
    int m_iToolbarSel = 0;
    void ToolBar()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        m_iToolbarSel = GUILayout.Toolbar(m_iToolbarSel, new string[] { "ToolBar1", "ToolBar2", "ToolBar3" });
        EditorGUILayout.EndVertical();
    }
}

