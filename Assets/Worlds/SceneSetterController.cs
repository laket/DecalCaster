using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetterController : MonoBehaviour {
    GameObject dram_;

	// Use this for initialization
	void Start () {
        dram_ = GameObject.Find("dram_base");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F))
        {
            var camera = Camera.main;
            var cpos = camera.transform.position;
            camera.fieldOfView = Random.Range(20, 52);

            int x = 0;
            if (Random.Range(0, 2) < 1) {
                x = Random.Range(13, 21);
            }
            else
            {
                x = Random.Range(-5, 2);
            }

            camera.transform.position = new Vector3(x, cpos.y, cpos.z);
        }
	}
}
