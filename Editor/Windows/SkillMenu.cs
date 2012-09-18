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

        [MenuItem("Skill/Animation/Animation Assigner", false, 50)]
        public static void ShowAnimationAssigner()
        {
            AnimationAssigner.Instance.Show();
        }

        [MenuItem("Skill/AI/BTree Debugger", false, 100)]
        public static void ShowBTreeDebugger()
        {
            BehaviorTreeDebugger.Instance.Show();
        }

        [MenuItem("Skill/Export to Skill Studio", false, 1001)]
        public static void ShowExporter()
        {
            Exporter.Instance.Show();
        }

        [MenuItem("Skill/About", false, 1002)]
        public static void ShowAbout()
        {
            AboutSkill.Instance.Show();
        }


    }
}
