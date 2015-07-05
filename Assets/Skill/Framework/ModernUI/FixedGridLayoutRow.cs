using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class FixedGridLayoutRow : MonoBehaviour, ILayoutController
    {
        public int Rows = 4;
        public float WidthRatio = 1f;


        private GridLayoutGroup _Grid;

        void Awake()
        {
            _Grid = GetComponent<GridLayoutGroup>();
        }

        public void SetLayoutHorizontal()
        {

            if (_Grid == null)
                _Grid = GetComponent<GridLayoutGroup>();

            float h = _Grid.GetComponent<RectTransform>().rect.height;
            h -= _Grid.padding.vertical;
            h -= Mathf.Max(0, (Rows - 1)) * _Grid.spacing.y;
            h /= Rows;

            float w = h * WidthRatio;
            _Grid.cellSize = new Vector2(w, h);
        }

        public void SetLayoutVertical()
        {

        }
    }
}