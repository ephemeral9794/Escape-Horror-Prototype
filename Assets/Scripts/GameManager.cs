using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EscapeHorror.Prototype {

    using MapEvent = MapEventData.MapEvent;
    using TransParam = TransitionParameterTable.Parameter;
	using TrickParam = TrickParameterTable.Parameter;
	using TalkParam = TalkParameterTable.Parameter;
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

		public bool IsNovelMode {
			get{
				return (scenario != null) ? scenario.IsEnabled : false;
			} 
			set{
				novelMode = value;
				if (value)
				{
					scenario.Enable();
				} else
				{
					scenario.Disable();
				}
			}
		}
		public MapEventData[] MapEvents { get; private set; }
        public TransitionParameterTable[] TransParams { get; private set; }
		public TrickParameterTable[] TrickParams { get; private set; }
		public TalkParameterTable[] TalkParams { get; private set; }
		public ConfigPrefs Config { get; private set; }

		private float timeElasped;
		private ScenarioManager scenario;
		private bool novelMode = false;

		private void Awake()
		{
			MapEvents = Resources.LoadAll<MapEventData>("MapEvent");
			TransParams = Resources.LoadAll<TransitionParameterTable>("MapEvent");
			TrickParams = Resources.LoadAll<TrickParameterTable>("MapEvent");
			TalkParams = Resources.LoadAll<TalkParameterTable>("MapEvent");
			Config = Resources.Load<ConfigPrefs>("ConfigPrefs");
        }

		// Use this for initialization
		void Start ()
		{
			scenario = FindObjectOfType<ScenarioManager>();
            //IsNovelMode = false;
            if (scenario != null)
            if (scenario.overlay) { 
                scenario.IsEnabled = false;
            }
            Config.Apply();
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
	
		/*public MapEvent GetMapEvent(Vector2Int pos) {
			int index = SceneManager.GetActiveScene().buildIndex;
			return MapEvents.SingleOrDefault(val => val.SceneNumber == index)[pos];
		}*/
		public MapEventData GetMapEventData()
		{
			int index = SceneManager.GetActiveScene().buildIndex;
			return MapEvents.SingleOrDefault(val => val.SceneNumber == index);
		}
        /*public MapEventData[] GetMapEventData()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            return MapEvents.Where(val => val.SceneNumber == index).ToArray();
        }*/
		public TransParam[] GetTransitionParams(MapEventData mapEvent, Vector2Int pos)
		{
			var param = TransParams.SingleOrDefault(p => p.CurrentScene == mapEvent.SceneNumber);
            var list = new List<TransParam>();
            foreach (var e in mapEvent[pos])
            {
                list.AddRange(param.GetParameters(e.NextScene));
            }
			return list.ToArray();
		}
        /*public KeyValuePair<MapEvent, TransParam> GetEventAndParam(Vector2Int pos)
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            var mapEvent = MapEvents.SingleOrDefault(val => val.SceneNumber == index);
            var table = TransParams.SingleOrDefault(p => p.CurrentScene == mapEvent.SceneNumber);
            var param = table.Parameters.SingleOrDefault(pp => pp.MapSceneNumber == mapEvent[pos].NextScene);
            return new KeyValuePair<MapEvent, TransParam>(mapEvent[pos], param);
        }*/
		public TrickParam[] GetTrickParams(MapEventData mapEvent, Vector2Int pos)
		{
			//Debug.LogFormat("{0}, {1}", TrickParams[0].CurrentScene, mapEvent.SceneNumber);
			var param = TrickParams.SingleOrDefault(p => p.CurrentScene == mapEvent.SceneNumber);
            var list = new List<TrickParam>();
            foreach (var e in mapEvent[pos])
            {
                list.AddRange(param.GetParameters(e.NextScene));
            }
            return list.ToArray();
            //return param.GetParameters(mapEvent[pos].NextScene);
		}
		public TalkParam[] GetTalkParams(MapEventData mapEvent, Vector2Int pos)
		{
			//Debug.LogFormat("{0}, {1}", TalkParams[0].CurrentScene, mapEvent.SceneNumber);
			var param = TalkParams.SingleOrDefault(p => p.CurrentScene == mapEvent.SceneNumber);
            var list = new List<TalkParam>();
            foreach (var e in mapEvent[pos])
            {
                list.AddRange(param.GetParameters(e.NextScene));
            }
            return list.ToArray();
            //return param.GetParameters(mapEvent[pos].NextScene);
        }

		public void SetScenario(TextAsset text)
		{
            //Debug.Log(text.name);
            //Debug.Log(text.ToString());
			scenario.scenarioText = text;
			scenario.AnalysisScenario();
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