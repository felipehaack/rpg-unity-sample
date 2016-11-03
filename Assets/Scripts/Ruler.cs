using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Ruler : MonoBehaviour {

	public Text textHP;
	public Text textName;

	public GameObject panelInfo;
	public GameObject panelAction;

	enum Actions {
		SearchPlayer,
		SelectedPlayer
	};

	private Actions action;
	private int playerLayer;
	private int playerLayerNumber = 9;

	//Save Player Config
	private GameObject oldSearchPlayer;
	private GameObject currentGameObject;

	//Control Player
	private GameObject[] allPlayers;
	private List<GameObject> excludePlayer;
	private int currentKind = 0;
	private int alliedCount = 0;
	private int enemyCount = 0;

	void Start () {
	
		playerLayer = 1 << playerLayerNumber;
		action = Actions.SearchPlayer;

		excludePlayer = new List<GameObject> ();
		allPlayers = GameObject.FindGameObjectsWithTag ("Player");
		adjustEnemyAndAllied ();
	}

	private bool mouseButtoUp(){

		return Input.GetButtonUp ("Fire1");
	}

	private void disableOldSearchPlayer(GameObject current){

		if (oldSearchPlayer != null) {

			if (!oldSearchPlayer.transform.Equals (current.transform)) {

				oldSearchPlayer.GetComponent<Character> ().disableSphereEffect ();
			}
		}
	}

	void Update () {

		detectRightMouseClick ();

		switch (action) {

			case Actions.SearchPlayer: {
					
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				if (Physics.Raycast (ray, out hit, 100, playerLayer)) {

					Character c = hit.transform.GetComponent<Character> ();

					if (c.kind == currentKind && !excludePlayer.Contains(hit.transform.gameObject)) {

						c.enableSphereEffect ();

						currentGameObject = hit.transform.gameObject;

						disableOldSearchPlayer (currentGameObject);
						oldSearchPlayer = currentGameObject;

						if (mouseButtoUp ()) {

							c.play ();

							action = Actions.SelectedPlayer;
						}
					}else{

						Debug.Log("This player already make your move!");
					}
				}

				break;
			}
		}
	}

	public void setSearchPlayerEnum(){ 

		action = Actions.SearchPlayer;
	}

	private void detectRightMouseClick(){

		if (Input.GetButtonDown ("Fire2")) {

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit, 100, playerLayer)) {

				setCurrentPlayerStatus (hit.transform.gameObject);

				setPanelInfo (true);
			} else {

				setPanelInfo (false);
			}
		}
	}

	public void setCurrentPlayerStatus(GameObject gameObject){

		Character character = gameObject.GetComponent<Character> ();
		textName.text = character.cName;
		textHP.text = character.cHp.ToString();
		//textDamage.text = character.cMaxDamage.ToString();
	}

	public void setPanelInfo(bool flag){

		panelInfo.GetComponent<Animator> ().SetBool ("Toggle", flag);
	}

	public void setPanelAction(bool flag){

		panelAction.GetComponent<Animator> ().SetBool ("Toggle", flag);
	}

	private void adjustEnemyAndAllied(){

		for (int i = 0; i < allPlayers.Length; ++i) {

			Character c = allPlayers [i].GetComponent<Character> ();

			if (c.kind == 0)
				alliedCount++;
			else
				enemyCount++;
		}
	}

	public void removePlayer(GameObject gameObject, int kind, int hp){

		if (hp <= 0)
			Destroy (gameObject);

		if (kind == 0)
			alliedCount -= 1;

		if (kind == 1)
			enemyCount -= 1;
	}

	public void finishAction(){

		excludePlayer.Add (currentGameObject);

		if (currentKind == 0 && excludePlayer.Count >= alliedCount) {

			currentKind = 1;
			excludePlayer = new List<GameObject> ();
		}

		if (currentKind == 1 && excludePlayer.Count >= enemyCount) {

			currentKind = 0;
			excludePlayer = new List<GameObject> ();
		}
	}

	//Skills and Itens
	public void Heal(){

		currentGameObject.GetComponent<Character> ().Heal ();

		finishAction ();
	}

	public void Damage(){

		currentGameObject.GetComponent<Character> ().Damage ();
	}

	public void Pass(){

		currentGameObject.GetComponent<Character> ().Pass ();

		finishAction ();
	}
}
