using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeHorror.Prototype
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class DollController : MonoBehaviour {
		[SerializeField]
		private DollParameter parameter;
		[SerializeField]
		private int speed = 15;

		private Grid grid;
		private PlayerController player;
		private new Rigidbody2D rigidbody;

		private DollVisual visual;
		private List<DollParameter.DollNavigation> navigations;
		private Vector3 grid_size;  // 1グリッドあたりの大きさ（ワールド座標基準）
		private Vector3Int prev;
		private Vector3Int next;
		private int frames;
		private Vector2Int Direction;
		private int NextNav;

		// Use this for initialization
		void Start () {
			grid = FindObjectOfType<Grid>();
			player = FindObjectOfType<PlayerController>();
			rigidbody = GetComponent<Rigidbody2D>();
			visual = parameter.Visual;
			navigations = parameter.Navigations;
			NextNav = 0;
			Direction = navigations[NextNav].Direct;
			grid_size = grid.cellSize;
			prev = grid.WorldToCell(transform.position);
			next = prev;
			transform.position = grid.CellToWorld(prev) + new Vector3(0.5f, 0.5f, 0.0f);
			frames = speed;
			ChangeVisual();
		}
	
		// Update is called once per frame
		void Update () {
			var pos = transform.position;

			if (Move(ref pos))
			{
				var navpos = navigations[NextNav].Position;
				if (grid.WorldToCell(pos) == new Vector3Int(navpos.x, navpos.y, 0))
				{
					NextNav++;
					if (NextNav >= navigations.Count)
					{
						Destroy(gameObject);
						return;
					}
					Direction = navigations[NextNav].Direct;
					ChangeVisual();
				}
				MovePoint(Direction, ref next);
			}

			pos += new Vector3(0.5f, 0.5f, 0.0f);
			rigidbody.MovePosition(new Vector2(pos.x, pos.y));
			frames++;
		}

		bool Move(ref Vector3 pos)
		{
			float t = (float)frames / speed;
			var div = next - prev;
			var wprev = grid.CellToWorld(prev);
			pos.x = wprev.x + (div.x * grid_size.x) * t;
			pos.y = wprev.y + (div.y * grid_size.y) * t;

			if (frames >= speed)
			{
				frames = 0;
				prev = next;
				return true;
			}
			else
			{
				return false;
			}
		}

		void MovePoint(Vector2 direct, ref Vector3Int point)
		{
			//var save = point;
			// 左右
			if (direct.x > 0.0f)
			{
				//direction.x = 1;
				point.x += 1;
			}
			else if (direct.x < 0.0f)
			{
				//direction.x = -1;
				point.x -= 1;
			}
			// 上下
			if (direct.y > 0.0f)
			{
				//direction.y = 1;
				point.y += 1;
			}
			else if (direct.y < 0.0f)
			{
				//direction.y = -1;
				point.y -= 1;
			}
		}

		private void ChangeVisual()
		{
			var renderer = GetComponent<SpriteRenderer>();
			var dir = new Vector2Int(Direction.x, Direction.y);
			if (dir == Vector2Int.up) {
				renderer.sprite = visual.back;
			} else if (dir == Vector2Int.down) {
				renderer.sprite = visual.front;
			} else if(dir == Vector2Int.right) {
				renderer.sprite = visual.right;
			} else if (dir == Vector2Int.left) {
				renderer.sprite = visual.left;
			} else {
				renderer.sprite = visual.front;
			}
		}
	}
}
