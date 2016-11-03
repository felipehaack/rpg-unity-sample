using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CharacterScriptable))]
public class CharactorEditor : Editor {
	
	private string[] kinds = new string[]{ "Allied" , "Enemy" };

	public override void OnInspectorGUI(){

		CharacterScriptable character = target as CharacterScriptable;

		character.id = character.GetInstanceID ();

		EditorGUILayout.LabelField ("Character ID: " + character.id.ToString ());

		character.playerName = EditorGUILayout.TextField ("Character Name:", character.playerName);

		character.vectorX = EditorGUILayout.IntSlider ("Position in X:", character.vectorX, 0, Generator.max, null);
		character.vectorZ = EditorGUILayout.IntSlider ("Position in Z:", character.vectorZ, 0, Generator.max, null);

		character.position = new Vector3 (character.vectorX, 0, character.vectorZ);

		EditorGUILayout.LabelField ("Character Goal:");
		character.kindPos = EditorGUILayout.Popup (character.kindPos, kinds);

		character.gameObject = EditorGUILayout.ObjectField ("Character Class:", character.gameObject, typeof(GameObject), false) as GameObject;

		character.hp = EditorGUILayout.IntField ("Custom HP: ", character.hp);
		character.maxDamage = EditorGUILayout.IntField ("Custom Max Damage: ", character.maxDamage);
		character.minDamage = EditorGUILayout.IntField ("Custom Min Damage: ", character.minDamage);
		character.displacement = EditorGUILayout.IntField ("Custom Displacement: ", character.displacement);
	}
}
