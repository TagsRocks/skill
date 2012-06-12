using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.AI;
using Skill.DataModels;
using Skill.DataModels.Animation;

namespace Skill.Studio
{
    #region SkinMeshNode
    public class SkinMeshNode : EntityNode
    {
        public override EntityType EntityType { get { return EntityType.SkinMesh; } }

        public SkinMeshNode()
            : base("NewSkinMesh")
        {

        }
    }
    #endregion

    public class SkinMeshNodeViewModel : EntityNodeViewModel
    {
        public SkinMeshNodeViewModel(EntityNodeViewModel parent, SkinMeshNode skinmesh)
            : base(parent, skinmesh)
        {            
        }

        public override string ImageName { get { return Images.SkinMesh; } }

        public override void New()
        {
            SkinMesh sk = new SkinMesh();
            SaveData(sk);
        }

        public override EntityNodeViewModel Clone(EntityNodeViewModel copyParent)
        {
            SkinMeshNode cloneNode = new SkinMeshNode();
            cloneNode.Name = Name;
            SkinMeshNodeViewModel cloneNodeVM = new SkinMeshNodeViewModel(copyParent, cloneNode);
            
            return cloneNodeVM;
        }


        public override void SaveData(object data)
        {                        
            if (data != null)
            {
                SkinMesh sk = data as SkinMesh;
                if (sk != null)
                {
                    string filename = AbsolutePath;
                    Project.CreateDirectory(filename);

                    sk.Name = System.IO.Path.GetFileNameWithoutExtension(filename);

                    DataFile datafile = new DataFile();
                    datafile.Document.Add(sk.ToXElement());
                    datafile.Save(filename);
                }
            }

        }

        public override object LoadData()
        {
            string filename = AbsolutePath;
            if (System.IO.File.Exists(filename))
            {
                SkinMesh sk = new SkinMesh();
                DataFile data = new DataFile(filename);
                sk.Load(data.Root);
                sk.Name = System.IO.Path.GetFileNameWithoutExtension(filename);
                return sk;
            }
            return null;
        }
    }
}
