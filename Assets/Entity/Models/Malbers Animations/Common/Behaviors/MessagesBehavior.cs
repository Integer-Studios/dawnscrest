using UnityEngine;
namespace MalbersAnimations
{
    [System.Serializable]
    public class MesssageItem
    {
        public string message;
        public TypeMessage typeM;
        public bool boolValue;
        public int intValue;
        public float floatValue;

        public float time;
        public bool sent;
    }
    public class MessagesBehavior : StateMachineBehaviour
    {
        public MesssageItem[] onEnterMessage;
        public MesssageItem[] onExitMessage;
        public MesssageItem[] onTimeMessage;


        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (MesssageItem ontimeM in onTimeMessage)
                ontimeM.sent = false;

            foreach (MesssageItem onEnterM in onEnterMessage)
                DeliverMessage(onEnterM, animator);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (MesssageItem onExitM in onExitMessage)
                DeliverMessage(onExitM, animator);
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (MesssageItem onTimeM in onTimeMessage)
            {
                if (!onTimeM.sent && stateInfo.normalizedTime >= onTimeM.time)
                {
                    onTimeM.sent = true;
                    DeliverMessage(onTimeM, animator);
                }
            }
        }

        void DeliverMessage(MesssageItem m, Animator anim)
        {
            switch (m.typeM)
            {
                case TypeMessage.Bool:
                    anim.SendMessage(m.message, m.boolValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Int:
                    anim.SendMessage(m.message, m.intValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Float:
                    anim.SendMessage(m.message, m.floatValue, SendMessageOptions.DontRequireReceiver);
                    break;
                default:
                    break;
            }
        }
    }
}