using UnityEngine;
using System.Collections;
using Skill.Framework.Triggers;
using Skill.Framework.Sequence;


namespace Skill.Framework.Triggers
{
    public class SendMessageTrigger : Trigger
    {

        public GameObject Receiver;                 // The object to receive this message.    
        public string FunctionName;                 // Name of function to call    
        public SendMessageOptions MessageOptions;   // Options    
        public SendMessageParameter ParameterType;  // Parameter type    
        public float Float;                         // Float parameter    
        public int Int;                             // Int parameter    
        public bool Boolean;                        // Boolean parameter    
        public Object ObjectReference;              // Object reference parameter    
        public string String;                       // String parameter


        protected override bool OnEnter(Collider other)
        {
            if (Receiver != null)
            {
                if (!string.IsNullOrEmpty(FunctionName))
                {
                    switch (ParameterType)
                    {
                        case SendMessageParameter.None:
                            Receiver.SendMessage(FunctionName, MessageOptions);
                            break;
                        case SendMessageParameter.Float:
                            Receiver.SendMessage(FunctionName, Float, MessageOptions);
                            break;
                        case SendMessageParameter.Int:
                            Receiver.SendMessage(FunctionName, Int, MessageOptions);
                            break;
                        case SendMessageParameter.Bool:
                            Receiver.SendMessage(FunctionName, Boolean, MessageOptions);
                            break;
                        case SendMessageParameter.ObjectReference:
                            Receiver.SendMessage(FunctionName, ObjectReference, MessageOptions);
                            break;
                        case SendMessageParameter.String:
                            Receiver.SendMessage(FunctionName, String, MessageOptions);
                            break;
                    }
                }
                else
                    Debug.LogWarning("You must specify a valid FunctionName for SendMessage event");
            }
            else
                Debug.LogWarning("You must specify a valid Receiver for SendMessage event");

            return true;
        }

    }
}