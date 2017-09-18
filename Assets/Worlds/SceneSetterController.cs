using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetterController : MonoBehaviour {
	// Use this for initialization
	void Start () {
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

            //light_.transform.Rotate(new Vector3(100.0f, 0.0f, 0.0f));
            //var src = light_.transform.localRotation.eulerAngles;
            
            //light_.transform.localRotation = Quaternion.Euler(Random.Range(-185.0f, -175.0f), src.y, src.z);
            camera.transform.position = new Vector3(x, cpos.y, cpos.z);
        }
	}
}
