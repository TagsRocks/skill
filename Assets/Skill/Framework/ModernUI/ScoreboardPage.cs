using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Skill.Framework.ModernUI
{
    public abstract class ScoreboardPage : MenuPage
    {
        public bool GetOnFirstShow = true;
        public bool GetAfterPost = true;
        public Scoreboard Scoreboard;
        public GameObject LoadingIndicator;

        private bool _FirstTime;
        public override void Show()
        {
            base.Show();
            LoadingIndicator.SetActive(false);
            if (!_FirstTime)
            {
                _FirstTime = true;
                Scoreboard.PostCompleted += Scoreboard_PostCompleted;
                Scoreboard.GetCompleted += Scoreboard_GetCompleted;
                if (!Scoreboard.IsBusy && GetOnFirstShow)
                    Get();
            }
        }
        public virtual void Submit()
        {
            Scoreboard.Field[] fields = GetPostFields();
            if (fields != null)
                Scoreboard.Post(fields);
            else
                EnablePage(true);
        }

        void Scoreboard_GetCompleted(object sender, System.EventArgs e)
        {
            EnablePage(true);
            LoadingIndicator.SetActive(false);
            if (string.IsNullOrEmpty(Scoreboard.Error))
                _ReloadTable = true;
            else
                ShowError(Scoreboard.Error);
        }
        void Scoreboard_PostCompleted(object sender, System.EventArgs e)
        {
            EnablePage(true);
            if (string.IsNullOrEmpty(Scoreboard.Error))
            {
                if (GetAfterPost)
                    Get();
            }
            else
                ShowError(Scoreboard.Error);
        }

        private bool _ReloadTable;
        protected override void Update()
        {
            if (_ReloadTable)
            {
                if (!string.IsNullOrEmpty(Scoreboard.GetResult))
                    UpdateList(Scoreboard.GetResult);
                _ReloadTable = false;
            }
            base.Update();
        }
        private void Post()
        {
            if (Scoreboard.IsBusy) return;
            EnablePage(false);
        }
        public virtual void Get()
        {
            if (Scoreboard.IsBusy) return;
            Scoreboard.Get(Id);
            EnablePage(false);
            LoadingIndicator.SetActive(true);
        }


        protected abstract void EnablePage(bool enable);
        protected abstract void UpdateList(string data);
        protected abstract string Id { get; }
        protected abstract Scoreboard.Field[] GetPostFields();
        protected abstract void ShowError(string error);
    }
}