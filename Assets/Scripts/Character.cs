using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character: CharacterBase {


	public string cName;
	public int cHp;
	public int cMaxDamage;
	public int cMinDamage;
	public int kind;
	public int displacement;

	private enum Actions {
		Move,
		Fight,
		None
	};
	private Actions action;

	//The Ruler
	private Ruler ruler;

	//Status
	private bool isCanPlay = false;
	//private bool isOverlap = false;
	//private bool isOverlapFight = false;

	//Cubos and dimension
	private List<GameObject> cubes;
	private int currentCubePosition = 0;

	//Cubos overlay - Move and Attack
	private Vector3 gizmosOverlap;
	private Vector3 moveOverlap;
	private Vector3 moveOverlapDefault = new Vector3(3, 1, 3);

	//Save moviments
	private Transform[] oldPositions;

	private enum ActionsKind {
		Attack,
		None
	};
	private ActionsKind actionKind;

	void Start () {

		cubeLayer = 1 << cubeLayerNumber;
		playerLayer = 1 << playerLayerNumber;

		ruler = FindObjectOfType<Ruler> ();

		if (displacement > 0) {

			moveOverlapDefault = new Vector3 (displacement, 1, displacement);
		}

		moveOverlap = moveOverlapDefault;

		setGizmosVector (moveOverlap);

		oldPositions = new Transform[2];
	}

	void Update(){

		if (isCanPlay) {

			detectEscPress ();

			switch (action) {

				case Actions.Move: {

					mouseMoveAction ();

					if (detectMouseButtonDown() && transformOverlap != null) {

						action = Actions.Fight;

						Vector3 parent = transformOverlap.parent.position;

						oldPositions [0] = transformOverlap.parent;
						oldPositions [1] = getCubeBellowWithRaycast (transform.position);

						cubes.Add (oldPositions [1].gameObject);

						Vector3 newOverlap = new Vector3 ();
						newOverlap.x = parent.x > transform.position.x ? parent.x - transform.position.x : transform.position.x - parent.x;
						newOverlap.z = parent.z > transform.position.z ? parent.z - transform.position.z : transform.position.z - parent.z;

						moveByPosition (oldPositions [0].position);

						hideRectangle (findRectangleOverlap(oldPositions[0]));
						hideRectangle (findRectangleMouseOver (oldPositions [0]));

						showRectangle (findRectangleOverlap (oldPositions [1]));
						hideRectangle (findRectangleMouseOver (oldPositions [1]));

						int x = (int) (parent.x > oldPositions [1].position.x ? parent.x - oldPositions [1].position.x : oldPositions [1].position.x - parent.x);
						int z = (int) (parent.z > oldPositions [1].position.z ? parent.z - oldPositions [1].position.z : oldPositions [1].position.z - parent.z);

						int distance = Mathf.Max (x, z);

						int newXZ = (int) moveOverlap.x - distance;

						if (newXZ == 0)
							newXZ = 1;
						
						moveOverlap = new Vector3 (newXZ, moveOverlap.y, newXZ);
						setGizmosVector (moveOverlap);

						uncastOverlap ();
						castOverlap ();

						ruler.setPanelAction (true);
					}

					break;
				}

				case Actions.Fight: {

					if (detectMouseButtonDown ()) {

						RaycastHit hit;
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

						switch (actionKind) {

							case ActionsKind.Attack: {

								if (Physics.Raycast (ray, out hit, 100, playerLayer)) {

									Character c = hit.transform.GetComponent<Character> ();

									if (c.kind != kind && (int) Vector3.Distance(transform.position, hit.transform.position) <= (int) moveOverlap.x) {

										c.cHp -= Random.Range(cMinDamage, cMaxDamage);

										if (c.cHp <= 0) {

											ruler.removePlayer (hit.transform.gameObject, c.kind, c.cHp);
										}
											
										ruler.finishAction ();
										if (c.cHp > 0) {
											ruler.setCurrentPlayerStatus (hit.transform.gameObject);
											ruler.setPanelInfo (true);
										}
										Invoke ("stop", 1);
									} else {

										Debug.Log ("Cant damage an allied or distance is not enough!");
									}
								}

								break;
							}
						}
					}

					break;
				}
			}
		}
	}

	void OnDrawGizmos(){

		if (isCanPlay) {

			Gizmos.color = Color.red;
			Gizmos.DrawWireCube (transform.position, gizmosOverlap);
		}
	}

	private void moveByPosition(Vector3 position){

		transform.position = new Vector3 (position.x, transform.position.y, position.z);
	}

	private Transform getCubeBellowWithRaycast(Vector3 position){

		RaycastHit hit;

		if (Physics.Raycast (position, Vector3.down, out hit, 2, cubeLayer)) {

			return hit.transform;
		} else {

			return null;
		}
	}

	private bool detectMouseButtonDown(){

		return Input.GetButtonUp ("Fire1");
	}

	private void detectEscPress(){

		if (Input.GetKeyDown (KeyCode.Escape)) {
			
			if (action == Actions.Move) {
				
				stop ();
			}

			if (action == Actions.Fight) {
				
				isCanPlay = false;

				transformOverlap = null;
				transformMouseOver = null;

				moveByPosition (oldPositions [1].position);
				moveOverlap = moveOverlapDefault;

				hideRectangle (findRectangleOverlap(oldPositions[0]));
				hideRectangle (findRectangleMouseOver (oldPositions [0]));

				hideRectangle (findRectangleOverlap (oldPositions [1]));
				hideRectangle (findRectangleMouseOver (oldPositions [1]));

				uncastOverlap ();
				castOverlap ();
				setGizmosVector (moveOverlap);

				Invoke ("setIsCanPlay", 1f);
				Invoke ("setActionMoveMode", 1f);

				ruler.setPanelAction (false);
			}
		}
	}

	private void setIsCanPlay(){

		isCanPlay = true;
	}

	private void setActionMoveMode(){

		action = Actions.Move;
	}

	private void setGizmosVector(Vector3 vector){

		gizmosOverlap = new Vector3(vector.x * 2, vector.y, vector.z * 2);
	}

	private void castOverlapRecursive(){

		if (currentCubePosition < cubes.Count) {
			
			Transform cube = findRectangleOverlap (cubes [currentCubePosition++].transform);

			if (cube != null) {
				
				showRectangle (cube);

				Invoke ("castOverlapRecursive", sleepTime);
			} else {

				Debug.Log ("Falhou!" + transform.name);
			}
		} else {

			currentCubePosition = 0;
		}
	}

	private void castOverlap(){

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		Collider[] colliders = Physics.OverlapBox (transform.position, moveOverlap, Quaternion.identity, cubeLayer);

		if (colliders.Length > 0 && players.Length > 0) {

			colliders = shuffleArray (colliders);

			cubes = adjustCubeArray (colliders, players);

			castOverlapRecursive ();
		} else {
			
			Debug.LogError ("No Colliders|Players was found! CO " + colliders.Length + " - PL " + players.Length);
		}
	}

	private void uncastOverlap(){

		cubes.ForEach (cube => {

			Transform current = cube.transform;
			hideRectangle(findRectangleOverlap(current));
			hideRectangle(findRectangleMouseOver(current));
		});
	}

	public void enableSphereEffect(){

		transform.FindChild ("Particle").gameObject.SetActive (true);
	}

	public void disableSphereEffect(){

		transform.FindChild ("Particle").gameObject.SetActive (false);
	}

	public void play(){

		action = Actions.Move;
		isCanPlay = true;

		enableSphereEffect ();
		castOverlap ();
	}

	public void stop(){

		action = Actions.None;
		actionKind = ActionsKind.None;

		isCanPlay = false;

		disableSphereEffect ();
		uncastOverlap ();

		ruler.setPanelAction (false);
		ruler.setSearchPlayerEnum ();

		moveOverlap = moveOverlapDefault;
	}

	//Skills and Itens
	public void Heal(){

		cHp += 50;

		stop ();
	}

	public void Pass(){

		stop ();
	}

	public void Damage(){

		actionKind = ActionsKind.Attack;
	}
}
