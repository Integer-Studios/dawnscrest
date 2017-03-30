using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


namespace MalbersAnimations
{
    [CustomEditor(typeof(MessagesBehavior))]
    public class MessageBehaviorsEd : Editor
    {
        private ReorderableList 
            listOnEnter,
            listOnExit,
            listOnTime;

        bool OnEnter, OnExit, OnTime;

        private MessagesBehavior MMessage;

        private void OnEnable()
        {
            MMessage = ((MessagesBehavior)target);

            listOnEnter = new ReorderableList(serializedObject, serializedObject.FindProperty("onEnterMessage"), true, true, true, true);
            listOnExit = new ReorderableList(serializedObject, serializedObject.FindProperty("onExitMessage"), true, true, true, true);
            listOnTime = new ReorderableList(serializedObject, serializedObject.FindProperty("onTimeMessage"), true, true, true, true);


            listOnEnter.drawElementCallback = drawElementCallback1;
            listOnEnter.drawHeaderCallback = HeaderCallbackDelegate1;

            listOnExit.drawElementCallback = drawElementCallback2;
            listOnExit.drawHeaderCallback = HeaderCallbackDelegate1;

            listOnTime.drawElementCallback = drawElementCallback3;
            listOnTime.drawHeaderCallback = HeaderCallbackDelegate2;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            EditorGUILayout.BeginVertical(MalbersEditor.Style(MalbersEditor.MA_LightBlue));
            EditorGUILayout.HelpBox("Sends Messages to the Animator Asociate Scripts", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(MalbersEditor.Style(MalbersEditor.MA_LightGray));
            {
                EditorGUILayout.BeginVertical(MalbersEditor.Style(MalbersEditor.MA_LightGray));
                EditorGUI.indentLevel++;
                if (listOnEnter.count > 0) OnEnter = true;
                OnEnter = EditorGUILayout.Foldout(OnEnter, "On Enter (" + listOnEnter.count + ")");
                EditorGUI.indentLevel--;
                if (OnEnter)
                {
                    listOnEnter.DoLayoutList();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(MalbersEditor.Style(MalbersEditor.MA_LightGray));
                EditorGUI.indentLevel++;
                if (listOnExit.count > 0) OnExit = true;
                OnExit = EditorGUILayout.Foldout(OnExit, "On Exit ("+listOnExit.count+")");
                EditorGUI.indentLevel--;
                if (OnExit)
                {
                    listOnExit.DoLayoutList();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(MalbersEditor.Style(MalbersEditor.MA_LightGray));
                EditorGUI.indentLevel++;
                if (listOnTime.count > 0) OnTime = true;
                OnTime = EditorGUILayout.Foldout(OnTime, "On Time (" + listOnTime.count + ")");
                EditorGUI.indentLevel--;
                if (OnTime)
                {
                    listOnTime.DoLayoutList();
                }

               
                EditorGUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                }


            }
          
            EditorGUILayout.EndVertical();


            serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// Reordable List Header
        /// </summary>
        void HeaderCallbackDelegate1(Rect rect)
        {
            Rect R_1 = new Rect(rect.x, rect.y, (rect.width / 3), EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_1, "Message");

            Rect R_3 = new Rect(rect.x + ((rect.width) / 3) + 5, rect.y, ((rect.width) / 3) - 5, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_3, "Type");

            Rect R_5 = new Rect(rect.x + ((rect.width) / 3) * 2 + 5, rect.y, ((rect.width) / 3) - 5, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_5, "Value");
        }

        void HeaderCallbackDelegate2(Rect rect)
        {
            Rect R_1 = new Rect(rect.x, rect.y, (rect.width / 4), EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_1, "Message");

            Rect R_3 = new Rect(rect.x + ((rect.width) / 4) + 5, rect.y, ((rect.width) / 4) - 5, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_3, "Type");
            Rect R_4 = new Rect(rect.x + ((rect.width) / 4) * 2 + 5, rect.y, ((rect.width) / 4) - 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(R_4, "Time");
            Rect R_5 = new Rect(rect.x + ((rect.width) / 4) * 3 + 5, rect.y, ((rect.width) / 4) - 5, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_5, "Value");
           
        }

        void drawElementCallback1(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = MMessage.onEnterMessage[index];
            rect.y += 2;

            Rect R_1 = new Rect(rect.x, rect.y, (rect.width / 3), EditorGUIUtility.singleLineHeight);
            element.message = EditorGUI.TextField(R_1, element.message);

            Rect R_3 = new Rect(rect.x + ((rect.width) / 3) + 5, rect.y, ((rect.width) / 3) - 5, EditorGUIUtility.singleLineHeight);
            element.typeM = (TypeMessage)EditorGUI.EnumPopup(R_3, element.typeM);

            Rect R_5 = new Rect(rect.x + ((rect.width) / 3) * 2 + 5, rect.y, ((rect.width) / 3) - 5, EditorGUIUtility.singleLineHeight);
            switch (element.typeM)
            {
                case TypeMessage.Bool:
                    element.boolValue = EditorGUI.ToggleLeft(R_5, element.boolValue ? " True":" False", element.boolValue);
                    break;
                case TypeMessage.Int:
                    element.intValue = EditorGUI.IntField(R_5, element.intValue);
                    break;
                case TypeMessage.Float:
                    element.floatValue= EditorGUI.FloatField(R_5, element.floatValue);
                    break;
                default:
                    break;
            }
            
        }

        void drawElementCallback2(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = MMessage.onExitMessage[index];
            rect.y += 2;

            Rect R_1 = new Rect(rect.x, rect.y, (rect.width / 3), EditorGUIUtility.singleLineHeight);
            element.message = EditorGUI.TextField(R_1, element.message);

            Rect R_3 = new Rect(rect.x + ((rect.width) / 3) + 5, rect.y, ((rect.width) / 3) - 5, EditorGUIUtility.singleLineHeight);
            element.typeM = (TypeMessage)EditorGUI.EnumPopup(R_3, element.typeM);

            Rect R_5 = new Rect(rect.x + ((rect.width) / 3) * 2 + 5, rect.y, ((rect.width) / 3) - 5, EditorGUIUtility.singleLineHeight);
            switch (element.typeM)
            {
                case TypeMessage.Bool:
                    element.boolValue = EditorGUI.ToggleLeft(R_5, element.boolValue ? " True" : " False", element.boolValue);
                    break;
                case TypeMessage.Int:
                    element.intValue = EditorGUI.IntField(R_5, element.intValue);
                    break;
                case TypeMessage.Float:
                    element.floatValue = EditorGUI.FloatField(R_5, element.floatValue);
                    break;
                default:
                    break;
            }

        }

        void drawElementCallback3(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = MMessage.onTimeMessage[index];
            rect.y += 2;

            Rect R_1 = new Rect(rect.x, rect.y, (rect.width / 4), EditorGUIUtility.singleLineHeight);
            element.message = EditorGUI.TextField(R_1, element.message);

            Rect R_3 = new Rect(rect.x + ((rect.width) / 4) + 5, rect.y, ((rect.width) / 4) - 5, EditorGUIUtility.singleLineHeight);
            element.typeM = (TypeMessage)EditorGUI.EnumPopup(R_3, element.typeM);

            Rect R_4 = new Rect(rect.x + ((rect.width) / 4) *2 + 5, rect.y, ((rect.width) / 4) - 5, EditorGUIUtility.singleLineHeight);

            element.time = EditorGUI.FloatField(R_4, element.time);

            if (element.time > 1) element.time = 1;
            if (element.time < 0) element.time = 0;

            Rect R_5 = new Rect(rect.x + ((rect.width) / 4) * 3 + 5, rect.y, ((rect.width) / 4) - 5, EditorGUIUtility.singleLineHeight);
            switch (element.typeM)
            {
                case TypeMessage.Bool:
                    element.boolValue = EditorGUI.ToggleLeft(R_5, element.boolValue ? " True" : " False", element.boolValue);
                    break;
                case TypeMessage.Int:
                    element.intValue = EditorGUI.IntField(R_5, element.intValue);
                    break;
                case TypeMessage.Float:
                    element.floatValue = EditorGUI.FloatField(R_5, element.floatValue);
                    break;
                default:
                    break;
            }

        }

    }
}