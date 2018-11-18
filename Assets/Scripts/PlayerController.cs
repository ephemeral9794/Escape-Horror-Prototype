using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {
	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		animator.speed = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.DownArrow)){
            animator.speed = 1;
            animator.SetInteger("Walk", 0); 
        }
        if (Input.GetKey(KeyCode.LeftArrow)){
            animator.speed = 1;
            animator.SetInteger("Walk", 1);
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            animator.speed = 1;
            animator.SetInteger("Walk", 2);
        }
        if (Input.GetKey(KeyCode.UpArrow)){
            animator.speed = 1;
            animator.SetInteger("Walk", 3);
        }

        if (Input.GetKeyUp(KeyCode.DownArrow)){
            animator.speed = 0;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow)){
            animator.speed = 0;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow)){
            animator.speed = 0;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow)){
            animator.speed = 0;
        }
	}
}
