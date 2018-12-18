using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {
	[SerializeField]
	private int speed = 12;
	[SerializeField]
	private Grid grid;
	
	private Vector3 grid_size;	// 1グリッドあたりの大きさ（ワールド座標基準）
	private Vector2Int direction;	// プレイヤーの向き
	private Vector3Int prev;
	private Vector3Int next;
	private int frames;
	private Animator animator;
	private new Rigidbody2D rigidbody;
	private Tilemap[] tilemaps;
	private float timeElasped;
	private bool flag;

	private void Start() {
		direction = Vector2Int.zero;
		grid_size = grid.cellSize;
		prev = grid.WorldToCell(transform.position);
		next = prev;
		transform.position = grid.CellToWorld(prev) + new Vector3(0.5f, 0.5f, 0.0f);
		frames = 0;
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody2D>();
		tilemaps = grid.GetComponentsInChildren<Tilemap>();
		timeElasped = 0.0f;
		/*foreach (var t in tilemaps) {
			Debug.Log(t.name);
		}*/
		//this.OnKeyDownAsObservable(KeyCode.Space).Subscribe(_ => Debug.Log(direction));
		this.OnKeyDownAsObservable(KeyCode.Space).Subscribe(_ => {
			var pos = grid.WorldToCell(transform.position);
			var eventpos = new Vector2Int(pos.x + direction.x, pos.y + direction.y);
			var events = GameManager.GetMapEvent(eventpos);
			Debug.Log($"{eventpos}, {events}");
		}).AddTo(this);
	}

	// Update is called once per frame
	void Update () {
		var pos = transform.position;
		if (Move(ref pos)) {
			var dir = GetInputDirection();
			if(dir.x == 0.0f && dir.y == 0.0f) {
				animator.SetBool("Walking", false);
				float x = animator.GetFloat("DirectionX");
				float y = animator.GetFloat("DirectionY");
				//Debug.LogFormat("{0},{1}", x, y);
				direction = ToDirection(x, y);
				flag = false;
				//Debug.Log(direction);
			} else {
				if ((dir.x >= 1.0f || dir.x <= -1.0f) || (dir.y >= 1.0f || dir.y <= -1.0f)) {
					animator.SetBool("Walking", true);
					flag = true;
				}
				animator.SetFloat("DirectionX", dir.x);
				animator.SetFloat("DirectionY", dir.y);
			}
			if (flag)
				MovePoint(dir, ref next);
			//Debug.Log($"{next} {pos}");
		}
		//transform.position = pos + new Vector3(0.5f, 0.5f, 0.0f);
		pos += new Vector3(0.5f, 0.5f, 0.0f);
		rigidbody.MovePosition(new Vector2(pos.x, pos.y));
		frames++;
		timeElasped += Time.deltaTime;
	}

	bool Move(ref Vector3 pos) {
		float t = (float)frames / speed;
		var div = next - prev;
		var wprev = grid.CellToWorld(prev);
		pos.x = wprev.x + (div.x * grid_size.x) * t;
		pos.y = wprev.y + (div.y * grid_size.y) * t;
		//Debug.Log($"{wprev}, {prev}");
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
		var save = point;
		// 左右
		if (direct.x > 0.0f) {
			//direction.x = 1;
			point.x += 1;
		} else if (direct.x < 0.0f) {
			//direction.x = -1;
			point.x -= 1;
		}
		// 上下
		if (direct.y > 0.0f) {
			//direction.y = 1;
			point.y += 1;
		} else if (direct.y < 0.0f) {
			//direction.y = -1;
			point.y -= 1;
		}

		// 次の目標地点のタイルに当たり判定があるなら進まない（変更を戻す）
		foreach (var tilemap in tilemaps) {
			if (tilemap.GetColliderType(next) != Tile.ColliderType.None) {
				point = save;
			}
		}
	}

	Vector2 GetInputDirection()
	{
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		return new Vector2(x, y);
	}

	private Vector2Int ToDirection(float x, float y)
	{
		var direct = Vector2Int.zero;
		if (x > 0.0f)
		{
			direct.x = 1;
		}
		else if (x < 0.0f)
		{
			direct.x = -1;
		}/* else
		{
			direct.x = 0;
		}*/
		else if (y > 0.0f)
		{
			direct.y = 1;
		}
		else if (y < 0.0f)
		{
			direct.y = -1;
		}/* else
		{
			direct.y = 0;
		}*/
		return direct;
	}
}
