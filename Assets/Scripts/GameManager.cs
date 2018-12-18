using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public static GameManager Instance { get; private set; }

    public bool IsNovelMode { get; set; }
	public MapEventData MapEvent { get; private set; }

	private float timeElasped;

    private void Awake()
	{

		if (Instance == null)
		{
			IsNovelMode = false;
			MapEvent = Resources.Load<MapEventData>("Map Event");
			Instance = this;
		}
		else if (Instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
		timeElasped = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		timeElasped += Time.deltaTime;
	}

	public static MapEventData.MapEvent GetMapEvent(Vector2Int pos) {
		var mapevent = Instance.MapEvent;
		return mapevent[pos];
	}
}
