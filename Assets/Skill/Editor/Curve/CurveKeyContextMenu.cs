using UnityEngine;
using System.Collections;
using Skill.Editor;

namespace Skill.Editor.Curve
{
    public class CurveKeyContextMenu : Skill.Editor.UI.ContextMenu
    {
        private static CurveKeyContextMenu _Instance;
        public static CurveKeyContextMenu Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new CurveKeyContextMenu();
                return _Instance;
            }
        }

        private Skill.Editor.UI.MenuItem _MnuDelete;
        private Skill.Editor.UI.MenuItem _MnuAuto;
        private Skill.Editor.UI.MenuItem _MnuFreeSmooth;
        private Skill.Editor.UI.MenuItem _MnuFlat;
        private Skill.Editor.UI.MenuItem _MnuBroken;

        private Skill.Editor.UI.MenuItem _MnuLeftTangent;
        private Skill.Editor.UI.MenuItem _MnuLeftTangent_Free;
        private Skill.Editor.UI.MenuItem _MnuLeftTangent_Linear;
        private Skill.Editor.UI.MenuItem _MnuLeftTangent_Constant;

        private Skill.Editor.UI.MenuItem _MnuRightTangent;
        private Skill.Editor.UI.MenuItem _MnuRightTangent_Free;
        private Skill.Editor.UI.MenuItem _MnuRightTangent_Linear;
        private Skill.Editor.UI.MenuItem _MnuRightTangent_Constant;

        private Skill.Editor.UI.MenuItem _MnuBothTangent;
        private Skill.Editor.UI.MenuItem _MnuBothTangent_Free;
        private Skill.Editor.UI.MenuItem _MnuBothTangent_Linear;
        private Skill.Editor.UI.MenuItem _MnuBothTangent_Constant;


        public CurveKeyContextMenu()
        {
            _MnuDelete = new Skill.Editor.UI.MenuItem("Delete");
            _MnuAuto = new Skill.Editor.UI.MenuItem("Auto");
            _MnuFreeSmooth = new Skill.Editor.UI.MenuItem("Free Smooth");
            _MnuFlat = new Skill.Editor.UI.MenuItem("Flat");
            _MnuBroken = new Skill.Editor.UI.MenuItem("Broken");

            _MnuLeftTangent = new Skill.Editor.UI.MenuItem("Left Tangent");
            _MnuLeftTangent_Free = new Skill.Editor.UI.MenuItem("Free");
            _MnuLeftTangent_Linear = new Skill.Editor.UI.MenuItem("Linear");
            _MnuLeftTangent_Constant = new Skill.Editor.UI.MenuItem("Constant");
            _MnuLeftTangent.Add(_MnuLeftTangent_Free);
            _MnuLeftTangent.Add(_MnuLeftTangent_Linear);
            _MnuLeftTangent.Add(_MnuLeftTangent_Constant);

            _MnuRightTangent = new Skill.Editor.UI.MenuItem("Right Tangent");
            _MnuRightTangent_Free = new Skill.Editor.UI.MenuItem("Free");
            _MnuRightTangent_Linear = new Skill.Editor.UI.MenuItem("Linear");
            _MnuRightTangent_Constant = new Skill.Editor.UI.MenuItem("Constant");
            _MnuRightTangent.Add(_MnuRightTangent_Free);
            _MnuRightTangent.Add(_MnuRightTangent_Linear);
            _MnuRightTangent.Add(_MnuRightTangent_Constant);


            _MnuBothTangent = new Skill.Editor.UI.MenuItem("Both Tangents");
            _MnuBothTangent_Free = new Skill.Editor.UI.MenuItem("Free");
            _MnuBothTangent_Linear = new Skill.Editor.UI.MenuItem("Linear");
            _MnuBothTangent_Constant = new Skill.Editor.UI.MenuItem("Constant");
            _MnuBothTangent.Add(_MnuBothTangent_Free);
            _MnuBothTangent.Add(_MnuBothTangent_Linear);
            _MnuBothTangent.Add(_MnuBothTangent_Constant);

            this.Add(_MnuDelete);
            this.AddSeparator();
            this.Add(_MnuAuto);
            this.Add(_MnuFreeSmooth);
            this.Add(_MnuFlat);
            this.Add(_MnuBroken);
            this.AddSeparator();
            this.Add(_MnuLeftTangent);
            this.Add(_MnuRightTangent);
            this.Add(_MnuBothTangent);


            _MnuDelete.Click += _MnuDelete_Click;
            _MnuAuto.Click += _MnuAuto_Click;
            _MnuFreeSmooth.Click += _MnuFreeSmooth_Click;
            _MnuFlat.Click += _MnuFlat_Click;
            _MnuBroken.Click += _MnuBroken_Click;
            _MnuLeftTangent_Free.Click += _MnuLeftTangent_Free_Click;
            _MnuLeftTangent_Linear.Click += _MnuLeftTangent_Linear_Click;
            _MnuLeftTangent_Constant.Click += _MnuLeftTangent_Constant_Click;
            _MnuRightTangent_Free.Click += _MnuRightTangent_Free_Click;
            _MnuRightTangent_Linear.Click += _MnuRightTangent_Linear_Click;
            _MnuRightTangent_Constant.Click += _MnuRightTangent_Constant_Click;
            _MnuBothTangent_Free.Click += _MnuBothTangent_Free_Click;
            _MnuBothTangent_Linear.Click += _MnuBothTangent_Linear_Click;
            _MnuBothTangent_Constant.Click += _MnuBothTangent_Constant_Click;
        }

        protected override void BeginShow()
        {
            CurveKey key = Owner as CurveKey;

            var state = key.Track.View.Editor.GetSelectionState();

            string deleteName = null;
            if (state.SelectionCount > 1)
                deleteName = "Delete Keys";
            else
                deleteName = "Delete key";

            if (!deleteName.Equals(_MnuDelete.Name))
            {
                _MnuDelete.Name = deleteName;
                OnChanged();
            }

            if (state.SelectionCount > 0)
            {
                _MnuDelete.IsEnabled = true;
                _MnuAuto.IsEnabled = true; _MnuAuto.IsChecked = state.Auto;
                _MnuFreeSmooth.IsEnabled = true; _MnuFreeSmooth.IsChecked = state.FreeSmooth;
                _MnuFlat.IsEnabled = true; _MnuFlat.IsChecked = state.Flat;
                _MnuBroken.IsEnabled = true; _MnuBroken.IsChecked = state.Broken;

                _MnuLeftTangent.IsEnabled = true;
                _MnuLeftTangent_Free.IsChecked = state.LeftFree;
                _MnuLeftTangent_Linear.IsChecked = state.LeftLinear;
                _MnuLeftTangent_Constant.IsChecked = state.LeftConstant;

                _MnuRightTangent.IsEnabled = true;
                _MnuRightTangent_Free.IsChecked = state.RightFree;
                _MnuRightTangent_Linear.IsChecked = state.RightLinear;
                _MnuRightTangent_Constant.IsChecked = state.RightConstant;

                _MnuBothTangent.IsEnabled = true;
                _MnuBothTangent_Free.IsChecked = state.LeftFree && state.RightFree;
                _MnuBothTangent_Linear.IsChecked = state.LeftLinear && state.RightLinear;
                _MnuBothTangent_Constant.IsChecked = state.LeftConstant && state.RightConstant;
            }
            else
            {
                _MnuDelete.IsEnabled = false;
                _MnuAuto.IsEnabled = false; _MnuAuto.IsChecked = false;
                _MnuFreeSmooth.IsEnabled = false; _MnuFreeSmooth.IsChecked = false;
                _MnuFlat.IsEnabled = false; _MnuFlat.IsChecked = false;
                _MnuBroken.IsEnabled = false; _MnuBroken.IsChecked = false;

                _MnuLeftTangent.IsEnabled = false;
                _MnuLeftTangent_Free.IsChecked = false;
                _MnuLeftTangent_Linear.IsChecked = false;
                _MnuLeftTangent_Constant.IsChecked = false;

                _MnuRightTangent.IsEnabled = false;
                _MnuRightTangent_Free.IsChecked = false;
                _MnuRightTangent_Linear.IsChecked = false;
                _MnuRightTangent_Constant.IsChecked = false;

                _MnuBothTangent.IsEnabled = false;
                _MnuBothTangent_Free.IsChecked = false;
                _MnuBothTangent_Linear.IsChecked = false;
                _MnuBothTangent_Constant.IsChecked = false;
            }

            base.BeginShow();
        }

        void _MnuDelete_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.RemoveSelection();
        }

        void _MnuAuto_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetSmooth();
        }

        void _MnuFreeSmooth_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetEditable();
        }

        void _MnuFlat_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetFlat();
        }

        void _MnuBroken_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetBroken();
        }

        void _MnuLeftTangent_Free_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetLeftEditable();
        }

        void _MnuLeftTangent_Linear_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetLeftLinear();
        }

        void _MnuLeftTangent_Constant_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetLeftConstant();
        }

        void _MnuRightTangent_Free_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetRightEditable();
        }

        void _MnuRightTangent_Linear_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetRightLinear();
        }

        void _MnuRightTangent_Constant_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetRightConstant();
        }

        void _MnuBothTangent_Free_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetBothEditable();
        }

        void _MnuBothTangent_Linear_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetBothLinear();
        }

        void _MnuBothTangent_Constant_Click(object sender, System.EventArgs e)
        {
            CurveKey key = Owner as CurveKey;
            key.Track.View.Editor.SetBothConstant();
        }




    }
}
