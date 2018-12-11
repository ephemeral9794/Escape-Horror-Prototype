using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {
	[SerializeField]
	private int speed = 12;
	[SerializeField]
	private GridLayout grid;
	
	private Vector3 grid_size;	// 1グリッドあたりの大きさ（ワールド座標基準）
	private Vector3Int direction;	// プレイヤーの向き
	private Vector3Int prev;
	private Vector3Int next;
	private int frames;
	private Animator animator;

	private void Start() {
		direction = Vector3Int.zero;
		grid_size = grid.CellToWorld(new Vector3Int(1, 1, 0));
		prev = grid.WorldToCell(transform.position);
		next = prev;
		transform.position = grid.CellToWorld(prev) + new Vector3(0.5f, 0.5f, 0.0f);
		Debug.Log(grid.CellToWorld(prev));
		frames = 0;
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		var pos = transform.position;
		if (Move(ref pos)) {
			var dir = GetInputDirection();
			if(dir.x == 0.0f && dir.y == 0.0f) {
				animator.SetBool("Walking", false);
			} else {
				animator.SetBool("Walking", true);
				animator.SetFloat("DirectionX", dir.x);
				animator.SetFloat("DirectionY", dir.y);
			}
			MovePoint(dir, ref next);
			//Debug.Log($"{next} {pos}");
		}
		transform.position = pos + new Vector3(0.5f, 0.5f, 0.0f);
		frames++;
	}

	bool Move(ref Vector3 pos) {
		float t = (float)frames / speed;
		var div = next - prev;
		var wprev = grid.CellToWorld(prev);
		pos.x = wprev.x + (div.x * grid_size.x) * t;
		pos.y = wprev.y + (div.y * grid_size.y) * t;
		//Debug.Log(t);

		if (frames >= speed) {
			frames = 0;
			prev = next;
			return true;
		} else {
			return false;
		}
	}

	void MovePoint(Vector2 direct, ref Vector3Int point)
	{
		// 左右
		if (direct.x > 0.0f) {
			point.x += 1;
		} else if (direct.x < 0.0f) {
			point.x -= 1;
		}
		// 上下
		if (direct.y > 0.0f) {
			point.y += 1;
		} else if (direct.y < 0.0f) {
			point.y -= 1;
		}
	}

	Vector2 GetInputDirection()
	{
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		//Debug.Log($"{x}, {y}");
		return new Vector2(x, y);
	}
}
