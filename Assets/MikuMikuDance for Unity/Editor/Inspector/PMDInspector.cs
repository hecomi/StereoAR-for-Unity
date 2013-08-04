using UnityEngine;
using UnityEditor;
using System.Collections;
using MMD.PMD;
using System.IO;

namespace MMD
{
    public class PMDInspector : Editor
    {
        // PMD Load option
        public ShaderType shader_type;
        public bool rigidFlag;
        public bool use_mecanim;
        public bool use_ik;

        // last selected item
        private static string pmd_path = "";
        private static MMD.PMD.PMDFormat.Header pmd_header;
        private static string message = "";

        /// <summary>
        /// pmd_headerとデフォルトコンフィグの設定
        /// </summary>
        private void setup()
        {
            var t = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (pmd_path != t)
            {
                if (!File.Exists(t)) return;
                var config = MMD.Config.LoadAndCreate();

                // デフォルトコンフィグ
                shader_type = config.pmd_config.shader_type;
                rigidFlag = config.pmd_config.rigidFlag;
                use_mecanim = config.pmd_config.use_mecanim;
                use_ik = config.pmd_config.use_ik;

                // モデル情報
                pmd_path = t;
                if (config.inspector_config.use_pmd_preload)
                {
                    using (var fs = new FileStream(pmd_path, FileMode.Open, FileAccess.Read))
                    using (var bin = new BinaryReader(fs))
                    {
                        pmd_header = new MMD.PMD.PMDFormat.Header(bin);
                    }
                }
                else
                {
                    pmd_header = null;
                }
            }
            if (EditorApplication.isPlaying) pmd_path = "";
        }

        /// <summary>
        /// Inspector上のGUI描画処理を行います
        /// </summary>
        public override void OnInspectorGUI()
        {
            setup();

            // GUIの有効化
            GUI.enabled = !EditorApplication.isPlaying;

            // シェーダの種類
            shader_type = (ShaderType)EditorGUILayout.EnumPopup("Shader Type", shader_type);

            // 剛体を入れるかどうか
            rigidFlag = EditorGUILayout.Toggle("Rigidbody", rigidFlag);

            // Mecanimを使うかどうか
            use_mecanim = EditorGUILayout.Toggle("Use Mecanim (not work)", use_mecanim);

            // IKを使うかどうか
            use_ik = EditorGUILayout.Toggle("Use IK", use_ik);

            // Convertボタン
            EditorGUILayout.Space();
            if (message.Length != 0)
            {
                GUILayout.Label(message);
            }
            else
            {
                if (GUILayout.Button("Convert to Prefab"))
                {
                    new PMDLoaderScript(target, shader_type, rigidFlag, use_mecanim, use_ik);
                    message = "Loading done.";
                }
            }
            GUILayout.Space(40);

            // モデル情報
            if (pmd_header == null) return;
            EditorGUILayout.LabelField("Model Name");
            GUI.enabled = false;
            EditorGUILayout.TextArea(pmd_header.model_name);
            GUI.enabled = true;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Comment");
            GUI.enabled = false;
            EditorGUILayout.TextArea(pmd_header.comment, GUILayout.Height(300));
            GUI.enabled = true;
        }
    }
}