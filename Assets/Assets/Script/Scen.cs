using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scen : MonoBehaviour {
    //이 스크립트 지워도 됨? 쓰는거 본적 없는데.
    public GameObject Menu;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Esc"))
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;

            if(Time.timeScale == 0)
            Menu.SetActive(true);
            else
                Menu.SetActive(false);
        }

    }
}
