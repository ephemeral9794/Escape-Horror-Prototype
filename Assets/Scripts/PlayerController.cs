using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

namespace EscapeHorror.Prototype { 
	using Event = MapEventData.Event;
	public interface IRecieveMessage : IEventSystemHandler {
		void OnRecieve();
	}

	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class PlayerController : MonoBehaviour, IRecieveMessage {
		[SerializeField]
		private int speed = 12;
		[SerializeField]
		private GameObject Doll_Prefab;

		// プレイヤーの向き
		public Vector2Int Direction { get; set; }

		private Vector3 grid_size;  // 1グリッドあたりの大きさ（ワールド座標基準）
		private Vector3Int prev;
		private Vector3Int next;
		private int frames;
		private Animator animator;
		private new Rigidbody2D rigidbody;
		private Grid grid;
		private Tilemap[] tilemaps;
		private float timeElasped;
		private GameManager manager;
		private int nextScene = -1;
		private TargetHint hint;
		private bool DollFlag = false;
        private List<Vector2Int> endNovelPos = new List<Vector2Int>();

		private void Start()
		{
			animator = GetComponent<Animator>();
			rigidbody = GetComponent<Rigidbody2D>();
			hint = FindObjectOfType<TargetHint>();
			grid = FindObjectOfType<Grid>();
			tilemaps = grid.GetComponentsInChildren<Tilemap>();
			animator.SetFloat("DirectionX", Direction.x);
			animator.SetFloat("DirectionY", Direction.y);
			Direction = Vector2Int.zero;
			grid_size = grid.cellSize;
			prev = grid.WorldToCell(transform.position);
			next = prev;
			transform.position = grid.CellToWorld(prev) + new Vector3(0.5f, 0.5f, 0.0f);
			frames = 0;
			timeElasped = 0.0f;
			manager = FindObjectOfType<GameManager>();
			Action<Unit> action = (_) => {
				var pos = grid.WorldToCell(transform.position);
				var eventpos = new Vector2Int(pos.x + Direction.x, pos.y + Direction.y);
				var mapEventData = manager.GetMapEventData();
                if (mapEventData != null) { 
				    var mapEvent = mapEventData[eventpos];
                    //var pair = manager.GetEventAndParam(eventpos);
				    //var mapEvent = manager.GetMapEvent(eventpos);
                    //var mapEvent = pair.Key;
                    foreach (var e in mapEvent) {
                        if (e.Event == Event.Transition_Action)
                        {   // 移動アクション
                            nextScene = e.NextScene;
                            var ctrl = FindObjectOfType<DoorController>();
                            ctrl.OnAnimationTrigger();
                        }
                        else if (e.Event == Event.Trick)
                        {   // 仕掛け
                            var parameters = manager.GetTrickParams(mapEventData, eventpos);
                            foreach (var param in parameters)
                            {
                                switch (param.Type)
                                {
                                    case TrickParameterTable.TrickType.Doll:
                                        hint.ChangeText("人形から逃げる");
                                        DollFlag = true;
                                        break;
                                    case TrickParameterTable.TrickType.Remove:
                                        {
                                            var overlay = tilemaps.SingleOrDefault(t => t.name == "Overlay");
                                            //Debug.Log(overlay);
                                            overlay.SetTile(new Vector3Int(param.Position.x, param.Position.y, 0), null);
                                        }
                                        break;
                                }
                            }
                        }else if (e.Event == Event.Talk_Action)
						{
							if (!endNovelPos.Contains(eventpos))
							{
								var param = manager.GetTalkParams(mapEventData, eventpos);
								//Debug.Log(param[0].Scenario);
								if (!manager.IsNovelMode)
								{
									manager.IsNovelMode = true;
									manager.SetScenario(param[0].Scenario);
									endNovelPos.Add(eventpos);
								}
							}
						} 
                    }
                }
			};
            this.OnKeyDownAsObservable(KeyCode.Space).Subscribe(action).AddTo(this);
            this.OnKeyDownAsObservable(KeyCode.Return).Subscribe(action).AddTo(this);
		}

		void Update () {
			var pos = transform.position;
			if (Move(ref pos)) {
				var dir = GetInputDirection();
				if(dir.x == 0.0f && dir.y == 0.0f) {
					animator.SetBool("Walking", false);
					float x = animator.GetFloat("DirectionX");
					float y = animator.GetFloat("DirectionY");
					Direction = ToDirection(x, y);
				} else {
					animator.SetBool("Walking", true);
					animator.SetFloat("DirectionX", dir.x);
					animator.SetFloat("DirectionY", dir.y);
				}
				MovePoint(dir, ref next);
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

            // ノベルモード中は操作不可
            if (manager.IsNovelMode) {
                point = save;
            }
            // 階移動
            //var mapEvent = manager.GetMapEvent(new Vector2Int(next.x, next.y));
            //var pair = manager.GetEventAndParam(new Vector2Int(next.x, next.y));
            var mapEventData = manager.GetMapEventData();
            //var pos = new Vector2Int(next.x, next.y);
            var pos = new Vector2Int(point.x, point.y);
            if (mapEventData != null) { 
			    var mapEvent = mapEventData[pos];
                foreach (var e in mapEvent) 
                    if (e.Event == Event.Transition ) {
					    var param = manager.GetTransitionParams(mapEventData, pos);
					    point = save;
                        manager.ChangeScene(e.NextScene, param[0].Position, param[0].Direction);
			        } else if (e.Event == Event.Trick_Trap) {
				        var parameters = manager.GetTrickParams(mapEventData, pos);
                        foreach (var param in parameters) { 
                            switch (param.Type) { 
                                case TrickParameterTable.TrickType.Doll: { 
				                        if (DollFlag) {
					                        var p = grid.CellToWorld(new Vector3Int(param.Position.x, param.Position.y, 0)) + new Vector3(0.5f, 0.5f, 0.0f);
					                        Instantiate(Doll_Prefab, p, Quaternion.identity);
					                        DollFlag = false;
				                        } 
                                    } break;
                                case TrickParameterTable.TrickType.Remove:
                                    {
                                        var overlay = tilemaps.SingleOrDefault(t => t.name == "Overlay");
                                        overlay.SetTile(new Vector3Int(param.Position.x, param.Position.y, 0), null);
                                    } break;
                            }
                        }
			        } else if (e.Event == Event.Talk) {
                        if (!endNovelPos.Contains(pos)) { 
					        var param = manager.GetTalkParams(mapEventData, pos);
							//Debug.Log(param[0].Scenario);
                            if (!manager.IsNovelMode) {
                                manager.IsNovelMode = true;
                                manager.SetScenario(param[0].Scenario);
                                endNovelPos.Add(pos);
                            }
                        }
				    } else if (e.Event == Event.End_Game)
					{
						manager.ScenarioManager.overlay = false;
						manager.ScenarioManager.nextScene = e.NextScene;
					}
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
	
		public void OnRecieve()
		{
			// 部屋移動
			manager.ChangeScene(nextScene);
		}
	}
}