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

            CreateIsDirtyProperty();
            CreateSetAsCleanMethod();


            // create static creator method
            CreateCreatorMethod();

            // create variables and properties
            CreateProperties();


            CreateXmlSaveMethod();
            CreateBinarySaveMethod();
            CreateXmlLoadMethods();
            CreateBinaryLoadMethod();
        }

        private void CreateIsDirtyProperty()
        {
            StringBuilder isDirtyBody = new StringBuilder();
            bool hasClassProperty = false;
            foreach (var p in _SaveClass.Properties)
            {
                if (p.Type == PropertyType.Class)
                {
                    if (!hasClassProperty)
                    {
                        isDirtyBody.AppendLine("if(_IsDirty)  return _IsDirty;");
                        hasClassProperty = true;
                    }
                    ClassPropertyData cp = p as ClassPropertyData;
                    ClassData cd = FindClass(cp.ClassName);
                    if (cd != null)
                    {
                        string pName = Variable.GetName(p.Name);
                        if (p.IsArray)
                        {
                            isDirtyBody.AppendLine(string.Format("if({0} != null) {{", pName));
                            isDirtyBody.AppendLine(string.Format("for (int i = 0; i < {0}.Length; i++) {{", pName));
                            if (!cd.IsStruct)
                                isDirtyBody.AppendLine(string.Format("if({0}[i] != null && {0}[i].IsDirty) return true;", pName));
                            else
                                isDirtyBody.AppendLine(string.Format("if({0}[i].IsDirty) return true;", pName));
                            isDirtyBody.AppendLine("}");
                            isDirtyBody.AppendLine("}");
                        }
                        else
                        {
                            if (!cd.IsStruct)
                                isDirtyBody.AppendLine(string.Format("if({0} != null)", pName));
                            isDirtyBody.AppendLine(string.Format("if({0}.IsDirty) return true;", pName));
                        }
                    }
                }
            }



            isDirtyBody.AppendLine("return _IsDirty;");
            Property isDirtyProperty = new Property("bool", "IsDirty", Variable.GetName("IsDirty"), false);
            isDirtyProperty.Comment = "is any changes happened to savable object";
            isDirtyProperty.SetGetBody(isDirtyBody.ToString());
            Add(isDirtyProperty);
            Add(new Variable("bool", "IsDirty"));
        }

        private void CreateSetAsCleanMethod()
        {
            StringBuilder setAsCleanBody = new StringBuilder();
            setAsCleanBody.AppendLine("_IsDirty = false;");

            foreach (var p in _SaveClass.Properties)
            {
                if (p.Type == PropertyType.Class)
                {
                    ClassPropertyData cp = p as ClassPropertyData;
                    ClassData cd = FindClass(cp.ClassName);
                    if (cd != null)
                    {
                        string pName = Variable.GetName(p.Name);
                        if (p.IsArray)
                        {
                            setAsCleanBody.AppendLine(string.Format("if({0} != null) {{", pName));
                            setAsCleanBody.AppendLine(string.Format("for (int i = 0; i < {0}.Length; i++) {{", pName));
                            if (!cd.IsStruct)
                                setAsCleanBody.AppendLine(string.Format("if({0}[i] != null)", pName));
                            setAsCleanBody.AppendLine(string.Format("{0}[i].SetAsClean();", pName));
                            setAsCleanBody.AppendLine("}");
                            setAsCleanBody.AppendLine("}");
                        }
                        else
                        {
                            if (!cd.IsStruct)
                                setAsCleanBody.AppendLine(string.Format("if({0} != null)", pName));
                            setAsCleanBody.AppendLine(string.Format("{0}.SetAsClean();", pName));
                        }
                    }
                }
            }
            Add(new Method("void", "SetAsClean", setAsCleanBody.ToString()) { Modifier = Modifiers.Public });
        }

        /// <summary>
        /// Find class by name
        /// </summary>
        /// <param name="className">name of class</param>
        /// <returns>class if found , otherwise null</returns>
        private ClassData FindClass(string className)
        {
            if (SaveData.GeneratingInstance != null)
            {
                foreach (var c in SaveData.GeneratingInstance.Classes)
                {
                    if (c.Name == className)
                        return c;
                }
            }
            return null;
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
        private string ConvertToString(PrimitiveDataType pt, bool safe)
        {
            switch (pt)
            {
                case PrimitiveDataType.Integer:
                    return safe ? "Skill.Framework.SafeInt" : "int";
                case PrimitiveDataType.Boolean:
                    return safe ? "Skill.Framework.SafeBool" : "bool";
                case PrimitiveDataType.Float:
                    return safe ? "Skill.Framework.SafeFloat" : "float";
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
                            Add(new Variable(string.Format("{0}[]", ConvertToString(pp.PrimitiveType, pp.SafeMemory)), p.Name));
                            Add(new Property(string.Format("{0}[]", ConvertToString(pp.PrimitiveType, pp.SafeMemory)), p.Name, Variable.GetName(p.Name), true, Property.DirtyMode.CheckAndSet) { Comment = p.Comment });
                        }
                        else
                        {
                            Add(new Variable(ConvertToString(pp.PrimitiveType, pp.SafeMemory), p.Name));
                            Add(new Property(ConvertToString(pp.PrimitiveType, pp.SafeMemory), p.Name, Variable.GetName(p.Name), true, Property.DirtyMode.CheckAndSet) { Comment = p.Comment });
                        }
                        break;
                    case PropertyType.Class:
                        ClassPropertyData cp = p as ClassPropertyData;
                        if (p.IsArray)
                        {
                            Add(new Variable(string.Format("{0}[]", cp.ClassName), p.Name));
                            Add(new Property(string.Format("{0}[]", cp.ClassName), p.Name, Variable.GetName(p.Name), true, Property.DirtyMode.CheckAndSet) { Comment = p.Comment });
                        }
                        else
                        {
                            Add(new Variable(cp.ClassName, p.Name));
                            ClassData cd = FindClass(cp.ClassName);
                            Add(new Property(cp.ClassName, p.Name, Variable.GetName(p.Name), true, cd.IsStruct ? Property.DirtyMode.Set : Property.DirtyMode.CheckAndSet) { Comment = p.Comment });
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
                {
                    toXmlElementBody.AppendLine(string.Format("Skill.Framework.IO.XmlElement {0} = stream.Create(\"{1}\",{3}{2});", variable, p.Name, Variable.GetName(p.Name), ((PrimitivePropertyData)p).GetCast()));
                }
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
                    saveBinaryBody.AppendLine(string.Format("stream.Write({1}{0});", Variable.GetName(p.Name), ((PrimitivePropertyData)p).GetCast()));
                else
                    saveBinaryBody.AppendLine(string.Format("stream.Write<{0}>({1});", ((ClassPropertyData)p).ClassName, Variable.GetName(p.Name)));
            }

            Method saveMethod = new Method("void", "Save", saveBinaryBody.ToString(), "Skill.Framework.IO.BinarySaveStream stream");
            saveMethod.Modifier = Modifiers.Public;
            Add(saveMethod);
        }


        private string GetLoadMethodName(PrimitiveDataType primitive, bool isArray, bool safe)
        {
            string result = "";
            switch (primitive)
            {
                case PrimitiveDataType.Integer:
                    result = safe && isArray ? "ReadSafeInt" : "ReadInt";
                    break;
                case PrimitiveDataType.Float:
                    result = safe && isArray ? "ReadSafeFloat" : "ReadFloat";
                    break;
                case PrimitiveDataType.Boolean:
                    result = safe && isArray ? "ReadSafeBoolean" : "ReadBoolean";
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
                        loadXmlBody.AppendLine(string.Format("this.{0} = stream.{1}( element );", Variable.GetName(p.Name), GetLoadMethodName(pp.PrimitiveType, pp.IsArray, pp.SafeMemory)));
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
            loadXmlBody.AppendLine("SetAsClean();");

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
                        loadMethodBody.AppendLine(string.Format("this.{0} = stream.{1}();", Variable.GetName(p.Name), GetLoadMethodName(pp.PrimitiveType, pp.IsArray, pp.SafeMemory)));
                        break;
                    case PropertyType.Class:
                        ClassPropertyData cp = (ClassPropertyData)p;
                        loadMethodBody.AppendLine(string.Format("this.{0} = stream.{1}( {2}.{3} );", Variable.GetName(p.Name),
                            GetSavableMethodName(cp.ClassName, cp.IsArray), cp.ClassName, GetStaticCreatorMethodName(cp.ClassName)));
                        break;
                }
            }

            loadMethodBody.AppendLine("SetAsClean();");

            Method loadBinaryMethod = new Method("void", "Load", loadMethodBody.ToString(), "Skill.Framework.IO.BinaryLoadStream stream");
            loadBinaryMethod.Modifier = Modifiers.Public;
            Add(loadBinaryMethod);
        }
    }
}
