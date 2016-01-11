using UnityEngine;
using System.Collections;


namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MenuPage : Skill.Framework.DynamicBehaviour
    {
        public MenuManager Menu;

        private CanvasGroup _CanvasGroup;
        protected RectTransform _RectTransform;

        public CanvasGroup Group { get { return _CanvasGroup; } }

        public bool IsVisible { get; private set; }

        protected override void GetReferences()
        {
            base.GetReferences();
            _CanvasGroup = GetComponent<CanvasGroup>();
            _RectTransform = GetComponent<RectTransform>();

            if (Menu == null)
                Menu = GetComponentInParent<MenuManager>();
        }




        public virtual void Show()
        {
            if (IsVisible) return;
            this.gameObject.SetActive(true);
            IsVisible = true;
            _CanvasGroup.interactable = true;
            _RectTransform.localPosition = new Vector3(0, 0, 0);
            _RectTransform.anchorMin = new Vector2(0, 0);
            _RectTransform.anchorMax = new Vector2(1, 1);
            _RectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        public virtual void Hide()
        {
            if (!IsVisible) return;
            IsVisible = false;
            _CanvasGroup.interactable = false;
            this.gameObject.SetActive(false);
        }


        public void Back()
        {
            Menu.Back();
        }
    }
}