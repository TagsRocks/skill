using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Skill.Studio
{
    #region IUnDoRedoCommand
    /// <summary>
    /// defines command to undo and redo some changes
    /// </summary>
    public interface IUnDoRedoCommand
    {
        void Undo();
        void Redo();
    }
    #endregion

    public class UnDoRedoChangeEventArgs : EventArgs
    {
        public IUnDoRedoCommand Command { get; private set; }

        public UnDoRedoChangeEventArgs(IUnDoRedoCommand command)
        {
            this.Command = command;
        }
    }

    public delegate void UnDoRedoChangeEventHandler(UnDoRedo sender, UnDoRedoChangeEventArgs args);

    /// <summary>
    /// Defines a class that take care of changes and change commands
    /// </summary>
    public class UnDoRedo
    {
        #region UnDoRedoStack
        /// <summary>
        /// Defines a cycle stack
        /// </summary>
        /// <typeparam name="T">Type of object to store in stack</typeparam>
        class UnDoRedoStack<T> where T : class
        {
            T[] _List;
            int _Front;
            int _Count;

            int GetCycledIndex(int index)
            {
                if (index < 0) return _List.Length - 1;
                else if (index >= _List.Length) return 0;
                else return index;
            }

            public UnDoRedoStack(int capacity)
            {
                _List = new T[Math.Max(capacity, 10)];
                for (int i = 0; i < _List.Length; i++)
                {
                    _List[i] = null;
                }
                _Front = 0;
                _Count = 0;
            }

            public int Count { get { return _Count; } }

            public T Pop()
            {
                if (_Count == 0)
                    throw new InvalidOperationException("Invalid operation pop where stack is empty");
                _Front = GetCycledIndex(_Front - 1);
                T t = _List[_Front];
                _List[_Front] = null;
                _Count--;
                return t;
            }

            public void Push(T item)
            {
                _List[_Front] = item;
                _Front = GetCycledIndex(_Front + 1);
                _List[_Front] = null;
                _Count = Math.Min(_List.Length, _Count + 1);
            }

            public void Clear()
            {
                _Count = 0;
                _Front = 0;
                for (int i = 0; i < _List.Length; i++)
                {
                    _List[i] = null;
                }
            }
        }
        #endregion


        UnDoRedoStack<IUnDoRedoCommand> _UndoCommands; // list of commands that can undo
        UnDoRedoStack<IUnDoRedoCommand> _RedoCommands; // list of conmmands that can redo

        /// <summary> Occurs when any change made in class </summary>
        public event EventHandler Change;
        /// <summary> Raised after a redo </summary>
        public event UnDoRedoChangeEventHandler RedoChange;
        /// <summary> Raised after a undo </summary>
        public event UnDoRedoChangeEventHandler UndoChange;

        // make history to unchanged state
        public void ResetChangeCount() { ChangeCount = 0; }
        /// <summary> if zero no changes happened </summary>
        public int ChangeCount { get; private set; }

        private void OnChange()
        {
            if (Change != null)
                Change(this, EventArgs.Empty);
        }
        private void OnUndoChange(UnDoRedoChangeEventArgs args)
        {
            if (UndoChange != null)
                UndoChange(this, args);
        }
        private void OnRedoChange(UnDoRedoChangeEventArgs args)
        {
            if (RedoChange != null)
                RedoChange(this, args);
        }

        /// <summary>
        /// Capacity of undo and redo stacks
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Create an UnDoRedo object
        /// </summary>
        public UnDoRedo()
        {
            IsEnable = true;
            Capacity = Properties.Settings.Default.UndoSize;
            _UndoCommands = new UnDoRedoStack<IUnDoRedoCommand>(Capacity);
            _RedoCommands = new UnDoRedoStack<IUnDoRedoCommand>(Capacity);
            ResetChangeCount();
        }

        /// <summary> Whether exist any undo commands </summary>
        public bool CanUndo { get { return _UndoCommands.Count > 0; } }
        /// <summary> Whether exist any redo commands </summary>
        public bool CanRedo { get { return _RedoCommands.Count > 0; } }

        /// <summary>
        /// Redo
        /// </summary>
        /// <param name="levels">number of redo</param>
        public void Redo(int levels = 1)
        {
            IsEnable = false;
            for (int i = 0; i < levels; i++)
            {
                if (_RedoCommands.Count != 0)
                {
                    IUnDoRedoCommand command = _RedoCommands.Pop();
                    command.Redo();
                    _UndoCommands.Push(command);
                    ChangeCount++;
                    UnDoRedoChangeEventArgs args = new UnDoRedoChangeEventArgs(command);
                    OnRedoChange(args);
                    OnChange();

                }
            }
            IsEnable = true;
        }

        /// <summary>
        /// Uundo
        /// </summary>
        /// <param name="levels">number of undo</param>
        public void Undo(int levels = 1)
        {
            IsEnable = false;
            for (int i = 0; i < levels; i++)
            {
                if (_UndoCommands.Count != 0)
                {
                    IUnDoRedoCommand command = _UndoCommands.Pop();
                    command.Undo();
                    _RedoCommands.Push(command);
                    ChangeCount--;
                    UnDoRedoChangeEventArgs args = new UnDoRedoChangeEventArgs(command);
                    OnUndoChange(args);
                    OnChange();
                }
            }
            IsEnable = true;
        }

        #region UndoHelperFunctions

        /// <summary>
        /// if false, ignore insert commands
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// Insert new command to undo stack list and ready for undo
        /// </summary>
        /// <param name="command"></param>
        public void Insert(IUnDoRedoCommand command)
        {
            if (!IsEnable) return;
            {
                _UndoCommands.Push(command);
                _RedoCommands.Clear();
                ChangeCount++;
                OnChange();
            }
        }

        #endregion
    }


    #region ChangePropertyUnDoRedo
    /// <summary>
    /// Undo and redo simple property of object (string, int, float, ...)
    /// </summary>
    class ChangePropertyUnDoRedo : IUnDoRedoCommand
    {
        object _Object; // object that has property
        object _NewValue;// new property value
        object _OldValue;// old property value
        string _PropertyName; // name of property

        public ChangePropertyUnDoRedo(object obj, string propertyName, object newValue, object oldValue)
        {
            this._Object = obj;
            this._NewValue = newValue;
            this._OldValue = oldValue;
            this._PropertyName = propertyName;
        }

        public void Undo()
        {
            SetValue(_OldValue);
        }

        public void Redo()
        {
            SetValue(_NewValue);
        }

        private void SetValue(object value)
        {
            PropertyInfo prop = _Object.GetType().GetProperty(_PropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(_Object, value, null);
            }
        }
    }
    #endregion
}
