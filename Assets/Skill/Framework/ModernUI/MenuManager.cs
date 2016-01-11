using UnityEngine;
using System.Collections;
using Skill.Framework;
using System.Collections.Generic;
using System;

namespace Skill.Framework.ModernUI
{
    public class MenuManager : DynamicBehaviour
    {

        private Stack<MenuPage> _PageStack;    // Stack of frames that prioritize frames to be visible
        private Stack<MenuPage> _DialogStack;  // Stack of frames that prioritize frames to be visible as dialogs                    

        /// <summary> Retrieves top most visible frame </summary>
        public MenuPage TopPage
        {
            get
            {
                if (_PageStack.Count > 0)
                    return _PageStack.Peek();
                else
                    return null;
            }
        }

        /// <summary> Retrieves top most visible dialog </summary>
        public MenuPage TopDialog
        {
            get
            {
                if (_DialogStack.Count > 0)
                    return _DialogStack.Peek();
                else
                    return null;
            }
        }

        /// <summary>
        /// Occurs when last frame removed(backed) from menu
        /// </summary>
        public event EventHandler Exit;
        /// <summary>
        /// Occurs when last frame removed(backed) from menu
        /// </summary>
        protected virtual void OnExit()
        {
            if (Exit != null)
                Exit(this, EventArgs.Empty);

        }

        public int PageStackCount { get { return _PageStack.Count; } }

        protected override void Start()
        {
            base.Start();
            this._PageStack = new Stack<MenuPage>();
            this._DialogStack = new Stack<MenuPage>();
        }

        /// <summary>
        /// Show a frame in next render.do not call this when a dialog is visible or the frame already in use
        /// </summary>
        /// <param name="frameName">Valid name of frame to show in next render</param>
        public virtual void ShowPage(MenuPage page)
        {

            if (TopDialog != null)// if a dialog is visible, operation is invalid
                throw new InvalidOperationException("can not show a page when showing a dialog.");

            if (_PageStack.Contains(page))
                throw new InvalidOperationException("This page is already in stack.");

            if (_PageStack.Count > 0)
                _PageStack.Peek().Hide();

            _PageStack.Push(page);
            page.Show();
        }

        /// <summary>
        /// Show a dialog in next render.do not call this when the frame already in use
        /// </summary>
        /// <param name="dialog">Valid name of frame(dialog) to show in next render</param>
        public virtual void ShowDialog(MenuPage dialog)
        {
            if (_DialogStack.Contains(dialog))
                throw new InvalidOperationException("This page is already in stack.");
            _DialogStack.Push(dialog);
            dialog.Show();
        }

        /// <summary>
        /// Pop current frame from stack and show previous frame. do not call this when a dialog is visible 
        /// </summary>
        public virtual void Back()
        {
            MenuPage top = null;
            if (_DialogStack.Count > 0) // a dialog is visible        
                top = _DialogStack.Pop();
            else if (_PageStack.Count > 0) // if back operation is valid        
                top = _PageStack.Pop();

            if (top != null)
            {
                top.Hide();
                if (_PageStack.Count > 0)
                {
                    MenuPage peek = _PageStack.Peek();
                    if (!peek.IsVisible)
                        peek.Show();
                }
                else if (_PageStack.Count == 0)
                    OnExit();
            }
        }

        /// <summary>
        /// Clear view stack
        /// </summary>
        public void Clear()
        {
            _PageStack.Clear();
            _DialogStack.Clear();
        }
    }
}