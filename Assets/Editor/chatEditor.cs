using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(SaySomething))]
public class chatEditor : Editor
{

    string text;

    SaySomething saysmth = null;

    void OnEnable()
    {
        saysmth = (SaySomething)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        text = EditorGUILayout.TextArea(text);

        if (GUILayout.Button("보내기", GUILayout.Width(60)) && text.Trim() != "")
        {
            saysmth.say(text);
            text = "";
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();

    }
}





