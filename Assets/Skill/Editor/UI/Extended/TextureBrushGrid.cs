using Skill.Framework.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor.UI.Extended
{
    public class TextureBrushGrid : Grid
    {

        private TextureBrush[] _Brushes;
        private Skill.Framework.UI.Box _BrushBg;
        private Skill.Framework.UI.SelectionGrid _BrushSelectionGrid;
        private bool _TryToLoadStyle;

        public TextureBrush this[int index] { get { return _Brushes[index]; } }
        public int Count { get { return _Brushes != null ? _Brushes.Length : 0; } }
        public int SelectedIndex { get { return _BrushSelectionGrid.SelectedIndex; } set { _BrushSelectionGrid.SelectedIndex = value; } }
        public TextureBrush SelectedBrush
        {
            get
            {
                if (_Brushes != null && _BrushSelectionGrid.SelectedIndex >= 0 && _BrushSelectionGrid.SelectedIndex < _Brushes.Length)
                    return _Brushes[_BrushSelectionGrid.SelectedIndex];
                else
                    return null;
            }
        }
        public Skill.Framework.UI.SelectionGrid SelectionGrid { get { return _BrushSelectionGrid; } }

        public TextureBrushGrid(int brushSize = 64)
        {
            LoadBrushes(brushSize);
            _BrushBg = new Skill.Framework.UI.Box() { Row = 0, Column = 0 };
            _BrushSelectionGrid = new Skill.Framework.UI.SelectionGrid() { Row = 0, Column = 0, XCount = 10 };
            for (int i = 0; i < _Brushes.Length; i++)
            {
                Skill.Framework.UI.SelectionGridItem item = new Skill.Framework.UI.SelectionGridItem();
                item.Name = _Brushes[i].Brush.name;
                item.Content.image = _Brushes[i].Brush;
                _BrushSelectionGrid.Items.Add(item);
            }

            this.Controls.Add(_BrushBg);
            this.Controls.Add(_BrushSelectionGrid);

            _TryToLoadStyle = false;
        }

        protected override void Render()
        {
            if (!_TryToLoadStyle)
            {
                _BrushSelectionGrid.Style = "gridlist";
                _TryToLoadStyle = true;
            }
            base.Render();
        }

        private void LoadBrushes(int brushSize)
        {
            List<Texture2D> list = new List<Texture2D>();
            int num = 1;
            Texture2D texture = null;
            do
            {
                texture = (Texture2D)EditorGUIUtility.Load("Brushes/builtin_brush_" + num + ".png");
                if (texture != null)
                {
                    list.Add(texture);
                }
                num++;
            } while (texture != null);
            num = 0;
            do
            {
                texture = EditorGUIUtility.FindTexture("brush_" + num + ".png");
                if (texture != null)
                {
                    list.Add(texture);
                }
                num++;
            } while (texture != null);

            _Brushes = new TextureBrush[list.Count];
            for (int i = 0; i < list.Count; i++)
                _Brushes[i] = new TextureBrush(list[i], brushSize);

        }

        public void Destroy()
        {
            if (_Brushes != null)
            {
                for (int i = 0; i < _Brushes.Length; i++)
                {
                    if (_Brushes[i] != null)
                        _Brushes[i].Destroy();
                }
                _Brushes = null;
            }
            _BrushSelectionGrid.Items.Clear();
        }
    }
}
