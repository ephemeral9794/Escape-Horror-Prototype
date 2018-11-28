﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {
	[SerializeField]
	private float speed = 4;
	[SerializeField]
	private GridLayout grid;
	
	private float timeDuration = 0.25f;
	private Vector3 grid_size;
	private Animator animator;
	private bool walking;
	private float timeElapsed;
	private Vector3 start_pos;
	private Vector3 end_pos;

	// Use this for initialization
	void Awake () {
		animator = GetComponent<Animator>();
		walking = false;
		animator.SetBool("Walking", walking);
		grid_size = grid.CellToWorld(new Vector3Int(1, 1, 0));
		//transform.position += (grid_size / 2);
		timeElapsed = 0;
	}

	void MoveStart(Vector2 direct)
	{
		start_pos = transform.position;
		end_pos = start_pos + new Vector3(grid_size.x * direct.x, grid_size.y * direct.y, grid_size.z);
		timeElapsed = 0.0f;
	}

	void Move()
	{
		timeElapsed += Time.deltaTime;
		float rate = timeElapsed / timeDuration;
		rate = Mathf.Clamp(rate, 0f, 1f);
		//Lerp：StartPosを0,EndPosを1としたときに、rate(0~１)の位置を返してくれる
		transform.position = Vector3.Lerp(start_pos, end_pos, rate);
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
			MoveStart(new Vector2(x, y));
		}
		animator.SetBool("Walking", walking);

		// 歩き状態ならプレイヤーを移動
		if (walking) 
			Move();
		//if(walking){
			/*var rate = timeElapsed / timeDuration;
			rate = Mathf.Clamp(rate, 0, 1);
			transform.position = Vector3.Lerp(start_pos, end_pos, rate);
			if (rate >= 1.0f) {
				timeElapsed = 0.0f;
				end_pos += new Vector3(grid_size.x * x, grid_size.y * y, grid_size.z);
			}*/
        //}
		
	}
}
