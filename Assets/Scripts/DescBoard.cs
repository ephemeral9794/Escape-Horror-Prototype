using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class DescBoard : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.OnKeyDownAsObservable(KeyCode.Space).Subscribe(_ => {
			Debug.Log("");
			Destroy(gameObject);
		}).AddTo(this);
	}
	
}
