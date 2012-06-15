using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio.Compiler
{
    public abstract class DataCompiler
    {
        private ICollection<CompileError> _Errors;

        public EntityNodeViewModel Node { get; private set; }
        public EntityType CompileType { get; private set; }

        public DataCompiler(EntityType compileType, ICollection<CompileError> errors)
        {
            this.CompileType = compileType;
            this._Errors = errors;
        }

        public void Compile(EntityNodeViewModel node)
        {
            if (node.EntityType != CompileType) return;
            this.Node = node;
            Compile();
        }

        private void AddCompileError(string errorDesc, ErrorType errorType)
        {
            CompileError err = new CompileError() { NodeAddress = Node.LocalPath, Type = errorType };
            err.Description = errorDesc;
            _Errors.Add(err);
        }

        protected void AddError(string errorDesc)
        {
            AddCompileError(errorDesc, ErrorType.Error);
        }

        protected void AddWarning(string warningDesc)
        {
            AddCompileError(warningDesc, ErrorType.Warning);
        }

        protected void AddMessage(string messageDesc)
        {
            AddCompileError(messageDesc, ErrorType.Message);
        }

        protected abstract void Compile();
    }
}
