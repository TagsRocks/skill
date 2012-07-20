using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skill.DataModels.IO;
using Skill.DataModels;

namespace Skill.CodeGeneration.CSharp
{
    /// <summary>
    /// Generate C# code for SaveClass
    /// </summary>
    class SaveClassClass : Class
    {
        private SaveClass _SaveClass;

        /// <summary>
        /// Create an instance of SaveClassClass
        /// </summary>
        /// <param name="saveClass">SaveClass model</param>
        public SaveClassClass(SaveClass saveClass)
            : base(saveClass.Name)
        {
            this._SaveClass = saveClass;
            this.IsPartial = false;

            AddInherit("Skill.IO.ISavable");

            Method constructor = new Method("", Name, "");
            constructor.Modifiers = Modifiers.Public;
            Add(constructor);

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
            Method m = new Method(this.Name, GetStaticCreatorMethodName(this.Name), string.Format("return new {0}();", this.Name)) { IsStatic = true, Modifiers = Modifiers.Public };
            Add(m);
        } 
        #endregion

        /// <summary>
        /// Covert PrimitiveType to valid c# code
        /// </summary>
        /// <param name="pt">PrimitiveType to conver</param>
        /// <returns>PrimitiveType in c# code</returns>
        private string ConvertToString(PrimitiveType pt)
        {
            switch (pt)
            {
                case PrimitiveType.Int:
                case PrimitiveType.Float:
                case PrimitiveType.Bool:
                case PrimitiveType.String:
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
                        PrimitiveProperty pp = p as PrimitiveProperty;

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
                        ClassProperty cp = p as ClassProperty;
                        if (p.IsArray)
                        {
                            Add(new Variable(string.Format("{0}[]", cp.ClassName), p.Name, "null"));
                            Add(new Property(string.Format("{0}[]", cp.ClassName), p.Name, Variable.GetName(p.Name)) { Comment = p.Comment });
                        }
                        else
                        {
                            Add(new Variable(cp.ClassName, p.Name, "null"));
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
                    toXmlElementBody.AppendLine(string.Format("System.Xml.XmlElement {0} = stream.Create(\"{1}\",{2});", variable, p.Name, Variable.GetName(p.Name)));
                else
                    toXmlElementBody.AppendLine(string.Format("System.Xml.XmlElement {0} = stream.Create<{1}>(\"{2}\",{3});", variable, ((ClassProperty)p).ClassName, p.Name, Variable.GetName(p.Name)));
                toXmlElementBody.AppendLine(string.Format("e.AppendChild({0});", variable));
            }

            Method toXmlElement = new Method("void", "Save", toXmlElementBody.ToString(), "System.Xml.XmlElement e", "Skill.IO.XmlSaveStream stream");
            toXmlElement.Modifiers = Modifiers.Public;
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
                    saveBinaryBody.AppendLine(string.Format("stream.Write<{0}>({1});", ((ClassProperty)p).ClassName, Variable.GetName(p.Name)));
            }

            Method saveMethod = new Method("void", "Save", saveBinaryBody.ToString(), "Skill.IO.BinarySaveStream stream");
            saveMethod.Modifiers = Modifiers.Public;
            Add(saveMethod);
        }


        private string GetLoadMethodName(PrimitiveType primitive, bool isArray)
        {
            string result = "";
            switch (primitive)
            {
                case PrimitiveType.Int:
                    result = "ReadInt";
                    break;
                case PrimitiveType.Float:
                    result = "ReadFloat";
                    break;
                case PrimitiveType.Bool:
                    result = "ReadBoolean";
                    break;
                case PrimitiveType.String:
                    result = "ReadString";
                    break;
                case PrimitiveType.Bounds:
                    result = "ReadBounds";
                    break;
                case PrimitiveType.Color:
                    result = "ReadColor";
                    break;
                case PrimitiveType.Matrix4x4:
                    result = "ReadMatrix4x4";
                    break;
                case PrimitiveType.Plane:
                    result = "ReadPlane";
                    break;
                case PrimitiveType.Quaternion:
                    result = "ReadQuaternion";
                    break;
                case PrimitiveType.Ray:
                    result = "ReadRay";
                    break;
                case PrimitiveType.Rect:
                    result = "ReadRect";
                    break;
                case PrimitiveType.Vector2:
                    result = "ReadVector2";
                    break;
                case PrimitiveType.Vector3:
                    result = "ReadVector3";
                    break;
                case PrimitiveType.Vector4:
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

            loadXmlBody.AppendLine("System.Xml.XmlElement element = e.FirstChild as System.Xml.XmlElement;");
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
                        PrimitiveProperty pp = (PrimitiveProperty)p;
                        loadXmlBody.AppendLine(string.Format("this.{0} = stream.{1}( element );", Variable.GetName(p.Name), GetLoadMethodName(pp.PrimitiveType, pp.IsArray)));
                        break;
                    case PropertyType.Class:
                        ClassProperty cp = (ClassProperty)p;
                        loadXmlBody.AppendLine(string.Format("this.{0} = stream.{1}( element , {2}.{3} );", Variable.GetName(p.Name),
                            GetSavableMethodName(cp.ClassName, cp.IsArray), cp.ClassName, GetStaticCreatorMethodName(cp.ClassName)));
                        break;
                }

                loadXmlBody.AppendLine("break;");
            }

            loadXmlBody.AppendLine("}");
            loadXmlBody.AppendLine("element = element.NextSibling as System.Xml.XmlElement;");
            loadXmlBody.AppendLine("}");

            Method loadMethod = new Method("void", "Load", loadXmlBody.ToString(), "System.Xml.XmlElement e", "Skill.IO.XmlLoadStream stream");
            loadMethod.Modifiers = Modifiers.Public;
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
                        PrimitiveProperty pp = (PrimitiveProperty)p;
                        loadMethodBody.AppendLine(string.Format("this.{0} = stream.{1}();", Variable.GetName(p.Name), GetLoadMethodName(pp.PrimitiveType, pp.IsArray)));
                        break;
                    case PropertyType.Class:
                        ClassProperty cp = (ClassProperty)p;
                        loadMethodBody.AppendLine(string.Format("this.{0} = stream.{1}( {2}.{3} );", Variable.GetName(p.Name),
                            GetSavableMethodName(cp.ClassName, cp.IsArray), cp.ClassName, GetStaticCreatorMethodName(cp.ClassName)));
                        break;
                }                
            }            

            Method loadBinaryMethod = new Method("void", "Load", loadMethodBody.ToString(), "BinaryLoadStream stream");
            loadBinaryMethod.Modifiers = Modifiers.Public;
            Add(loadBinaryMethod);
        }
    }
}
