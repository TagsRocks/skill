using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Skill.Studio
{
    /// <summary>
    /// Defines address of some useful images
    /// </summary>
    public class Images 
    {        
        public static string ImageDirectory { get { return "/SkillStudio;component/Images/Default/"; } }                        

        public static string SharedAccessKeys { get { return Path.Combine(ImageDirectory, "SharedAccessKeys.png"); } }        
        public static string SkinMesh { get { return Path.Combine(ImageDirectory, "SkinMesh.png"); } }
        public static string AnimationSet { get { return Path.Combine(ImageDirectory, "AnimationSet.png"); } }
        public static string SaveGame { get { return Path.Combine(ImageDirectory, "SaveGame.png"); } }

        public static string Bone { get { return Path.Combine(ImageDirectory, "Bone.png"); } }

        public static string AnimationTree { get { return Path.Combine(ImageDirectory, "AnimationTree.png"); } }
        public static string AnimationTree_D { get { return Path.Combine(ImageDirectory, "AnimationTree_D.png"); } }

        public static string BehaviorTree { get { return Path.Combine(ImageDirectory, "BehaviorTree.png"); } }
        public static string BehaviorTree_D { get { return Path.Combine(ImageDirectory, "BehaviorTree_D.png"); } }

        public static string Action { get { return Path.Combine(ImageDirectory, "Action.png"); } }
        public static string Action_D { get { return Path.Combine(ImageDirectory, "Action_D.png"); } }

        public static string Decorator { get { return Path.Combine(ImageDirectory, "Decorator.png"); } }
        public static string Decorator_D { get { return Path.Combine(ImageDirectory, "Decorator_D.png"); } }

        public static string Condition { get { return Path.Combine(ImageDirectory, "Condition.png"); } }
        public static string Condition_D { get { return Path.Combine(ImageDirectory, "Condition_D.png"); } }

        public static string Selector { get { return Path.Combine(ImageDirectory, "Selector.png"); } }
        public static string Selector_D { get { return Path.Combine(ImageDirectory, "Selector_D.png"); } }

        public static string FolderOpen { get { return Path.Combine(ImageDirectory, "Folder_Open.png"); } }
        public static string FolderOpen_D { get { return Path.Combine(ImageDirectory, "Folder_Open_D.png"); } }

        public static string FolderClosed { get { return Path.Combine(ImageDirectory, "Folder_Closed.png"); } }
        public static string FolderClosed_D { get { return Path.Combine(ImageDirectory, "Folder_Closed_D.png"); } }

        public static string Project { get { return Path.Combine(ImageDirectory, "Project.png"); } }
        public static string Project_D { get { return Path.Combine(ImageDirectory, "Project_D.png"); } }

        public static string Sequence { get { return Path.Combine(ImageDirectory, "Sequence.png"); } }
        public static string Sequence_D { get { return Path.Combine(ImageDirectory, "Sequence_D.png"); } }

        public static string Concurrent { get { return Path.Combine(ImageDirectory, "Concurrent.png"); } }
        public static string Concurrent_D { get { return Path.Combine(ImageDirectory, "Concurrent_D.png"); } }

        public static string Random { get { return Path.Combine(ImageDirectory, "Random.png"); } }
        public static string Random_D { get { return Path.Combine(ImageDirectory, "Random_D.png"); } }

        public static string Priority { get { return Path.Combine(ImageDirectory, "Priority.png"); } }
        public static string Priority_D { get { return Path.Combine(ImageDirectory, "Priority_D.png"); } }

        public static string Loop { get { return Path.Combine(ImageDirectory, "Loop.png"); } }
        public static string Loop_D { get { return Path.Combine(ImageDirectory, "Loop_D.png"); } }

        public static string Fixed { get { return Path.Combine(ImageDirectory, "Fixed.png"); } }
        public static string Fixed_D { get { return Path.Combine(ImageDirectory, "Fixed_D.png"); } }


        public static string NewProject { get { return Path.Combine(ImageDirectory, "NewProject.png"); } }
        public static string NewProject_D { get { return Path.Combine(ImageDirectory, "NewProject_D.png"); } }

        public static string Build { get { return Path.Combine(ImageDirectory, "Build.png"); } }
        public static string Build_D { get { return Path.Combine(ImageDirectory, "Build_D.png"); } }

        public static string CheckForErrors { get { return Path.Combine(ImageDirectory, "CheckForErrors.png"); } }
        public static string CheckForErrors_D { get { return Path.Combine(ImageDirectory, "CheckForErrors_D.png"); } }

        public static string Copy { get { return Path.Combine(ImageDirectory, "Copy.png"); } }
        public static string Copy_D { get { return Path.Combine(ImageDirectory, "Copy_D.png"); } }

        public static string Cut { get { return Path.Combine(ImageDirectory, "Cut.png"); } }
        public static string Cut_D { get { return Path.Combine(ImageDirectory, "Cut_D.png"); } }

        public static string Delete { get { return Path.Combine(ImageDirectory, "Delete.png"); } }
        public static string Delete_D { get { return Path.Combine(ImageDirectory, "Delete_D.png"); } }

        public static string Down { get { return Path.Combine(ImageDirectory, "Down.png"); } }
        public static string Down_D { get { return Path.Combine(ImageDirectory, "Down_D.png"); } }

        public static string New { get { return Path.Combine(ImageDirectory, "New.png"); } }
        public static string New_D { get { return Path.Combine(ImageDirectory, "New_D.png"); } }

        public static string Paste { get { return Path.Combine(ImageDirectory, "Paste.png"); } }
        public static string Paste_D { get { return Path.Combine(ImageDirectory, "Paste_D.png"); } }

        public static string Play { get { return Path.Combine(ImageDirectory, "Play.png"); } }
        public static string Play_D { get { return Path.Combine(ImageDirectory, "Play_D.png"); } }

        public static string Properties { get { return Path.Combine(ImageDirectory, "Properties.png"); } }
        public static string Properties_D { get { return Path.Combine(ImageDirectory, "Properties_D.png"); } }

        public static string Save { get { return Path.Combine(ImageDirectory, "Save.png"); } }
        public static string Save_D { get { return Path.Combine(ImageDirectory, "Save_D.png"); } }

        public static string Open { get { return Path.Combine(ImageDirectory, "Open.png"); } }
        public static string Open_D { get { return Path.Combine(ImageDirectory, "Open_D.png"); } }

        public static string SaveAll { get { return Path.Combine(ImageDirectory, "SaveAll.png"); } }
        public static string SaveAll_D { get { return Path.Combine(ImageDirectory, "SaveAll_D.png"); } }

        public static string Up { get { return Path.Combine(ImageDirectory, "Up.png"); } }
        public static string Up_D { get { return Path.Combine(ImageDirectory, "Up_D.png"); } }

        public static string Edit_Undo { get { return Path.Combine(ImageDirectory, "Edit_Undo.png"); } }
        public static string Edit_Undo_D { get { return Path.Combine(ImageDirectory, "Edit_Undo_D.png"); } }

        public static string Edit_Redo { get { return Path.Combine(ImageDirectory, "Edit_Redo.png"); } }
        public static string Edit_Redo_D { get { return Path.Combine(ImageDirectory, "Edit_Redo_D.png"); } }

        public static string Settings { get { return Path.Combine(ImageDirectory, "Settings.png"); } }
        public static string Settings_D { get { return Path.Combine(ImageDirectory, "Settings_D.png"); } }

        public static string Help { get { return Path.Combine(ImageDirectory, "Help.png"); } }
        public static string Help_D { get { return Path.Combine(ImageDirectory, "Help_D.png"); } }


        public static string CSharpCode { get { return Path.Combine(ImageDirectory, "CSharp.png"); } }
    }
}
