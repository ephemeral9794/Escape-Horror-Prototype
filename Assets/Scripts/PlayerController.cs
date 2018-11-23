using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {
	[SerializeField]
	private float speed = 4;

	private Animator animator;
	private bool walking;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		walking = false;
		animator.SetBool("Walking", walking);
	}
	
	// Update is called once per frame
	void Update () {
		// 入力を取得
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");

		// 入力がない場合、待機へ移行
		if (x == 0.0f && y == 0.0f) {
			walking = false;
		} // 入力がある場合、歩きへ移行
		else {
			walking = true;
			// プレイヤーの方向を入力
			animator.SetFloat("DirectionX", x);
			animator.SetFloat("DirectionY", y);
		}
		animator.SetBool("Walking", walking);

		// 歩き状態ならプレイヤーを移動
		if(walking){
            transform.Translate(new Vector3(x, y, 0) * Time.deltaTime * speed);
            transform.position = new Vector3(transform.position.x,transform.position.y,0.0f);//transform.position.y / 1000f);
        }
	}
}
