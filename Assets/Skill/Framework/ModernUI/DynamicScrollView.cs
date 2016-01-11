using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine.UI;

namespace Skill.Framework.ModernUI
{
    [RequireComponent(typeof(ScrollRect))]
    public class DynamicScrollView : Skill.Framework.DynamicBehaviour, IList<object>
    {
        public GameObject ItemPrefab;
        public float ItemHeightFactor = 0.14f;
        public int Spacing = 0;
        public HideFlags HideFlags = HideFlags.HideInHierarchy;

        private RectTransform _RectTransform;
        private ScrollRect _ScrollRect;
        private Skill.Framework.ScreenSizeChange _ScreenSizeChange;
        private int _PreIndex;
        private bool _Changed;
        private int _Spacing;
        private List<ScrollItem> _Items = new List<ScrollItem>();

        public ScrollRect ScrollRect { get { return _ScrollRect; } }


        public Action<GameObject, int> Initialize;

        private class ScrollItem
        {
            public GameObject Object;
            public object UserData;
            public int Index;
        }

        protected override void GetReferences()
        {
            base.GetReferences();
            _ScrollRect = GetComponent<ScrollRect>();
            _RectTransform = GetComponent<RectTransform>();
        }

        protected override void Update()
        {
            base.Update();
            UpdateContentSize();
            if (_ScreenSizeChange.IsChanged || _Changed || _Spacing != Spacing)
            {
                UpdateItemPositions();
                _Changed = false;
                _Spacing = Spacing;
            }
            UpdateViewItems();
        }


        private void EnableItem(ScrollItem data)
        {
            if (data.Object == null)
            {
                data.Object = Instantiate(ItemPrefab);
                data.Object.hideFlags = this.HideFlags;
                if (Initialize != null)
                    Initialize(data.Object, data.Index);
            }
            if (data.Object.transform.parent == null)
            {
                data.Object.transform.SetParent(_ScrollRect.content, false);
                UpdateItemPosition(data);
            }
            if (!data.Object.gameObject.activeSelf)
                data.Object.gameObject.SetActive(true);
        }

        private void DisableItem(ScrollItem data)
        {
            if (data.Object != null)
            {
                if (data.Object.gameObject.activeSelf)
                    data.Object.gameObject.SetActive(false);
                data.Object.transform.SetParent(null);
            }
        }

        private void UpdateItemPosition(RectTransform rt, int index, float itemH)
        {
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, ((itemH + Spacing) * index) * -1);
            Vector2 size = rt.sizeDelta;
            size.y = itemH;
            rt.sizeDelta = size;
        }

