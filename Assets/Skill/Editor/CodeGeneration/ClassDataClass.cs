using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.Editor.IO;

namespace Skill.Editor.CodeGeneration
{
    /// <summary>
    /// Generate C# code for SaveClass
    /// </summary>
    class ClassDataClass : Class
    {
        private ClassData _SaveClass;

        /// <summary>
        /// Create an instance of SaveClassClass
        /// </summary>
        /// <param name="saveClass">SaveClass model</param>
        public ClassDataClass(ClassData saveClass)
            : base(saveClass.Name)
        {
            this._SaveClass = saveClass;
            this.IsPartial = false;
            this.IsStruct = saveClass.IsStruct;

            AddInherit("Skill.Framework.IO.ISavable");

            if (!this.IsStruct)
            {
                base.AddAttribute("System.Serializable");
                Method constructor = new Method("", Name, "");
                constructor.Modifier = Modifiers.Public;
                Add(constructor);
            }

            // create static creator method
            CreateCreatorMethod();

            // create variables and properties
            CreateProperties();


            CreateXmlSaveMethod();
            CreateBinarySaveMethod();
            CreateXmlLoadMethods();
            CreateBinaryLoadMethod();
        }

        #region Create static CreatorMethod
        private static string GetStaticCreatorMethodName(string clasName) { return string.Format("Create{0}", clasName); }

        private void CreateCreatorMethod()
        {
            Method m = new Method(this.Name, GetStaticCreatorMethodName(this.Name), string.Format("return new {0}();", this.Name)) { IsStatic = true, Modifier = Modifiers.Public };
            Add(m);
        }
        #endregion

        /// <summary>
        /// Covert PrimitiveType to valid c# code
        /// </summary>
        /// <param name="pt">PrimitiveType to conver</param>
        /// <returns>PrimitiveType in c# code</returns>
        private string ConvertToString(PrimitiveDataType pt)
        {
            switch (pt)
            {
                case PrimitiveDataType.Integer:
                    return "int";
                case PrimitiveDataType.Boolean:
                    return "bool";
                case PrimitiveDataType.Float:
                case PrimitiveDataType.String:
                    return pt.ToString().ToLower();
                default:
                    return pt.ToString();
            }
        }

        /// <summary>
        /// Create variables and properties        
        /// </summary>
        private void CreateProperties()
        {
            foreach (var p in _SaveClass.Properties)
            {
                switch (p.Type)
                {
                    case PropertyType.Primitive:
                        PrimitivePropertyData pp = p as PrimitivePropertyData;

                        if (p.IsArray)
                        {
                            Add(new Variable(string.Format("{0}[]", ConvertToString(pp.PrimitiveType)), p.Name));
                            Add(new Property(string.Format("{0}[]", ConvertToString(pp.PrimitiveType)), p.Name, Variable.GetName(p.Name)) { Comment = p.Comment });
                        }
                        else
                        {
                            Add(new Variable(ConvertToString(pp.PrimitiveType), p.Name));
                            Add(new Property(ConvertToString(pp.PrimitiveType), p.Name, Variable.GetName(p.Name)) { Comment = p.Comment });
                        }
                        break;
                    case PropertyType.Class:
                        ClassPropertyData cp = p as ClassPropertyData;
                        if (p.IsArray)
                        {
                            Add(new Variable(string.Format("{0}[]", cp.ClassName), p.Name));
                            Add(new Property(string.Format("{0}[]", cp.ClassName), p.Name, Variable.GetName(p.Name)) { Comment = p.Comment });
                        }
                        else
                        {
                            Add(new Variable(cp.ClassName, p.Name));
                            Add(new Property(cp.ClassName, p.Name, Variable.GetName(p.Name)) { Comment = p.Comment });
                        }
                        break;
                    default:
                        break;
                }
            }
        }


        private void CreateXmlSaveMethod()
        {
            StringBuilder toXmlElementBody = new StringBuilder();

            string variable;
            foreach (var p in this._SaveClass.Properties)
            {
                variable = string.Format("_{0}Element", p.Name);
                if (p.Type == PropertyType.Primitive)
                    toXmlElementBody.AppendLine(string.Format("Skill.Framework.IO.XmlElement {0} = stream.Create(\"{1}\",{2});", variable, p.Name, Variable.GetName(p.Name)));
                else
                    toXmlElementBody.AppendLine(string.Format("Skill.Framework.IO.XmlElement {0} = stream.Create<{1}>(\"{2}\",{3});", variable, ((ClassPropertyData)p).ClassName, p.Name, Variable.GetName(p.Name)));
                toXmlElementBody.AppendLine(string.Format("e.AppendChild({0});", variable));
            }

            Method toXmlElement = new Method("void", "Save", toXmlElementBody.ToString(), "Skill.Framework.IO.XmlElement e", "Skill.Framework.IO.XmlSaveStream stream");
            toXmlElement.Modifier = Modifiers.Public;
            Add(toXmlElement);
        }

