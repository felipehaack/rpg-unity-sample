using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBase: MonoBehaviour {

	[System.NonSerialized]
	public int cubeLayer;

	[System.NonSerialized]
	public int cubeLayerNumber = 8;

	[System.NonSerialized]
	public float sleepTime = 0.01f;

	[System.NonSerialized]
	public int playerLayer;

	[System.NonSerialized]
	public int playerLayerNumber = 9;

	[System.NonSerialized]
	public Transform transformOverlap;

	[System.NonSerialized]
	public Transform transformMouseOver;

	public T[] shuffleArray<T>(T[] arr) {

		for (int i = arr.Length - 1; i > 0; i--) {

			int r = Random.Range(0, i + 1);

			T tmp = arr[i];
			arr[i] = arr[r];
			arr[r] = tmp;
		}

		return arr;
	}

	public List<GameObject> adjustCubeArray(Collider[] colliders, GameObject[] players){

		List<GameObject> result = new List<GameObject> ();

		if (colliders.Length > 0 && players.Length > 0) {

			for (int i = 0; i < colliders.Length; ++i)
				result.Add (colliders [i].gameObject);

			for (int i = 0; i < players.Length; ++i) {

				for (int j = 0; j < result.Count; ++j) {

					if (players [i].transform.position.z == result [j].transform.position.z
						&& result [j].transform.position.x == players [i].transform.position.x) {

						result.Remove (result [j]);

						break;
					}
				}
			}
		}

		return result;
	}

	public void mouseMoveAction(){

		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast (ray, out hit, 100, cubeLayer)) {

			Transform localOverlap = findRectangleOverlap(hit.transform);
			Transform localMouseOver = findRectangleMouseOver(hit.transform);

			if (localOverlap.transform.localScale.z > 0) {

				if (transformOverlap != null && !localOverlap.transform.Equals (transformOverlap.transform)) {

					hideRectangle (localOverlap);
					showRectangle (localMouseOver);

					showRectangle (transformOverlap);
					hideRectangle (transformMouseOver);
				} else {

					if (transformOverlap == null) {

						hideRectangle (localOverlap);
						showRectangle (localMouseOver);
					}
				}

				transformOverlap = localOverlap;
				transformMouseOver = localMouseOver;
			}
		}
	}

	public void showRectangle(Transform transform){

		Animator animator = transform.GetComponent<Animator> ();
		animator.ResetTrigger ("Hide");
		animator.SetTrigger ("Show");
	}

	public void hideRectangle(Transform transform){

		Animator animator = transform.GetComponent<Animator> ();
		animator.ResetTrigger ("Show");
		animator.SetTrigger("Hide");
	}

	public Transform findRectangleOverlap(Transform transform){

		return transform.FindChild ("RectangleOverlap");
	}

	public Transform findRectangleMouseOver(Transform transform){

		return transform.FindChild ("RectangleMouseOver");
	}
}
