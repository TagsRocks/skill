using UnityEngine;
using System.Collections;

namespace Skill.Framework
{
    public abstract class Scoreboard : MonoBehaviour
    {
        public class Field
        {
            public string Name;
            public string Value;
            public bool Hash;
        }

        /// <summary> Number of item to retrieve from server </summary>
        public int ItemCount = 10;

        protected abstract string Security { get; }
        protected abstract string PostUrl { get; }
        protected abstract string GetUrl { get; }

        public string PostResult { get; private set; }
        public string GetResult { get; private set; }
        public string Error { get; private set; }
        public bool IsBusy { get; private set; }

        public event System.EventHandler PostCompleted;
        public event System.EventHandler GetCompleted;
        public event System.EventHandler GetError;

        protected virtual void OnPostCompleted()
        {
            if (PostCompleted != null)
                PostCompleted(this, System.EventArgs.Empty);
        }
        protected virtual void OnGetCompleted()
        {
            if (GetCompleted != null)
                GetCompleted(this, System.EventArgs.Empty);
        }
        protected virtual void OnGetError()
        {
            if (GetError != null)
                GetError(this, System.EventArgs.Empty);
        }

        private bool _CallPostCompleted;
        private bool _CallGetCompleted;
        private bool _CallGetError;

        void Update()
        {
            if (_CallGetError)
            {
                OnGetError();
                _CallGetError = false;
            }
            if (_CallPostCompleted)
            {
                OnPostCompleted();
                _CallPostCompleted = false;
            }
            if (_CallGetCompleted)
            {
                OnGetCompleted();
                _CallGetCompleted = false;
            }
        }

        public void Get(string id)
        {
            if (IsBusy) return;
            StartCoroutine(GetScore(id));
        }

        public void Post(params Field[] fields)
        {
            if (IsBusy) return;
            StartCoroutine(PostScore(fields));
        }

        private IEnumerator PostScore(params Field[] fields)
        {
            PostResult = string.Empty;
            IsBusy = true;

            string str = string.Empty;
            foreach (var f in fields)
            {
                if (f.Hash)
                    str += f.Value;
            }
            str += this.Security;
            string hash = Utility.GenerateMD5(str).ToLower();

            WWWForm form = new WWWForm();
            foreach (var f in fields)
                form.AddField(f.Name, f.Value);
            form.AddField("hash", hash);

            WWW www = new WWW(PostUrl, form);
            yield return www;

            Error = www.error;
            PostResult = www.text;
            if (!string.IsNullOrEmpty(Error))
                _CallGetError = true;
            IsBusy = false;
            _CallPostCompleted = true;
        }

        private IEnumerator GetScore(string id)
        {
            GetResult = string.Empty;
            IsBusy = true;

            WWWForm form = new WWWForm();
            form.AddField("limit", ItemCount);
            form.AddField("id", id);

            WWW www = new WWW(GetUrl, form);
            yield return www;

            Error = www.error;
            GetResult = www.text;
            if (!string.IsNullOrEmpty(Error))
                _CallGetError = true;
            IsBusy = false;
            _CallGetCompleted = true;
        }
    }
}