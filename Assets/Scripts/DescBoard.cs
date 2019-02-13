using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class DescBoard : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Action<Unit> action = (_) => {
			Destroy(gameObject);
		};
		this.OnKeyDownAsObservable(KeyCode.Space).Subscribe(action).AddTo(this);
		this.OnKeyDownAsObservable(KeyCode.Return).Subscribe(action).AddTo(this);
	}
	
}
