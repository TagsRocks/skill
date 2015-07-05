using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class FixedGridLayoutColumn : MonoBehaviour, ILayoutController
    {
        public int Columns = 4;
        public float HeigthRatio = 1.2f;


        private GridLayoutGroup _Grid;

        void Awake()
        {
            _Grid = GetComponent<GridLayoutGroup>();
        }

        public void SetLayoutHorizontal()
        {
            if (_Grid == null)
                _Grid = GetComponent<GridLayoutGroup>();


            float w = _Grid.GetComponent<RectTransform>().rect.width;
            w -= _Grid.padding.horizontal;
            w -= Mathf.Max(0, (Columns - 1)) * _Grid.spacing.x;
            w /= Columns;

            float h = w * HeigthRatio;
            _Grid.cellSize = new Vector2(w, h);
        }

        public void SetLayoutVertical()
        {

        }
    }
}