using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Cucurbit.Editor
{
    public class VersionEditorWindow : EditorWindow
    {        
        [MenuItem("Cucurbit/Version Controller", false, 1000)]
        public static void Open()
        {
            GetWindow(typeof(VersionEditorWindow), false, "Version Editor Window");
        }

        string originBundleVersion = "";

        int iOSBuildNumber = 0;
        int androidBuildNumber = 0;
        string commonBundleVersion = "0.0.0";

        int iOSBuildNumberNext = 0;
        int androidBuildNumberNext = 0;
        string commonBundleVersionNext = "0.0.0";

        Vector2 scrollPos = Vector2.zero;

        const int limitPoint = 5;
        string[] buttonTitles = new string[limitPoint] {
            "Major",
            "Minor",
            "Point",
            "MPoint",
            "MMPoint",
        };

        private void OnEnable()
        {
            iOSBuildNumber = 0;
            int.TryParse(PlayerSettings.iOS.buildNumber, out iOSBuildNumber);
            iOSBuildNumberNext = iOSBuildNumber;

            androidBuildNumber = PlayerSettings.Android.bundleVersionCode;
            androidBuildNumberNext = androidBuildNumber;

            commonBundleVersion = PlayerSettings.bundleVersion;
            string digitLimitVersion = "";
            if(commonBundleVersion.Length <= 0) {
                digitLimitVersion = "0.0.0";
            }
            else {
                var vsBase = commonBundleVersion.Split('.');
                var limit = Mathf.Min(limitPoint, vsBase.Length);
                for (int i = 0; i < limit; i++) {
                    digitLimitVersion += int.Parse(vsBase[i]) + (i + 1 < limit ? "." : "");
                }
            }
            originBundleVersion = digitLimitVersion;
            commonBundleVersion = originBundleVersion;
            commonBundleVersionNext = originBundleVersion;
        }

        private void OnGUI()
        {
            Color defaultColor = GUI.color;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Common Settings
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Common Settings");
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.TextField("Bundle Version:", commonBundleVersion);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(155);
                        EditorGUILayout.LabelField("↓");
                    }
                    EditorGUILayout.EndHorizontal();

                    if (commonBundleVersionNext != commonBundleVersion) {
                        GUI.color = Color.green;
                    }
                    EditorGUILayout.TextField("Bundle Version:", commonBundleVersionNext);
                    GUI.color = defaultColor;
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Change Point:", GUILayout.Width(150));

                    var points = commonBundleVersion.Split('.');
                    if (GUILayout.Button("Dec")) {
                        if (1 < points.Length) {
                            commonBundleVersion = commonBundleVersion.Remove(commonBundleVersion.LastIndexOf("."));
                            commonBundleVersionNext = commonBundleVersionNext.Remove(commonBundleVersionNext.LastIndexOf("."));
                        }
                    }
                    if (GUILayout.Button("Add")) {
                        if (points.Length < limitPoint) {
                            commonBundleVersion += ".0";
                            commonBundleVersionNext += ".0";
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(3);

            var vsBase = commonBundleVersion.Split('.');
            var vsNext = commonBundleVersionNext.Split('.');

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Increment:", GUILayout.Width(80));
                    for (int i = 0; i < vsNext.Length; i++) {
                        if (GUILayout.Button(buttonTitles[i])) {
                            var point = int.Parse(vsNext[i]) + 1;
                            vsNext[i] = point.ToString();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Decrement:", GUILayout.Width(80));
                    for (int i = 0; i < vsNext.Length; i++) {
                        if (GUILayout.Button(buttonTitles[i])) {
                            if (int.Parse(vsNext[i]) - 1 >= int.Parse(vsBase[i])) {
                                var point = int.Parse(vsNext[i]) - 1;
                                vsNext[i] = point.ToString();
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            commonBundleVersionNext = string.Join(".", vsNext);

            // Android Build
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Android Build Settings");
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.IntField("Bundle Version:", androidBuildNumber);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(155);
                        EditorGUILayout.LabelField("↓");
                    }
                    EditorGUILayout.EndHorizontal();

                    if (androidBuildNumberNext != androidBuildNumber) {
                        GUI.color = Color.green;
                    }
                    EditorGUILayout.IntField("Bundle Version:", androidBuildNumberNext);
                    GUI.color = defaultColor;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            // iOS Build
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("iOS Build Settings");
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.IntField("Build Number:", iOSBuildNumber);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(155);
                        EditorGUILayout.LabelField("↓");
                    }
                    EditorGUILayout.EndHorizontal();

                    if (iOSBuildNumberNext != iOSBuildNumber) {
                        GUI.color = Color.green;
                    }
                    EditorGUILayout.IntField("Build Number:", iOSBuildNumberNext);
                    GUI.color = defaultColor;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Increment:", GUILayout.Width(80));
                    if (GUILayout.Button("Android")) {
                        androidBuildNumberNext++;
                    }
                    if (GUILayout.Button("iOS")) {
                        iOSBuildNumberNext++;
                    }
                    if (GUILayout.Button("Both")) {
                        androidBuildNumberNext++;
                        iOSBuildNumberNext++;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Decrement:", GUILayout.Width(80));
                    if (GUILayout.Button("Android")) {
                        if (androidBuildNumberNext - 1 >= androidBuildNumber) {
                            androidBuildNumberNext--;
                        }
                    }
                    if (GUILayout.Button("iOS")) {
                        if (iOSBuildNumberNext - 1 >= iOSBuildNumber) {
                            iOSBuildNumberNext--;
                        }
                    }
                    if (GUILayout.Button("Both")) {
                        if (androidBuildNumberNext - 1 >= androidBuildNumber) {
                            androidBuildNumberNext--;
                        }
                        if (iOSBuildNumberNext - 1 >= iOSBuildNumber) {
                            iOSBuildNumberNext--;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            // Save/Reset
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save", GUILayout.Height(30))) {

                    PlayerSettings.iOS.buildNumber = iOSBuildNumberNext.ToString();
                    PlayerSettings.Android.bundleVersionCode = androidBuildNumberNext;
                    PlayerSettings.bundleVersion = commonBundleVersionNext;
                }
                if (GUILayout.Button("Reset", GUILayout.Height(30))) {
                    iOSBuildNumberNext = iOSBuildNumber;
                    androidBuildNumberNext = androidBuildNumber;
                    commonBundleVersion = originBundleVersion;
                    commonBundleVersionNext = originBundleVersion;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndScrollView();
        }
    }
}