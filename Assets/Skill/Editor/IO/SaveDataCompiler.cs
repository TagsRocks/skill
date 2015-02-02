using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Skill.Editor.IO
{
    public static class SaveDataCompiler
    {
        private static bool _ErrorFound;
        private static SaveData _SaveData;
        public static bool Compile(SaveData data)
        {
            _ErrorFound = false;
            _SaveData = data;
            CheckForErrors(_SaveData);
            List<string> nameList = new List<string>(data.Classes.Length);
            foreach (var cl in data.Classes)
            {
                CheckForErrors(cl);

                if (!nameList.Contains(cl.Name))
                {
                    int count = 0;
                    foreach (var item in data.Classes)
                    {
                        if (item.Name == cl.Name)
                            count++;
                    }
                    if (count > 1)
                    {
                        Debug.LogError(string.Format("There are {0} Class with same name ({1}).", count, cl.Name));
                        _ErrorFound = true;
                    }
                    nameList.Add(cl.Name);
                }

            }
            nameList.Clear();

            _SaveData = null;
            return !_ErrorFound;
        }

        private static void CheckForErrors(ClassData cl)
        {
            if (string.IsNullOrEmpty(cl.Name))
            {
                Debug.LogError("There is a class with empty name.");
                _ErrorFound = true;
            }
            if (cl.Properties == null) return;
            foreach (var p in cl.Properties)
            {
                if (string.IsNullOrEmpty(p.Name))
                {
                    Debug.LogError("There is a property with empty name.");
                    _ErrorFound = true;
                }
                else
                {
                    int count = 0;
                    foreach (var item in cl.Properties)
                    {
                        if (item.Name == p.Name)
                            count++;
                    }
                    if (count > 1)
                    {
                        Debug.LogError(string.Format("There are {0} Property in Class {1} with same name ({2}).", count, cl.Name, p.Name));
                        _ErrorFound = true;
                    }
                }

                if (p.Type == PropertyType.Class)
                {
                    int count = 0;
                    foreach (var item in _SaveData.Classes)
                    {
                        if (item.Name == ((ClassPropertyData)p).ClassName)
                            count++;
                    }
                    if (count <= 0)
                    {
                        Debug.LogError(string.Format("The property {0} of class {1} has invalid ClassName.", p.Name, cl.Name));
                        _ErrorFound = true;
                    }
                }
            }
        }



        private static void CheckForWarnings()
        {
            CheckForNoPropertyWarning(_SaveData);
            foreach (var cl in _SaveData.Classes)
                CheckForNoPropertyWarning(cl);
        }

        private static void CheckForNoPropertyWarning(ClassData cl)
        {
            if (cl.Properties == null || cl.Properties.Length == 0)
            {
                Debug.LogWarning(string.Format("The Class {0} has not any Property.", cl.Name));
            }
        }
    }
}