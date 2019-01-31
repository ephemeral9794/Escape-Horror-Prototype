using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeHorror.Prototype {

    using MapEvent = MapEventData.MapEvent;
    using Parameter = TransitionParameterTable.Parameter;
    public class GameManager : MonoBehaviour {

		//public static Vector2Int init_pos = new Vector2Int(0, 0);
		public static Vector2Int next_pos = Vector2Int.zero;
        public static Vector2Int next_dir = Vector2Int.zero;

		[SerializeField]
		private SceneFadeManager fadeManager;
		[SerializeField]
		private bool fadeIn = true;
		[SerializeField]
		private bool fadeOut = true;

		public bool IsNovelMode { get; set; }
		public MapEventData[] MapEvent { get; private set; }
        public TransitionParameterTable[] ParamTable { get; private set; }

		private float timeElasped;

		private void Awake()
		{
			IsNovelMode = false;
			MapEvent = Resources.LoadAll<MapEventData>("MapEvent");
            ParamTable = Resources.LoadAll<TransitionParameterTable>("MapEvent");
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
			//if (next_pos != Vector2Int.zero || next_dir != Vector2Int.zero)
            if ((next_pos.x != 0 && next_pos.y != 0) || (next_dir.x != 0 && next_dir.y != 0))
			{
				var grid = FindObjectOfType<Grid>();
				var player_pos = grid.CellToWorld(new Vector3Int(next_pos.x, next_pos.y, 0));
                player.gameObject.transform.position = player_pos;
                //Debug.Log($"{next_pos} {next_dir} {player.gameObject.transform.position}");
                player.Direction = next_dir;
				next_pos = Vector2Int.zero;
				next_dir = Vector2Int.zero;
			}
			/*if (init_pos.x != 0 && init_pos.y != 0)
			{
				var grid = FindObjectOfType<Grid>();
				var player_pos = grid.CellToWorld(new Vector3Int(init_pos.x, init_pos.y, 0));
				player.gameObject.transform.position = player_pos;
				player.Direction = (init_pos.x < 0) ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
				init_pos = new Vector2Int(0, 0);
			}*/
		}
	
		public MapEvent GetMapEvent(Vector2Int pos) {
			int index = SceneManager.GetActiveScene().buildIndex;
			return MapEvent.SingleOrDefault(val => val.SceneNumber == index)[pos];
		}
        public KeyValuePair<MapEvent, Parameter> GetEventAndParam(Vector2Int pos)
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            var mapEvent = MapEvent.SingleOrDefault(val => val.SceneNumber == index);
            var table = ParamTable.SingleOrDefault(p => p.CurrentScene == mapEvent.SceneNumber);
            var param = table.Parameters.SingleOrDefault(pp => pp.MapSceneNumber == mapEvent[pos].NextScene);
            return new KeyValuePair<MapEvent, Parameter>(mapEvent[pos], param);
        }

		//public void ChangeScene(bool floorShift) => fadeManager.ChangeScene(floorShift, fadeOut);
		public void ChangeScene(int nextScene)
		{
			next_pos = Vector2Int.zero;
			next_dir = Vector2Int.zero;
			fadeManager.ChangeScene(nextScene, fadeOut);
		}
		public void ChangeScene(int nextScene, Vector2Int nextPos, Vector2Int nextDirect){
			next_pos = nextPos;
			next_dir = nextDirect;
			fadeManager.ChangeScene(nextScene, fadeOut);
		}
	}

}