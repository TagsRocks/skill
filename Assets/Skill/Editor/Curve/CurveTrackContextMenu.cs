using UnityEngine;
using System.Collections;

namespace Skill.Editor.Curve
{
    public class CurveTrackContextMenu : Skill.Editor.UI.ContextMenu
    {
        private static CurveTrackContextMenu _Instance;
        public static CurveTrackContextMenu Instance
        {
            get
            {
                if (_Instance == null) _Instance = new CurveTrackContextMenu();
                return _Instance;
            }
        }

        private Skill.Editor.UI.MenuItem _MnuAddKey;


        public CurveTrackContextMenu()
        {
            _MnuAddKey = new Skill.Editor.UI.MenuItem("Add Key");
            this.Add(_MnuAddKey);

            _MnuAddKey.Click += _MnuAddKey_Click;
        }

        void _MnuAddKey_Click(object sender, System.EventArgs e)
        {
            CurveTrack track = Owner as CurveTrack;
            float time = track.GetTime(Position.x, false);
            track.AddKey(time);

        }
    }
}