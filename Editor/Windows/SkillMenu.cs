using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    public class SkillMenu : ScriptableObject
    {
        [MenuItem("Skill/General/Implant", false, 0)]
        public static void ShowImplant()
        {
            Implant.Instance.Show();
        }

        [MenuItem("Skill/AI/BTree Debugger", false, 10)]
        public static void ShowBTreeDebugger()
        {
            BehaviorTreeDebugger.Instance.ShowUtility();
        }

        [MenuItem("Skill/Export to Skill Studio", false, 101)]
        public static void ShowExporter()
        {
            Exporter.Instance.Show();
        }

        [MenuItem("Skill/About", false, 102)]
        public static void ShowAbout()
        {
            AboutSkill.Instance.ShowUtility();
        }


    }
}
