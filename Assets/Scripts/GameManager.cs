using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeHorror.Prototype { 
	public class GameManager : MonoBehaviour {

		public static Vector2Int init_pos = new Vector2Int(0,0);

		[SerializeField]
		private SceneFadeManager fadeManager;
		[SerializeField]
		private bool fadeIn = true;
		[SerializeField]
		private bool fadeOut = true;

		public bool IsNovelMode { get; set; }
		public MapEventData[] MapEvent { get; private set; }

		private float timeElasped;

		private void Awake()
		{
			IsNovelMode = false;
			MapEvent = Resources.LoadAll<MapEventData>("MapEvent");
			/*foreach (var events in MapEvent)
			{
				Debug.Log(events);
			}*/
			/*var guid = AssetDatabase.FindAssets("t:MapEventData");
			MapEvent = new MapEventData[guid.Length];
			for (int i = 0; i < guid.Length; i++) { 
				var path = AssetDatabase.GUIDToAssetPath(guid[i]);
				MapEvent[i] = AssetDatabase.LoadAssetAtPath<MapEventData>(path);
			}*/
			//MapEvent = Resources.Load<MapEventData>("Map Event");
		}

		// Use this for initialization
		void Start () {
			timeElasped = 0.0f;
			fadeManager.fadeState = fadeIn ? 0 : 1;
			var player = FindObjectOfType<PlayerController>();
			if (init_pos.x != 0 && init_pos.y != 0)
			{
				var grid = FindObjectOfType<Grid>();
				var player_pos = grid.CellToWorld(new Vector3Int(init_pos.x, init_pos.y, 0));
				player.gameObject.transform.position = player_pos;
				player.Direction = (init_pos.x < 0) ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
				init_pos = new Vector2Int(0, 0);
			}
		}
	
		public MapEventData.MapEvent GetMapEvent(Vector2Int pos) {
			int index = SceneManager.GetActiveScene().buildIndex;
			return MapEvent.SingleOrDefault(val => val.SceneNumber == index)[pos];
		}

		public void ChangeScene(bool floorShift) => fadeManager.ChangeScene(floorShift, fadeOut);
	}

}