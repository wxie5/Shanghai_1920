using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleLevelController : MonoBehaviour {
    public GameObject boss;

    public GameObject portal;
    public string sceneName;

    public static BattleLevelController instance;

    public void OnWin() {
        // portal.SetActive(true);
        StartCoroutine(LoadScene(2f));
    }

    private IEnumerator LoadScene(float delay) {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    private void Start() {
        if (instance == null) {
            instance = this;
        }
        boss.GetComponent<Enemy>().Activate();
    }
}
