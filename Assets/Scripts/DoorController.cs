using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Animator))]
public class DoorController : MonoBehaviour {

	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	public void OnAnimationTrigger()
	{
		animator.SetTrigger("OpenTrigger");
	}
}
