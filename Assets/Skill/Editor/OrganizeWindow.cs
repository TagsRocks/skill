using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Skill.Framework.UI;


public class OrganizeWindow : EditorWindow
{
    #region EditorWindow
    private static OrganizeWindow _Instance;
    public static OrganizeWindow Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = ScriptableObject.CreateInstance<OrganizeWindow>();
            return _Instance;
        }
    }

    private static Vector2 Size = new Vector2(320, 140);

    public OrganizeWindow()
    {
        hideFlags = HideFlags.DontSave;

        base.titleContent = new GUIContent() { text = "Organize" };
        base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
        base.minSize = Size;
        CreateUI();
    }

    void OnFocus()
    {
        if (_Frame != null)
        {
            _RefreshStyles = true;
        }
    }

    #endregion

    #region Serialized Variables

    /// <summary> unit length in world space. the space containing meshes seperate to unit cubes then mehses inside each square will be merged </summary>
    [SerializeField]
    private Vector3 _UnitLength = new Vector3(1, 0, 0);

    #endregion

    #region UI
    private bool _RefreshStyles = true;
    private Skill.Editor.UI.EditorFrame _Frame;
    private Skill.Framework.UI.Grid _Panel;

    private Skill.Framework.UI.Button _BtnOrganizeByPrefab;
    private Skill.Framework.UI.Button _BtnRemoveEmptyObjects;

    private Skill.Framework.UI.Label _UnitsLabel;
    private Skill.Editor.UI.Vector3Field _UnitsField;
    private Skill.Framework.UI.Button _BtnOrganizeByUnits;





    private void CreateUI()
    {
        _Frame = new Skill.Editor.UI.EditorFrame("Frame", this);
        _Frame.Grid.RowDefinitions.Add(140, Skill.Framework.UI.GridUnitType.Pixel); // Panel
        _Frame.Grid.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // Empty        
        _Frame.Grid.Padding = new Skill.Framework.UI.Thickness(2, 4);

        _Panel = new Skill.Framework.UI.Grid();
        _Panel.RowDefinitions.Add(24, Skill.Framework.UI.GridUnitType.Pixel); // _UnitsField
        _Panel.RowDefinitions.Add(36, Skill.Framework.UI.GridUnitType.Pixel); // _BtnOrganizeByPrefab
        _Panel.RowDefinitions.Add(36, Skill.Framework.UI.GridUnitType.Pixel); // _BtnOrganizeByUnits 
        _Panel.RowDefinitions.Add(36, Skill.Framework.UI.GridUnitType.Pixel); // _BtnRemoveEmptyObjects             
        _Panel.RowDefinitions.Add(4, Skill.Framework.UI.GridUnitType.Pixel); // empty
        _Frame.Controls.Add(_Panel);

        var margin = new Skill.Framework.UI.Thickness(0, 4, 0, 0);

        Grid g = new Grid() { Row = 0, Column = 0, Margin = margin };
        g.ColumnDefinitions.Add(40, GridUnitType.Pixel);
        g.ColumnDefinitions.Add(1, GridUnitType.Star);
        _UnitsLabel = new Label() { Column = 0, Text = "Units" };
        g.Controls.Add(_UnitsLabel);

        _BtnOrganizeByPrefab = new Button() { Row = 1, Column = 0, Margin = margin }; _BtnOrganizeByPrefab.Content.text = "Organize by prefab";
        _Panel.Controls.Add(_BtnOrganizeByPrefab);

        _UnitsField = new Skill.Editor.UI.Vector3Field() { Column = 1, Value = _UnitLength };
        g.Controls.Add(_UnitsField);

        _Panel.Controls.Add(g);

        _BtnOrganizeByUnits = new Button() { Row = 2, Column = 0, Margin = margin }; _BtnOrganizeByUnits.Content.text = "Organize by units";
        _Panel.Controls.Add(_BtnOrganizeByUnits);

        _BtnRemoveEmptyObjects = new Button() { Row = 3, Column = 0, Margin = margin }; _BtnRemoveEmptyObjects.Content.text = "Delete empty objects";
        _Panel.Controls.Add(_BtnRemoveEmptyObjects);

        _BtnOrganizeByPrefab.Click += _BtnOrganizeByPrefab_Click;
        _BtnOrganizeByUnits.Click += _BtnOrganizeByUnits_Click;
        _BtnRemoveEmptyObjects.Click += _BtnRemoveEmptyObjects_Click;
        _UnitsField.ValueChanged += _UnitsField_ValueChanged;
    }

    void _UnitsField_ValueChanged(object sender, System.EventArgs e)
    {
        _UnitLength = _UnitsField.Value;
    }

    void _BtnOrganizeByUnits_Click(object sender, System.EventArgs e)
    {

    }
    void _BtnRemoveEmptyObjects_Click(object sender, System.EventArgs e)
    {
        if (Selection.transforms != null)
        {
            _RemoveEmptyObjects = Selection.transforms;
            Repaint();
        }
    }
    void _BtnOrganizeByPrefab_Click(object sender, System.EventArgs e)
    {
        if (Selection.transforms != null)
        {
            _OrganizeByPrefabs = Selection.transforms;
            Repaint();
        }
    }



    void OnGUI()
    {
        if (_Frame != null)
        {
            RefreshStyles();
            _Frame.OnGUI();
        }
    }

    private void RefreshStyles()
    {
        if (_RefreshStyles)
        {
            _RefreshStyles = false;
        }
    }

    #endregion


    #region By Prefab

    class PrefabInstance
    {
        public Transform Parent { get; set; }

        public Object Prefab { get; private set; }
        public List<Transform> Instances { get; private set; }

        public PrefabInstance(Object prefab)
        {
            this.Prefab = prefab;
            this.Instances = new List<Transform>();
        }
    }

    private List<PrefabInstance> _Groups;

    public void OrganizeByPrefab(Transform root)
    {
        if (_Groups == null)
            _Groups = new List<PrefabInstance>();
        _Groups.Clear();
        Group(root);

        foreach (var g in _Groups)
        {
            GameObject obj = new GameObject(g.Prefab.name + "_G");
            Undo.RegisterCreatedObjectUndo(obj, "group");
            obj.transform.position = root.position;
            obj.transform.rotation = root.rotation;
            g.Parent = obj.transform;
            g.Parent.parent = root;
        }

        foreach (var g in _Groups)
        {
            foreach (var instance in g.Instances)
            {
                Undo.SetTransformParent(instance, g.Parent, "parent");
            }
        }        
        _Groups.Clear();
    }

    private void Group(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Object prefab = PrefabUtility.GetPrefabParent(child.gameObject);
            if (prefab != null)
            {
                PrefabInstance pi = GetPrefabInstance(prefab);
                pi.Instances.Add(child);
            }
            else
            {
                Group(child);
            }
        }
    }

    private PrefabInstance GetPrefabInstance(Object prefab)
    {
        foreach (var g in _Groups)
        {
            if (g.Prefab == prefab)
                return g;
        }
        PrefabInstance pi = new PrefabInstance(prefab);
        _Groups.Add(pi);
        return pi;
    }

    private static bool IsEmptyObject(Transform transform)
    {
        return transform.GetComponents<Component>().Length == 1;
    }

    #endregion

    #region RemoveEmptyObjects

    private void RemoveEmptyObjects(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.childCount == 0 && IsEmptyObject(child))
                Undo.DestroyObjectImmediate(child.gameObject);
            else
                RemoveEmptyObjects(child);
        }
    }
    #endregion

    [MenuItem("Skill/Tools/Organize", false, 3)]
    static void ShowOrganizeWindow()
    {
        OrganizeWindow.Instance.Show();
    }


    private Transform[] _OrganizeByPrefabs;
    private Transform[] _RemoveEmptyObjects;
    void Update()
    {
        if (_OrganizeByPrefabs != null)
        {
            foreach (var t in _OrganizeByPrefabs)
            {
                PrefabType pt = PrefabUtility.GetPrefabType(t.gameObject);
                if (pt != PrefabType.ModelPrefab && pt != PrefabType.Prefab)
                    OrganizeByPrefab(t);
            }
            _OrganizeByPrefabs = null;
        }
        if (_RemoveEmptyObjects != null)
        {
            foreach (var t in _RemoveEmptyObjects)
            {
                PrefabType pt = PrefabUtility.GetPrefabType(t.gameObject);
                if (pt != PrefabType.ModelPrefab && pt != PrefabType.Prefab)
                    RemoveEmptyObjects(t);
            }
            _RemoveEmptyObjects = null;
        }
    }

}
