using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class PMDLoaderScript {

	//--------------------------------------------------------------------------------
	// ファイル読み込み
	
	public Object pmd;
	public bool rigidFlag;		// 物理や剛体のオン・オフ
	public MMD.PMD.ShaderType shader_type;
	public bool use_mecanim;
	public bool use_ik;
    public MMD.PMD.PMDFormat format;

	FileStream fst;		// テスト用

	BinaryReader LoadFile(Object obj, string path) {
		FileStream f = new FileStream(path, FileMode.Open, FileAccess.Read);
		this.fst = f;
		BinaryReader r = new BinaryReader(f);
		return r;
	}
		
	// PMDファイル読み込み
	void LoadPMDFile() {
		string path = AssetDatabase.GetAssetPath(this.pmd);
		BinaryReader bin = this.LoadFile(this.pmd, path);
		this.format = MMD.PMD.PMDLoader.Load(bin, null, path);
		BurnUnityFormatForPMD();
		bin.Close();
	}
		
	// Use this for initialization
	public PMDLoaderScript (Object pmdFile, MMD.PMD.ShaderType shader_type, bool rigidFlag, bool use_mecanim, bool use_ik) {
		this.pmd = pmdFile;
		this.rigidFlag = rigidFlag;
		this.shader_type = shader_type;
		this.use_mecanim = use_mecanim;
		this.use_ik = use_ik;

		if (this.pmd != null) {
			LoadPMDFile();
		}
	}

	//--------------------------------------------------------------------------------
	// PMDファイルの読み込み
	
	// PMDファイルをUnity形式に変換
	void BurnUnityFormatForPMD() {
        
        MMD.PMD.PMDConverter conv = new MMD.PMD.PMDConverter();

        GameObject obj = new GameObject(this.format.name);
        this.format.fst = this.fst;
        this.format.caller = obj;
        this.format.shader_type = this.shader_type;

        Mesh mesh = conv.CreateMesh(this.format);                   // メッシュの生成・設定
        Material[] materials = conv.CreateMaterials(this.format);   // マテリアルの生成・設定
        GameObject[] bones = conv.CreateBones(this.format);         // ボーンの生成・設定

		// バインドポーズの作成
        conv.BuildingBindpose(this.format, mesh, materials, bones);
		obj.AddComponent<Animation>();	// アニメーションを追加

		MMDEngine engine = obj.AddComponent<MMDEngine>();

		// IKの登録
        if (this.use_ik)
            engine.ik_list = conv.EntryIKSolver(this.format, bones);

		// 剛体関連
		if (this.rigidFlag)
		{
			try
			{
                var rigids = conv.CreateRigids(this.format, bones);
                conv.AssignRigidbodyToBone(this.format, bones, rigids);
                conv.SetRigidsSettings(this.format, bones, rigids);
                conv.SettingJointComponent(this.format, bones, rigids);

				// 非衝突グループ
                List<int>[] ignoreGroups = conv.SettingIgnoreRigidGroups(this.format, rigids);
                int[] groupTarget = conv.GetRigidbodyGroupTargets(this.format, rigids);

				MMDEngine.Initialize(engine, groupTarget, ignoreGroups, rigids);
			}
			catch { }
		}

        // Mecanim設定 (not work yet..)
#if UNITY_4_0 || UNITY_4_1
        AvatarSettingScript avt_setting = new AvatarSettingScript(this.format.caller);
		avt_setting.SettingAvatar();
#endif

        // プレファブに登録
        Object prefab = PrefabUtility.CreateEmptyPrefab(this.format.folder + "/" + format.name + ".prefab");
        PrefabUtility.ReplacePrefab(this.format.caller, prefab);
        
        // アセットリストの更新
        AssetDatabase.Refresh();

		// 一度，表示されているモデルを削除して新しくPrefabのインスタンスを作る
		GameObject.DestroyImmediate(obj);
        PrefabUtility.InstantiatePrefab(prefab);
	}
}
