using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI
{
    public class SkillMenu : ScriptableObject
    {
        [MenuItem("Skill/Export to Skill Studio")]
        public static void AnimationEditor()
        {
            Exporter.Instance.Show();
        }

        [MenuItem("Skill/AI/BTree Debugger")]
        public static void BTreeDebugger()
        {
            BehaviorTreeDebugger.Instance.ShowUtility();
        }

        [MenuItem("Skill/About")]
        public static void About()
        {
            AboutSkill.Instance.ShowUtility();
        }
    }
}
