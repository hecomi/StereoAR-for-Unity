using UnityEngine;
using UnityEditor;
using System.Collections;
using MMD.PMD;
using System.IO;

namespace MMD
{
    public class VMDInspector : Editor
    {
        // VMD Load option
        public bool createAnimationFile;
        public int interpolationQuality;
        public GameObject pmdPrefab;

        // last selected item
        private static string vmd_path = "";
        private static MMD.VMD.VMDFormat.Header vmd_header;
        private static string message = "";

        /// <summary>
        /// 選択されているオブジェクトがVMDファイルかチェックします
        /// </summary>
        /// <returns>VMDファイルであればそのパスを、異なればnullを返します。</returns>
        void setup()
        {
            var t = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (vmd_path != t)
            {
                if (!File.Exists(t)) return;

                // デフォルトコンフィグ
                var config = MMD.Config.LoadAndCreate();
                createAnimationFile = config.vmd_config.createAnimationFile;
                interpolationQuality = config.vmd_config.interpolationQuality;

                // モデル情報
                vmd_path = t;
                if (config.inspector_config.use_vmd_preload)
                {
                    using (var fs = new FileStream(vmd_path, FileMode.Open, FileAccess.Read))
                    using (var bin = new BinaryReader(fs))
                    {
                        vmd_header = new MMD.VMD.VMDFormat.Header(bin);
                    }
                }
                else
                {
                    vmd_header = null;
                }
            }
        }

        /// <summary>
        /// Inspector上のGUI描画処理を行います
        /// </summary>
        public override void OnInspectorGUI()
        {
            setup();

            // GUIの有効化
            GUI.enabled = true;

            pmdPrefab = EditorGUILayout.ObjectField("PMD Prefab", pmdPrefab, typeof(Object), false) as GameObject;
            createAnimationFile = EditorGUILayout.Toggle("Create Asset", createAnimationFile);
            interpolationQuality = EditorGUILayout.IntSlider("Interpolation Quality", interpolationQuality, 1, 10);

            // Convertボタン
            EditorGUILayout.Space();
            if (message.Length != 0)
            {
                GUILayout.Label(message);
            }
            else
            {
                if (GUILayout.Button("Convert"))
                {
                    new VMDLoaderScript(target, pmdPrefab, createAnimationFile, interpolationQuality);
                    message = "Loading done.";
                }
            }
            GUILayout.Space(40);

            // モデル情報
            if (vmd_header == null) return;
            EditorGUILayout.LabelField("Model Name");
            GUI.enabled = false;
            EditorGUILayout.TextArea(vmd_header.vmd_model_name);
            GUI.enabled = true;
        }
    }
}
