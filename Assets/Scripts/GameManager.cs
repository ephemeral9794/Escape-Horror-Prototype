using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

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
		var guid = AssetDatabase.FindAssets("t:MapEventData");
		MapEvent = new MapEventData[guid.Length];
		for (int i = 0; i < guid.Length; i++) { 
			var path = AssetDatabase.GUIDToAssetPath(guid[i]);
			MapEvent[i] = AssetDatabase.LoadAssetAtPath<MapEventData>(path);
		}
		//MapEvent = Resources.Load<MapEventData>("Map Event");
    }

    // Use this for initialization
    void Start () {
		timeElasped = 0.0f;
		fadeManager.fadeState = fadeIn ? 0 : 1;
	}
	
	public MapEventData.MapEvent GetMapEvent(Vector2Int pos) {
		int index = SceneManager.GetActiveScene().buildIndex;
		return MapEvent.SingleOrDefault(val => val.SceneNumber == index)[pos];
	}

	public void ChangeScene() => fadeManager.ChangeScene(fadeOut);
}
