using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CSwitchScene : MonoBehaviour {

	public void LoadScene(string name) {
		SceneManager.LoadScene (name);
	}

}
