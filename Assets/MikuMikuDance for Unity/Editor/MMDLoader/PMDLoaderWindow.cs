using UnityEngine;
using System.Collections;
using UnityEditor;
using MMD.PMD;

public class PMDLoaderWindow : EditorWindow {
	Object pmdFile = null;
	bool rigidFlag = true;
	bool use_mecanim = true;
	ShaderType shader_type = ShaderType.MMDShader;

	bool use_ik = true;

	[MenuItem("Plugins/MMD Loader/PMD Loader")]
	static void Init() {        
        var window = (PMDLoaderWindow)EditorWindow.GetWindow<PMDLoaderWindow>(true, "PMDLoader");
		window.Show();
	}

    public PMDLoaderWindow()
    {
        // デフォルトコンフィグ
        var config = MMD.Config.LoadAndCreate();
        shader_type = config.pmd_config.shader_type;
        rigidFlag = config.pmd_config.rigidFlag;
        use_mecanim = config.pmd_config.use_mecanim;
        use_ik = config.pmd_config.use_ik; 
    }
	
	void OnGUI() {
		const int height = 20;
		int width = (int)position.width;// -16;
		
		pmdFile = EditorGUI.ObjectField(
			new Rect(0, 0, width, height), "PMD File" , pmdFile, typeof(Object), false);
		
		// シェーダの種類
		shader_type = (ShaderType)EditorGUI.EnumPopup(new Rect(0, height, width, height), "Shader Type", shader_type);

		// 剛体を入れるかどうか
		rigidFlag = EditorGUI.Toggle(new Rect(0, height * 2, width / 2, height), "Rigidbody", rigidFlag);

		// Mecanimを使うかどうか
		use_mecanim = false; // EditorGUI.Toggle(new Rect(0, height * 3, width / 2, height), "Use Mecanim", use_mecanim);

		// IKを使うかどうか
		use_ik = EditorGUI.Toggle(new Rect(0, height * 4, width / 2, height), "Use IK", use_ik);
		
		int buttonHeight = height * 5;
		if (pmdFile != null) {
			if (GUI.Button(new Rect(0, buttonHeight, width / 2, height), "Convert")) {
				var loader = new PMDLoaderScript(pmdFile, shader_type, rigidFlag, use_mecanim, use_ik);
                
                // 読み込み完了メッセージ
                var window = LoadedWindow.Init();
                window.Text = string.Format(
                  "----- model name -----\n{0}\n\n----- comment -----\n{1}",
                  loader.format.head.model_name,
                  loader.format.head.comment
                );
                window.Show();
                
                pmdFile = null;		// 読み終わったので空にする 
			}
		} else {
			EditorGUI.LabelField(new Rect(0, buttonHeight, width, height), "Missing", "Select PMD File");
		}
	}
}