        private void CreateBinarySaveMethod()
        {
            StringBuilder saveBinaryBody = new StringBuilder();

            foreach (var p in this._SaveClass.Properties)
            {
                if (p.Type == PropertyType.Primitive)
                    saveBinaryBody.AppendLine(string.Format("stream.Write({0});", Variable.GetName(p.Name)));
                else
                    saveBinaryBody.AppendLine(string.Format("stream.Write<{0}>({1});", ((ClassPropertyData)p).ClassName, Variable.GetName(p.Name)));
            }

            Method saveMethod = new Method("void", "Save", saveBinaryBody.ToString(), "Skill.Framework.IO.BinarySaveStream stream");
            saveMethod.Modifier = Modifiers.Public;
            Add(saveMethod);
        }


        private string GetLoadMethodName(PrimitiveDataType primitive, bool isArray)
        {
            string result = "";
            switch (primitive)
            {
                case PrimitiveDataType.Integer:
                    result = "ReadInt";
                    break;
                case PrimitiveDataType.Float:
                    result = "ReadFloat";
                    break;
                case PrimitiveDataType.Boolean:
                    result = "ReadBoolean";
                    break;
                case PrimitiveDataType.String:
                    result = "ReadString";
                    break;
                case PrimitiveDataType.Bounds:
                    result = "ReadBounds";
                    break;
                case PrimitiveDataType.Color:
                    result = "ReadColor";
                    break;
                case PrimitiveDataType.Matrix4x4:
                    result = "ReadMatrix4x4";
                    break;
                case PrimitiveDataType.Plane:
                    result = "ReadPlane";
                    break;
                case PrimitiveDataType.Quaternion:
                    result = "ReadQuaternion";
                    break;
                case PrimitiveDataType.Ray:
                    result = "ReadRay";
                    break;
                case PrimitiveDataType.Rect:
                    result = "ReadRect";
                    break;
                case PrimitiveDataType.Vector2:
                    result = "ReadVector2";
                    break;
                case PrimitiveDataType.Vector3:
                    result = "ReadVector3";
                    break;
                case PrimitiveDataType.Vector4:
                    result = "ReadVector4";
                    break;
            }
            if (isArray)
                result += "Array";
            return result;
        }

        private string GetSavableMethodName(string className, bool isArray)
        {
            string result = "ReadSavable";
            if (isArray)
                result += "Array";

            return string.Format("{0}<{1}>", result, className);
        }


        private void CreateXmlLoadMethods()
        {
            StringBuilder loadXmlBody = new StringBuilder();

            loadXmlBody.AppendLine("Skill.Framework.IO.XmlElement element = e.FirstChild as Skill.Framework.IO.XmlElement;");
            loadXmlBody.AppendLine("while (element != null)");
            loadXmlBody.AppendLine("{");

            loadXmlBody.AppendLine("switch (element.Name)");
            loadXmlBody.AppendLine("{");

            foreach (var p in _SaveClass.Properties)
            {
                loadXmlBody.AppendLine(string.Format("case \"{0}\":", p.Name));

                switch (p.Type)
                {
                    case PropertyType.Primitive:
                        PrimitivePropertyData pp = (PrimitivePropertyData)p;
                        loadXmlBody.AppendLine(string.Format("this.{0} = stream.{1}( element );", Variable.GetName(p.Name), GetLoadMethodName(pp.PrimitiveType, pp.IsArray)));
                        break;
                    case PropertyType.Class:
                        ClassPropertyData cp = (ClassPropertyData)p;
                        loadXmlBody.AppendLine(string.Format("this.{0} = stream.{1}( element , {2}.{3} );", Variable.GetName(p.Name),
                            GetSavableMethodName(cp.ClassName, cp.IsArray), cp.ClassName, GetStaticCreatorMethodName(cp.ClassName)));
                        break;
                }

                loadXmlBody.AppendLine("break;");
            }

            loadXmlBody.AppendLine("}");
            loadXmlBody.AppendLine("element = e.GetNextSibling(element);");
            loadXmlBody.AppendLine("}");

            Method loadMethod = new Method("void", "Load", loadXmlBody.ToString(), "Skill.Framework.IO.XmlElement e", "Skill.Framework.IO.XmlLoadStream stream");
            loadMethod.Modifier = Modifiers.Public;
            Add(loadMethod);
        }

        private void CreateBinaryLoadMethod()
        {
            StringBuilder loadMethodBody = new StringBuilder();

            foreach (var p in _SaveClass.Properties)
            {
                switch (p.Type)
                {
                    case PropertyType.Primitive:
                        PrimitivePropertyData pp = (PrimitivePropertyData)p;
                        loadMethodBody.AppendLine(string.Format("this.{0} = stream.{1}();", Variable.GetName(p.Name), GetLoadMethodName(pp.PrimitiveType, pp.IsArray)));
                        break;
                    case PropertyType.Class:
                        ClassPropertyData cp = (ClassPropertyData)p;
                        loadMethodBody.AppendLine(string.Format("this.{0} = stream.{1}( {2}.{3} );", Variable.GetName(p.Name),
                            GetSavableMethodName(cp.ClassName, cp.IsArray), cp.ClassName, GetStaticCreatorMethodName(cp.ClassName)));
                        break;
                }
            }

            Method loadBinaryMethod = new Method("void", "Load", loadMethodBody.ToString(), "Skill.Framework.IO.BinaryLoadStream stream");
            loadBinaryMethod.Modifier = Modifiers.Public;
            Add(loadBinaryMethod);
        }
    }
}
