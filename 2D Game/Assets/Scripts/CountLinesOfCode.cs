
// CHECK OUT COUNTRYBALLS THE HEIST ON STEAM PLS :D !! 
// HERE IS THE LINK: https://store.steampowered.com/app/1986290/Countryballs_The_Heist/
// MEANS A LOT!
// CHEERS MATE

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CountLinesOfCode : EditorWindow
{
    private SerializedObject serializedObject;
    private SerializedProperty foldersToIgnoreProperty;

    public List<string> foldersToIgnore = new List<string>();

    [MenuItem("Tools/Count Lines of Code")]
    public static void ShowWindow()
    {
        GetWindow<CountLinesOfCode>("Count Lines of Code");
    }

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        foldersToIgnoreProperty = serializedObject.FindProperty("foldersToIgnore");
    }

    private void OnGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Folders to Ignore (relative to Assets):");
        EditorGUILayout.PropertyField(foldersToIgnoreProperty, true);

        if (GUILayout.Button("Count Lines of Code"))
        {
            CountLines();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void CountLines()
    {
        string assetsPath = Application.dataPath;
        string[] allScripts = Directory.GetFiles(assetsPath, "*.cs", SearchOption.AllDirectories);
        List<string> foldersToIgnorePaths = new List<string>();

        foreach (string folder in foldersToIgnore)
        {
            string folderPath = Path.Combine(assetsPath, folder);
            if (Directory.Exists(folderPath))
            {
                foldersToIgnorePaths.Add(folderPath);
            }
        }

        int totalLines = 0;

        foreach (string script in allScripts)
        {
            bool ignore = false;

            foreach (string folderPath in foldersToIgnorePaths)
            {
                if (script.StartsWith(folderPath))
                {
                    ignore = true;
                    break;
                }
            }

            if (!ignore)
            {
                string[] lines = File.ReadAllLines(script);
                totalLines += lines.Length;
            }
        }

        Debug.Log("Total lines of code: " + totalLines);
    }
}
