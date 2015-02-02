using UnityEngine;
using System.Collections;

namespace Skill.Editor.AI
{
    public class AccessLimitDecoratorItem : DecoratorItem
    {

        public AccessLimitDecoratorItem(AccessLimitDecoratorData data)
            : base(data)
        {
        }
        public override string Title { get { return "AccessLimitDecorator"; } }

        private ALProperties _ALProperties;
        public override Skill.Editor.UI.Extended.PropertiesPanel Properties
        {
            get
            {
                if (_ALProperties == null) _ALProperties = new ALProperties(this);
                return _ALProperties;
            }
        }

        class ALProperties : ItemProperties
        {
            private AccessLimitDecoratorItem _ALItem;
            public ALProperties(AccessLimitDecoratorItem item)
                : base(item)
            {
                this._ALItem = item;
            }

            protected override void RefreshData()
            {
                RefreshAccessClasses();
                base.RefreshData();
            }

            private void RefreshAccessClasses()
            {
                AccessLimitDecoratorData data = (AccessLimitDecoratorData)_ALItem.Data;
                _AccessClassField.Options.Clear();
                _AccessClassField.SelectedIndex = -1;

                if (_ALItem.Editor.Asset.SharedKeys != null)
                {
                    int selectedIndex = -1;
                    for (int i = 0; i < _ALItem.Editor.Asset.SharedKeys.Length; i++)
                    {
                        var sk = _ALItem.Editor.Asset.SharedKeys[i];
                        if (sk != null)
                        {
                            Skill.Editor.UI.PopupOption op = new UI.PopupOption(i, sk.name);
                            _AccessClassField.Options.Add(op);
                            if (sk.name == data.ClassName)
                            {
                                selectedIndex = i;
                            }
                        }
                    }
                    _AccessClassField.SelectedIndex = selectedIndex;
                }
                else
                {
                    data.ClassName = string.Empty;
                }
                RefreshAccessKeys();
            }

            private void RefreshAccessKeys()
            {
                AccessLimitDecoratorData data = (AccessLimitDecoratorData)_ALItem.Data;
                _AccessKeyField.Options.Clear();
                _AccessKeyField.SelectedIndex = -1;

                if (!string.IsNullOrEmpty(data.ClassName) && _AccessClassField.SelectedOption != null)
                {
                    SharedAccessKeysAsset asset = null;
                    foreach (var item in _ALItem.Editor.Asset.SharedKeys)
                    {
                        if (item.name == data.ClassName)
                            asset = item;
                    }

                    if (asset != null)
                    {
                        int selectedIndex = -1;
                        var keys = asset.Load().Keys;
                        for (int i = 0; i < keys.Length; i++)
                        {
                            Skill.Editor.UI.PopupOption op = new UI.PopupOption((int)keys[i].Type, keys[i].Key);
                            _AccessKeyField.Options.Add(op);
                            if (keys[i].Key == data.AccessKey)
                            {
                                selectedIndex = i;
                            }
                        }
                        _AccessKeyField.SelectedIndex = selectedIndex;
                    }
                    else
                    {
                        data.AccessKey = string.Empty;
                    }
                }
                else
                {
                    data.AccessKey = string.Empty;
                }
            }


            private Skill.Editor.UI.Popup _AccessClassField;
            private Skill.Editor.UI.Popup _AccessKeyField;

            protected override void CreateCustomFileds()
            {
                _AccessClassField = new Skill.Editor.UI.Popup() { Margin = new Framework.UI.Thickness(0, 2) };
                _AccessClassField.Label.text = "SharedKeys Class";
                Controls.Add(_AccessClassField);

                _AccessKeyField = new Skill.Editor.UI.Popup() { Margin = new Framework.UI.Thickness(0, 2) };
                _AccessKeyField.Label.text = "Key";
                Controls.Add(_AccessKeyField);

                _AccessClassField.OptionChanged += _AccessClassField_OptionChanged;
                _AccessKeyField.OptionChanged += _AccessKeyField_OptionChanged;
                base.CreateCustomFileds();
            }

            void _AccessClassField_OptionChanged(object sender, System.EventArgs e)
            {
                if (IgnoreChanges) return;
                if (_AccessClassField.SelectedOption != null)
                    ((AccessLimitDecoratorData)_ALItem.Data).ClassName = _AccessClassField.SelectedOption.Content.text;
                else
                    ((AccessLimitDecoratorData)_ALItem.Data).ClassName = string.Empty;
                RefreshAccessKeys();
            }

            void _AccessKeyField_OptionChanged(object sender, System.EventArgs e)
            {
                if (IgnoreChanges) return;
                if (_AccessKeyField.SelectedOption != null)
                {
                    ((AccessLimitDecoratorData)_ALItem.Data).AccessKey = _AccessKeyField.SelectedOption.Content.text;
                    ((AccessLimitDecoratorData)_ALItem.Data).KeyType = (Framework.AI.AccessKeyType)_AccessKeyField.SelectedOption.Value;
                }
                else
                {
                    ((AccessLimitDecoratorData)_ALItem.Data).AccessKey = string.Empty;
                    ((AccessLimitDecoratorData)_ALItem.Data).KeyType = Framework.AI.AccessKeyType.CounterLimit;
                }
            }
        }
    }
}