using UnityEngine;
using System.Collections;

public class AutoDeactivate : MonoBehaviour {
    [SerializeField]
    float timeBeforeDeactivation;
    float timer;
	void Start () {
        timer = 0.0f;
	}
	
	void Update () {
        timer += Time.deltaTime;
        if(timer >= timeBeforeDeactivation)
        {
            timer = 0.0f;
            gameObject.SetActive(false);
        }
	}
}
