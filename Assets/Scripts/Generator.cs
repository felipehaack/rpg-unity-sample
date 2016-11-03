using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Generator
{

	public static int max = 10;
	public static float scaleY = 0.58f;

	[MenuItem("Custom Tools/Generate Plane")]
	static void createGeneratePlane()
	{

		GameObject plane = new GameObject ();
		plane.name = "Cubes";

		GameObject cube = Resources.Load("prefabs/Cube") as GameObject;

		for (int x = 0; x <= max; ++x)
		{
			for (int z = 0; z <= max; ++z) {

				GameObject prefab = PrefabUtility.InstantiatePrefab (cube) as GameObject;
				prefab.transform.position = new Vector3 (z, 0, x);
				prefab.transform.localScale = new Vector3 (1, scaleY, 1);

				prefab.transform.parent = plane.transform;
			}
		}
	}

	[MenuItem("Custom Tools/Generate Players")]
	static void createGeneratePlayers(){

		foreach (CharacterScriptable player in Resources.FindObjectsOfTypeAll(typeof(CharacterScriptable)) as CharacterScriptable[]) {

			GameObject prefab = PrefabUtility.InstantiatePrefab (player.gameObject) as GameObject;

			if (prefab != null) {

				player.position.y = scaleY;
				player.gameObject = prefab;
				prefab.transform.position = player.position;

				Character c = prefab.GetComponent<Character> ();
				c.cName = player.playerName;

				if (player.hp > 0)
					c.cHp = player.hp;

				if (player.maxDamage > 0)
					c.cMaxDamage = player.maxDamage;

				if (player.minDamage > 0)
					c.cMinDamage = player.minDamage;

				c.kind = player.kindPos;
				c.displacement = player.displacement;
			} else {

				Debug.LogError ("No prefab was selected");
			}
		}
	}
}