        private void UpdateItemPosition(ScrollItem data)
        {
            RectTransform rt = data.Object.GetComponent<RectTransform>();
            if (rt == null)
            {
                Debug.LogError("RectTransform not found");
                return;
            }
            float itemW = _ScrollRect.content.rect.width;
            float itemH = itemW * ItemHeightFactor;
            UpdateItemPosition(rt, data.Index, itemH);
        }
        private void UpdateItemPositions()
        {
            float itemW = _ScrollRect.content.rect.width;
            float itemH = itemW * ItemHeightFactor;
            for (int i = 0; i < _Items.Count; i++)
            {
                ScrollItem item = _Items[i];
                item.Index = i;
                if (item.Object != null)
                {
                    RectTransform rt = item.Object.GetComponent<RectTransform>();
                    if (rt == null)
                    {
                        Debug.LogError("RectTransform not found");
                        return;
                    }
                    UpdateItemPosition(rt, item.Index, itemH);
                }
            }

        }
        private void UpdateContentSize()
        {
            _ScrollRect.content.anchorMin = new Vector2(0, 1);
            _ScrollRect.content.anchorMax = new Vector2(1, 1);
            _ScrollRect.content.pivot = new Vector2(0.5f, 1);

            float itemH = _ScrollRect.content.rect.width * ItemHeightFactor;
            Vector2 size = _ScrollRect.content.sizeDelta;
            size.y = itemH * _Items.Count;
            _ScrollRect.content.sizeDelta = size;
        }
        private void UpdateViewItems()
        {
            if (_Items.Count == 0) return;

            float itemH = _ScrollRect.content.rect.width * ItemHeightFactor;
            float height = _ScrollRect.content.rect.height;
            float scrollPos = 1.0f - Mathf.Clamp01(_ScrollRect.normalizedPosition.y);
            float posH = height * scrollPos;
            float view = _RectTransform.rect.height;
            float minH = posH - view;
            float maxH = minH + view * 2;

            if (minH < 0)
            {
                maxH += Mathf.Abs(minH);
                minH = 0;
            }
            else if (maxH > height)
            {
                minH -= maxH - height;
                maxH = height;
            }

            minH -= itemH;
            maxH += itemH;

            int index = Mathf.FloorToInt(scrollPos * (_Items.Count - 1));


            float h = posH;
            if (_PreIndex != index)
            {
                int step = _PreIndex < index ? -1 : 1;
                int ii = index;
                while (ii != _PreIndex)
                {
                    if (ii >= 0 && ii < _Items.Count)
                    {
                        if (h >= minH && h <= maxH)
                            EnableItem(_Items[ii]);
                        else
                            DisableItem(_Items[ii]);
                    }
                    h += step * itemH;
                    ii += step;
                }
            }


            h = posH;
            for (int i = index - 1; i >= 0; i--)
            {
                if (h >= minH)
                {
                    EnableItem(_Items[i]);
                }
                else
                {
                    if (_Items[i].Object == null || !_Items[i].Object.activeSelf)
                        break;
                    DisableItem(_Items[i]);
                }
                h -= itemH;
            }

            h = posH;
            for (int i = index; i < _Items.Count; i++)
            {
                if (h <= maxH)
                {
                    EnableItem(_Items[i]);
                }
                else
                {
                    if (_Items[i].Object == null || !_Items[i].Object.activeSelf)
                        break;
                    DisableItem(_Items[i]);
                }
                h += itemH;
            }

            _PreIndex = index;
        }

        public int IndexOf(object userData)
        {
            for (int i = 0; i < _Items.Count; i++)
            {
                if (_Items[i].UserData == userData)
                    return i;
            }
            return -1;
        }

        public void Insert(int index, object userData)
        {
            ScrollItem si = new ScrollItem() { Index = index, UserData = userData };
            _Items.Insert(index, si);
            _Changed = true;
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _Items.Count)
            {
                ScrollItem item = _Items[index];
                if (item.Object != null)
                {
                    item.Object.transform.SetParent(null);
                    Destroy(item.Object);
                }
                _Items.RemoveAt(index);
                _Changed = true;
            }
        }

        public GameObject GetObject(int index)
        {
            return _Items[index].Object;
        }

        public object this[int index] { get { return _Items[index].UserData; } set { _Items[index].UserData = value; } }

        public void Add(object userData)
        {
            ScrollItem si = new ScrollItem() { Index = _Items.Count, UserData = userData };
            _Items.Add(si);
            _Changed = true;
        }

        public void Clear()
        {
            foreach (var item in _Items)
            {
                if (item.Object != null)
                {
                    item.Object.transform.SetParent(null);
                    Destroy(item.Object);
                }
            }
            _Items.Clear();
        }

        public bool Contains(object userData)
        {
            for (int i = 0; i < _Items.Count; i++)
            {
                if (_Items[i].UserData == userData)
                    return true;
            }
            return false;
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            for (int i = 0; i < _Items.Count; i++)
                array[arrayIndex++] = _Items[i];
        }

        public int Count { get { return _Items.Count; } }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(object userData)
        {
            int index = IndexOf(userData);
            if (index >= 0 && index < _Items.Count)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return new ScrollItemEnumerator(_Items);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class ScrollItemEnumerator : IEnumerator<object>
        {
            IEnumerator<ScrollItem> _Enumerator;
            public ScrollItemEnumerator(List<ScrollItem> items)
            {
                _Enumerator = items.GetEnumerator();
            }

            public object Current
            {
                get
                {
                    ScrollItem item = _Enumerator.Current;
                    if (item == null) return null;
                    return item.UserData;
                }
            }

            public void Dispose()
            {
                _Enumerator.Dispose();
            }


            public bool MoveNext()
            {
                return _Enumerator.MoveNext();
            }

            public void Reset()
            {
                _Enumerator.Reset();
            }
        }
    }
}