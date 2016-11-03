using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[Serializable]
[CreateAssetMenu(fileName = "Character Metadata", menuName = "Create/Character", order = 1)]
public class CharacterScriptable : ScriptableObject {

	public GameObject gameObject;

	public int id;
	public string playerName;
	public int hp;
	public int maxDamage;
	public int minDamage;
	public int displacement = 0;

	public int kindPos;
	public string kind;

	public int vectorX = 0;
	public int vectorZ = 0;
	public Vector3 position;
}
