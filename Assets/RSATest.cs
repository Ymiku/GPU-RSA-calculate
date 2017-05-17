using UnityEngine;
using System.Collections;

public class RSATest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		int a;
		for (int n = 0; n < 100; n++) {
			for (int m = 0; m < 100; m++) {
				int i = Random.Range (0,100);
				for (int c = 0; c < 500; c++) {
					a = (i+c) * 3 % 33;
				}
			}
		}
	}
}
