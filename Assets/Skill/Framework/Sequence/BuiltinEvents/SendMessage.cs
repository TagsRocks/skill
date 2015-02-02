using UnityEngine;
using System.Collections;


namespace Skill.Framework.Sequence
{
    public enum SendMessageParameter
    {
        None,
        Float,
        Int,
        Bool,
        ObjectReference,
        String,
    }

    /// <summary>
    /// Add this event if you would like to call a function on an object.
    /// </summary>
    [CustomEvent("SendMessage", "Signal")]
    public class SendMessage : EventKey
    {
        [SerializeField]
        private GameObject _Receiver;
        [SerializeField]
        private string _FunctionName;
        [SerializeField]
        private SendMessageParameter _ParameterType;
        [SerializeField]
        private SendMessageOptions _MessageOptions;
        [SerializeField]
        private float _FloatParameter;
        [SerializeField]
        private int _IntParameter;
        [SerializeField]
        private bool _BoolParameter;
        [SerializeField]
        private Object _ObjectReferenceParameter;
        [SerializeField]
        private string _StringParameter;



        [ExposeProperty(101, "Receiver", " The object to receive this message.")]
        public GameObject Receiver { get { return _Receiver; } set { _Receiver = value; } }

        [ExposeProperty(102, "Function Name", "Name of function to call")]
        public string FunctionName { get { return _FunctionName; } set { _FunctionName = value; } }

        [ExposeProperty(103, "MessageOptions", "Options")]
        public SendMessageOptions MessageOptions { get { return _MessageOptions; } set { _MessageOptions = value; } }

        [ExposeProperty(104, "Parameter Type", "Parameter type")]
        public SendMessageParameter ParameterType { get { return _ParameterType; } set { _ParameterType = value; } }

        [ExposeProperty(105, "Float", "Float parameter")]
        public float FloatParameter { get { return _FloatParameter; } set { _FloatParameter = value; } }

        [ExposeProperty(106, "Int", "Int parameter")]
        public int IntParameter { get { return _IntParameter; } set { _IntParameter = value; } }

        [ExposeProperty(107, "Bool", "Boolean parameter")]
        public bool BoolParameter { get { return _BoolParameter; } set { _BoolParameter = value; } }

        [ExposeProperty(108, "Object", "Object reference parameter")]
        public Object ObjectReferenceParameter { get { return _ObjectReferenceParameter; } set { _ObjectReferenceParameter = value; } }

        [ExposeProperty(109, "String", "String parameter")]
        public string StringParameter { get { return _StringParameter; } set { _StringParameter = value; } }

        public override void FireEvent()
        {
            if (_Receiver != null)
            {
                if (!string.IsNullOrEmpty(_FunctionName))
                {
                    switch (_ParameterType)
                    {
                        case SendMessageParameter.None:
                            _Receiver.SendMessage(_FunctionName, _MessageOptions);
                            break;
                        case SendMessageParameter.Float:
                            _Receiver.SendMessage(_FunctionName, _FloatParameter, _MessageOptions);
                            break;
                        case SendMessageParameter.Int:
                            _Receiver.SendMessage(_FunctionName, _IntParameter, _MessageOptions);
                            break;
                        case SendMessageParameter.Bool:
                            _Receiver.SendMessage(_FunctionName, _BoolParameter, _MessageOptions);
                            break;
                        case SendMessageParameter.ObjectReference:
                            _Receiver.SendMessage(_FunctionName, _ObjectReferenceParameter, _MessageOptions);
                            break;
                        case SendMessageParameter.String:
                            _Receiver.SendMessage(_FunctionName, _StringParameter, _MessageOptions);
                            break;
                    }
                }
                else
                    Debug.LogWarning("You must specify a valid FunctionName for SendMessage event");
            }
            else
                Debug.LogWarning("You must specify a valid Receiver for SendMessage event");
        }
    }
}