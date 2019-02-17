using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Operators;
using UniRx.Triggers;

namespace EscapeHorror.Prototype { 
	public class ContinueController : MonoBehaviour {
		[SerializeField]
		private float time;

		private GameManager manager;
		private SceneFadeManager fadeManager;
		private float timeElasped;

		// Use this for initialization
		void Start () {
			timeElasped = 0.0f;
			manager = FindObjectOfType<GameManager>();
			fadeManager = FindObjectOfType<SceneFadeManager>();
			fadeManager.ObserveEveryValueChanged((m) => m.fadeState).Where((s) => s == 1).Subscribe((_) => {
				//Debug.Log("ObserveEveryValueChanged");
				if (time > 0) { 
					Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe((__) => {
						//Debug.Log("Timer");
						manager.ChangeScene(0);
					}).AddTo(this);
				} else { 
					Action<Unit> action = (__) => {
						manager.ChangeScene(0);
					};
					this.OnKeyAsObservable(KeyCode.Space).Subscribe(action);
					this.OnKeyAsObservable(KeyCode.Return).Subscribe(action);
				}
			}).AddTo(this);
		}
	
		// Update is called once per frame
		void Update () {
			timeElasped += Time.deltaTime;
		}
	}
}