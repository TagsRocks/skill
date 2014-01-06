using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.Studio.AI
{
    public static class GifAnimationManager
    {
        private class GifAnimationPosture
        {
            private string _StandingPath;
            private string _CrouchedPath;
            private string _PronePath;

            public string NonePostureAnimationPath { get; private set; }


            public GifAnimationPosture(string nonePostureAnimationPath)
            {
                this.NonePostureAnimationPath = nonePostureAnimationPath;

                _StandingPath = MainWindow.Instance.Project.GetGifAnimationPath(NonePostureAnimationPath.Replace(".gif", "_standing.gif"));
                _CrouchedPath = MainWindow.Instance.Project.GetGifAnimationPath(NonePostureAnimationPath.Replace(".gif", "_crouched.gif"));
                _PronePath = MainWindow.Instance.Project.GetGifAnimationPath(NonePostureAnimationPath.Replace(".gif", "_prone.gif"));

                if (!System.IO.File.Exists(_StandingPath)) _StandingPath = null;
                if (!System.IO.File.Exists(_CrouchedPath)) _CrouchedPath = null;
                if (!System.IO.File.Exists(_PronePath)) _PronePath = null;
            }

            public string GetPath(DataModels.Posture posture)
            {
                switch (posture)
                {
                    case Skill.DataModels.Posture.Standing:
                        if (_StandingPath != null) return _StandingPath;
                        break;
                    case Skill.DataModels.Posture.Crouched:
                        if (_CrouchedPath != null) return _CrouchedPath;
                        break;
                    case Skill.DataModels.Posture.Prone:
                        if (_PronePath != null) return _PronePath;
                        break;
                }

                if (_StandingPath != null) return _StandingPath;
                if (_CrouchedPath != null) return _CrouchedPath;
                if (_PronePath != null) return _PronePath;
                return null;
            }
        }

        private static string GetNonePostureAnimationPath(string animPath)
        {
            if (animPath != null)
            {
                animPath = animPath.ToLower();
                if (animPath.EndsWith("_standing.gif"))
                    animPath = animPath.Replace("_standing.gif", ".gif");
                else if (animPath.EndsWith("_crouched.gif"))
                    animPath = animPath.Replace("_crouched.gif", ".gif");
                else if (animPath.EndsWith("_prone.gif"))
                    animPath = animPath.Replace("_prone.gif", ".gif");
            }
            return animPath;
        }

        private static Dictionary<string, GifAnimationPosture> _Animations;

        static GifAnimationManager()
        {
            _Animations = new Dictionary<string, GifAnimationPosture>();
        }

        public static string GetGifPath(string gifPath, DataModels.Posture posture)
        {
            string result = null;
            if (!string.IsNullOrEmpty(gifPath))
            {
                string nonePostureAnimationPath = GetNonePostureAnimationPath(gifPath);
                GifAnimationPosture animation;
                if (!_Animations.TryGetValue(nonePostureAnimationPath, out animation))
                {
                    animation = new GifAnimationPosture(nonePostureAnimationPath);
                    _Animations.Add(animation.NonePostureAnimationPath, animation);
                }

                result = animation.GetPath(posture);
                if (result == null)
                    result = gifPath;
            }
            return result;
        }
    }
}
