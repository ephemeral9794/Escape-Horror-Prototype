using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlayer : MonoBehaviour {
	
	const int GRID_SPAN = 1;
	const float SPEED = 2.0f;
	Vector3 inputDirect;		//入力の向き 
	Vector3 direction;			//プレーヤーキャラの方向性 
	Vector3 prePos;				//１フレーム前の状態保存
	Vector3 pos;				//現フレーム位置
	Vector3 currentGrid;
	bool cross;					//グリッド境界
	
	// Update is called once per frame
	void Update ()
	{
		cross = false;
		//あくまで静止状態から初速を得るためだけの入力要求 
		//ゲームを開始して一度でも方向キーが入力されたらdirectionがVector3.zeroになることは今後一切ない
		//その意味で以下は起動直後用のみのコード
		if (direction == Vector3.zero) {
			direction = GetDirect ();
		}
		
		prePos = transform.position;
		pos = transform.position; 
		pos += direction * SPEED * Time.deltaTime; 
		
		prePos /= GRID_SPAN;	//1÷3=0.3333になる　この理屈でGRID_SPAN毎に判定が可能となる
		pos /= GRID_SPAN;
		
		if ((int)pos.x != (int)prePos.x) {
			cross = true;
		}
		if ((int)pos.z != (int)prePos.z) {
			cross = true;
		}
		
		//近いグリッド
		Vector3 nearGrid = new Vector3 (Mathf.Round (pos.x), pos.y, Mathf.Round (pos.z));
		
		// posが「？.01」～「？.49」までの間フラグが立つ（例0.01～0.49の間など） 
		// forwardPosはフラグ検出用に補助に使われているだけ
		Vector3 forwardPos = pos + (direction * 0.5f / GRID_SPAN);
		if (Mathf.RoundToInt (forwardPos.x) != Mathf.RoundToInt (pos.x) ||
			Mathf.RoundToInt (forwardPos.z) != Mathf.RoundToInt (pos.z)) {
			//進む先の壁のチェック
			if (Physics.Raycast (pos, direction, 1.0f * GRID_SPAN)) {
				//見かけは静止しているが運動量（direction）を弄らず位置を補正し続けている為「静止している訳ではない」 
				//運動量はVector3.Zero以外の値を持ち続けている事に注意 
				pos = nearGrid;
				cross = true;
			}
		}
		
		//この入力が本当の意味でのゲーム内プレーヤー入力
		inputDirect = GetDirect ();
		//入力をプレーヤーの方向性に反映させる　状況により反映方法が変わる
		if (cross) {
			if (inputDirect != Vector3.zero) {
				//入力した方向に曲がれるかどうかの壁チェック 
				if (!Physics.Raycast (nearGrid, inputDirect, 1.0f * GRID_SPAN)) {
					direction = inputDirect;
				}
			}
		} else {
			if (Vector3.Dot (direction, inputDirect) < -0.9999f) {
				direction = inputDirect;
			}
		}
			
		this.transform.position = pos;	//最期に補正の有無にかかわらず現フレーム位置を反映させる 
	}
	
	float inputTimer;
	const float THRESHOLD = 0.3f;
	const float CONTINUATION = 2.0f;
	Vector3 preInput;
	
	//４方向レバー　無入力時２秒延滞 
	Vector3 GetDirect ()
	{
		inputTimer += Time.deltaTime;
		Vector3 input = new Vector3 (Input.GetAxis ("Horizontal"), 0.0f, Input.GetAxis ("Vertical"));
		float absX = Mathf.Abs (input.x); 
		float absZ = Mathf.Abs (input.z); 
		
		if (absX < THRESHOLD && absZ < THRESHOLD) {
			if (inputTimer < CONTINUATION) {
				return preInput;
			}
			return Vector3.zero;
		}

		inputTimer = 0;
		if (absX < absZ) {
			input.x = 0.0f;
		} else {
			input.z = 0.0f;
		}
		
		input = input.normalized;
		preInput = input;
		return input;
	}
	
}
