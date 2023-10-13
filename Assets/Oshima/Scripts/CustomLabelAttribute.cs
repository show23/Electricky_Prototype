/*
 * インスペクタに表示される変数名を好きな文字に置き換えるカスタムエディタ
 * CustomLabelAttribute.cs : Ver. 1.0.2
 * Written by Takashi Sowa with ChatGPT
 * ▼使い方：以下のように記述すればインスペクタに表示される「variable」が「変数名」に置き換わる
 * [CustomLabel("変数名")]
 * public int variable = 0;//[SerializeField]でも利用可
*/

using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;

//カスタムのラベルを指定する為の属性で、この属性をフィールドに追加することでそのフィールドのインスペクタ上の表示名を変更することができる
[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
public class CustomLabelAttribute : PropertyAttribute{
	public string label;
	public CustomLabelAttribute(string label){
		this.label = label;
	}
}

//MonoBehaviourやその派生クラスのカスタムエディタとして機能させる
[CustomEditor(typeof(MonoBehaviour), true)]
public class CustomInspectorEditor : Editor{
	private FieldInfo[] serializedFields;
	private FieldInfo[] publicFields;

	//カスタムエディタが有効になったときに呼び出される
	//[SerializeField]とpublicを取得してそれぞれの情報を保持する
	private void OnEnable(){
		serializedFields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(f => f.GetCustomAttributes(typeof(SerializeField), true).Length > 0).ToArray();
		publicFields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
	}

	//インスペクタのGUIを描画する
	//シリアライズオブジェクトを更新し「DrawDefaultInspectorWithCustomLabel()」を呼び出してデフォルトのフィールド名を描画、変更を適用する
	public override void OnInspectorGUI(){
		DrawScriptLink();
		serializedObject.Update();
		DrawDefaultInspectorWithCustomLabel();
		serializedObject.ApplyModifiedProperties();
	}

	//インスペクタ上でスクリプトのリンクをグレーアウト表示する
	private void DrawScriptLink(){
		MonoScript script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);
		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.Space();
	}

	//カスタムラベルを考慮しながらフィールド名を描画する
	//フィールドごとにループを実行し「CustomLabelAttribute」が追加されている場合はそのカスタムラベルでフィールド名を描画、それ以外の[SerializeField]かpublicの場合はそのまま描画する
	private void DrawDefaultInspectorWithCustomLabel(){
		var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach(var field in fields){
			var customLabelAttributes = field.GetCustomAttributes(typeof(CustomLabelAttribute), false) as CustomLabelAttribute[];
			if(customLabelAttributes.Length > 0){
				var customLabel = customLabelAttributes[0];
				var serializedProperty = serializedObject.FindProperty(field.Name);
				if(serializedProperty != null){
					EditorGUILayout.PropertyField(serializedProperty, new GUIContent(customLabel.label), true);
				}
			}else if(serializedFields.Contains(field) || publicFields.Contains(field)){
				var serializedProperty = serializedObject.FindProperty(field.Name);
				if(serializedProperty != null){
					EditorGUILayout.PropertyField(serializedProperty, true);
				}
			}
		}
	}
}

//ScriptableObjectクラスにも同じカスタムエディタを機能させる
[CustomEditor(typeof(ScriptableObject), true)]
public class CustomInspectorEditorForScriptableObject : CustomInspectorEditor{}