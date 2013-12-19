using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class GameController : MonoBehaviour {
	
	public Rigidbody weapon;
	public Transform weaponHolder;
	public int forceMultiplier = 5;
	public UILabel healthLabel;
	public UILabel weaponLabel;
	public UILabel enemyCountLabel;
	
	private bool rightHand = true;
	private Vector2 startBtPosition;
	public GUIStyle noStyle;
	private GameObject player;
	public Texture btGoBg;
	public int btSize;
	public string collectableWeapon;
	public List<string> chars;
	public List<Vector3> spawnPositions;
	private bool gameActive = true;
	public int maxChar = 15;
	private int curChar = 0;
	public int maxWeapon = 5;
	private int curWeapon = 0;
	private int totalWeapons = 0;
	private int enemyCounter = 0;
	public int health = 100;
	public GameObject pointer;
	public GameObject gOverPanel;
	public GameObject loadingPanel;
	private bool gOver = false;
		
	void Start () {
		player = GameObject.Find("Player");
		if (PlayerPrefs.HasKey("Hand")) {
			if (PlayerPrefs.GetInt("Hand") == 0) {
				rightHand = false;
			} else {
				rightHand = true;
			}
		} else {
			PlayerPrefs.SetInt("Hand", 1);
		}
		
		if (rightHand) {
			startBtPosition = new Vector2(0, Screen.height - btSize);
		} else {
			startBtPosition = new Vector2(Screen.width - btSize, Screen.height-btSize);
		}
		
		StartCoroutine("StuffPopulation");
		StartCoroutine("DisablePointer");
		
	}
	
	void GameOver() {
		gOver = true;
		gOverPanel.SetActive(true);
		player.rigidbody.Sleep();
	}
	
	void Restart() {
		gOverPanel.SetActive(false);
		loadingPanel.SetActive(true);
		Application.LoadLevel("Maze");
	}
	
	IEnumerator DisablePointer() {
		yield return new WaitForSeconds(8);
		pointer.SetActive(false);
	}

	void Update () {
		
		foreach (Touch t in Input.touches) {
			if (t.phase == TouchPhase.Ended && !inButton(t.position)) {
				WeaponShot(t.position);
			}
		}
		
	}
	
	void UpdateHealth (int cnt) {
		if (!gOver) {
			health += cnt;
			if (health == 0) {
				GameOver();
			}
			healthLabel.text = health.ToString();
		}
	}
	
	void PickedWeapon() {
		curWeapon--;
	}
	
	IEnumerator StuffPopulation () {
		while (gameActive) {
			yield return new WaitForSeconds(5);
			if (curChar <= maxChar) {
				int rndPos = Random.Range(0, spawnPositions.Count - 1);
				int rndChar = Random.Range(0, chars.Count);
				int rndRot = Random.Range(0, 360);
				GameObject go = Instantiate(Resources.Load(chars[rndChar])) as GameObject;
				go.gameObject.transform.position = spawnPositions[rndPos];
				go.gameObject.transform.rotation = Quaternion.Euler(go.transform.eulerAngles.x, rndRot, go.transform.eulerAngles.z);
				curChar++;
			}
			
			if (curWeapon <= maxWeapon) {
				int rndPosW = Random.Range(0, spawnPositions.Count - 1);
				GameObject go = Instantiate(Resources.Load(collectableWeapon)) as GameObject;
				go.gameObject.transform.position = spawnPositions[rndPosW];
				curWeapon++;
			}
		}
	}
	
	void UpdateWeapons (int cnt) {
		totalWeapons += cnt;
		weaponLabel.text = totalWeapons.ToString();
	}
	
	void WeaponShot (Vector2 t) {
		if (totalWeapons > 0) {
			UpdateWeapons(-1);
			Vector3 flickTargetPositionM = Camera.main.ScreenToWorldPoint(new Vector3(t.x, t.y, 100));
			Rigidbody weaponClone = Instantiate(weapon, weaponHolder.position, weapon.rotation ) as Rigidbody;
			weaponClone.gameObject.GetComponent<WeaponControl>().enabled = true;
			weaponClone.transform.LookAt(flickTargetPositionM);
			weaponClone.constraints = RigidbodyConstraints.None;
			weaponClone.AddForce(weaponClone.transform.forward*100, ForceMode.Impulse);
		}
	}
	
	bool inButton(Vector2 pos) {
		if (pos.x >= startBtPosition.x && pos.x <= (startBtPosition.x + btSize) && pos.y <= btSize) {
			return true;
		} else {
			return false;
		}
	}
	
	void CharDied() {
		if (curChar > 0) {
			curChar--;
			enemyCounter++;
			enemyCountLabel.text = enemyCounter.ToString();
		}
	}
	
	void OnGUI() {
		if (GUI.RepeatButton(new Rect(startBtPosition.x, startBtPosition.y, btSize, btSize), btGoBg, noStyle)) {
			player.SendMessage("Move");
		}
	}
}
