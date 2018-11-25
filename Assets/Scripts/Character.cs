/*
The MIT License (MIT)
 
Copyright (c) 2015 Kotonaga
 
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
 
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
 
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
 
[ExecuteInEditMode()]
public class Character : MonoBehaviour
{
	/// <summary>
	/// ウディタ用キャラクタ画像素材
	/// </summary>
	public Texture2D m_image;
	/// <summary>
	///  1セルの幅
	/// </summary>
	public int m_cropWidth;
	/// <summary>
	/// 1セルの高さ
	/// </summary>
	public int m_cropHeight;
	 
	void Awake ()
	{
		if(this.m_image==null)
			return;
		 
		// スプライトレンダラの取得
		SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		if (spriteRenderer == null) {
			spriteRenderer = gameObject.AddComponent<SpriteRenderer> ();
		}
		// アニメータの取得
		Animator animator = gameObject.GetComponent<Animator> ();
		if (animator == null) {
			animator = gameObject.AddComponent<Animator> ();
		}
		// ぼやけを除去
		this.m_image.filterMode = FilterMode.Point;
		 
		// 15フレームに一回アニメーションする
		float frameLength = 15f / 60f;
		// 画像は横にアニメーションする
		int frameCount = this.m_image.width / this.m_cropWidth;
		// 画像は縦に種類が並んでいる
		int stateCount = this.m_image.height / this.m_cropHeight;
		bool extendedCharaChipSet = false;
		 
		// ただし、8方向のチップセットが存在するので、それに対処する
		if(frameCount==6 || frameCount==10){
			frameCount >>= 1;
			stateCount <<= 1;
			extendedCharaChipSet = true;
		}
		 
		// スプライト配列の作成
		Sprite[,] sprites = new Sprite[stateCount, frameCount];
 
		// Blend Tree 設定用のVector2配列の準備
		Vector2[] positions = {
			new Vector2(0, 1),
			new Vector2(1, 0),
			new Vector2(-1, 0),
			new Vector2(0, -1),
			new Vector2(1, 1),
			new Vector2(-1, 1),
			new Vector2(1, -1),
			new Vector2(-1, -1),
		};

		// スプライトの切り出し
		for (int y=0; y<stateCount; y++) {
 			for (int x=0; x<frameCount; x++) {
				//指定された幅高さでテスクチャからスプライトを切り出す
				var rect = new Rect(x * this.m_cropWidth, y * this.m_cropHeight, this.m_cropWidth, this.m_cropHeight);
				var sprite = Sprite.Create(this.m_image, rect, new Vector2(0.5f,0.5f), 1.0f);
				sprite.name = this.m_image.name +"_" + y + x;
				sprite.texture.filterMode = FilterMode.Point; 
				sprites[y,x] = sprite;
			}
		}
		 
		// アニメータコントローラの作成
		AnimatorController animatorController = new AnimatorController ();
		// 名前はテクスチャ名にする
		animatorController.name = this.m_image.name;
		//向きと状態指定用のパラメータを追加
		animatorController.AddParameter ("DirectionX", AnimatorControllerParameterType.Float);
		animatorController.AddParameter ("DirectionY", AnimatorControllerParameterType.Float);
		animatorController.AddParameter ("Walking", AnimatorControllerParameterType.Bool);
		 
		// アニメータコントローラにベースレイヤを追加
		animatorController.AddLayer("Base");

		// 止まり状態を追加する
		BlendTree waitBlendTree;
		AnimatorState waitState =  animatorController.CreateBlendTreeInController("Wait", out waitBlendTree);
		waitBlendTree.name = "Blend Tree";
		 
		// 止まり状態はアニメーションしないが
		// 方向によってスプライトが変化する
		waitBlendTree.blendType = BlendTreeType.SimpleDirectional2D;
		waitBlendTree.blendParameter = "DirectionX";
		waitBlendTree.blendParameterY = "DirectionY";
		 
		// 状態の数だけループ
		for (int i=0; i<stateCount; i++) {
			// アニメーションしないので、キーフレームを1つだけ用意する
			ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[1];
			keyFrames[0]= new ObjectReferenceKeyframe{ 
				time = 0.0f,// 止まりのスプライトを指定
				value = sprites[i,1]
			};
			// アニメーションクリップの作成
			AnimationClip clip = new AnimationClip
			{
				// 名前は歩きとかぶるので、wait_状態のインデックスにする
				name = "Wait_" + i,
				// はみ出た部分は消す
				wrapMode = WrapMode.Clamp
			};
			// カーブの作成
			EditorCurveBinding curveBinding = new EditorCurveBinding
			{
				//ファイルは存在しない
				path = string.Empty,
				//スプライトをアニメーションするのでtypeにSpriteRenderer
				// propertyNameにはm_Spriteを指定
				type = typeof(SpriteRenderer),
				propertyName = "m_Sprite"
			};
			//クリップとカーブにキーフレームをバインド
			AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames); 
			//状態を止まり状態のブレンドツリーに追加 
			waitBlendTree.AddChild(clip, positions[i]); 
		}

		// 歩き状態を追加する
		BlendTree walkBlendTree;
		AnimatorState walkState =  animatorController.CreateBlendTreeInController("Walk", out walkBlendTree);
		walkBlendTree.name = "Blend Tree";
		 
		// 歩き状態は歩く方向によってアニメーションが変化する
		walkBlendTree.blendType = BlendTreeType.SimpleDirectional2D;
		walkBlendTree.blendParameter = "DirectionX";
		walkBlendTree.blendParameterY = "DirectionY";
		 
		// 状態の数だけループ
		for (int y=0; y<stateCount; y++) {
			//キーフレーム配列を作成
			// 右足、止まり、左足、止まり、右足とアニメーションする
			// ループするので、最後に右足を入れないと左足の後の止まりが短く不自然になる
			ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[frameCount + 2];
			// 8方向拡張対応
			int offsetX = 0, offsetY = 0;
			if(extendedCharaChipSet && y >= (stateCount>>1)) {
				offsetX = frameCount;
				offsetY = -(stateCount>>1);
			}
			// 普通のアニメーションの作成
 			for (int x=0; x<frameCount; x++) {
				// キーフレームの追加
				keyFrames[x] = new ObjectReferenceKeyframe{time=x*frameLength, value=sprites[y,x]};
			}
			// 2枚目->1枚目とアニメーションする
			for (int x=1; x>=0; x--) {
				int index = frameCount + 1 - x;
				keyFrames[index] = new ObjectReferenceKeyframe{ time=index*frameLength, value=sprites[y,x] };
			}

			// アニメーションクリップの作成
			AnimationClip clip = new AnimationClip
			{
				// 名前は状態のインデックスにする
				name = $"Walk_{y.ToString()}",
				// はみ出た部分は消す
				wrapMode = WrapMode.Clamp
			};
			// カーブの作成
			EditorCurveBinding curveBinding = new EditorCurveBinding
			{
				// ファイルは存在しない
				path = string.Empty,
				// スプライトをアニメーションするのでtypeにSpriteRenderer
				// propertyNameにはm_Spriteを指定
				type = typeof(SpriteRenderer),
				propertyName = "m_Sprite"
			};
			// クリップとカーブにキーフレームをバインド
			AnimationUtility.SetObjectReferenceCurve (clip, curveBinding, keyFrames);
			// LoopTimeが直接設定出来ないので一旦シリアライズする
			SerializedObject serializedClip = new SerializedObject (clip);
			// アニメーションクリップ設定の取り出し
			AnimationClipSettings animationClipSettings = new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings"))
			{
				// LoopTimeを有効にする
				LoopTime = true
			};
			// 設定を書き戻す
			serializedClip.ApplyModifiedProperties ();
			// 状態を歩き状態のブレンドツリーに追加
			walkBlendTree.AddChild (clip, positions[y]);
		}
		
		// 止まり->歩きのトランジションを作成
		AnimatorStateTransition waitToWalk = waitState.AddTransition(walkState);
		// Walkingフラグがtrueの時
		waitToWalk.AddCondition(AnimatorConditionMode.If,0,"Walking");
		 
		// 歩き->止まりのトランジションを作成
		AnimatorStateTransition walkToWait = walkState.AddTransition(waitState);
		// Walkingフラグがfalseの時
		walkToWait.AddCondition(AnimatorConditionMode.IfNot,0,"Walking");

		// アニメータコントローラに設定
		animator.runtimeAnimatorController = (animatorController as RuntimeAnimatorController);
				 
		// とりあえず0,0のスプライトをデフォルトにセット
		spriteRenderer.sprite = sprites[0,0];
	}
}
 
/// <summary>
/// AnimationClipSettingsの設定用クラス
/// </summary>
class AnimationClipSettings
{
	SerializedProperty m_property;
	 
	public AnimationClipSettings (SerializedProperty prop)
	{
		this.m_property = prop;
	}
	 
	private SerializedProperty Get (string property)
	{
		return this.m_property.FindPropertyRelative (property);
	}
	 
	public bool LoopTime
	{
		get
		{
			return this.Get ("m_LoopTime").boolValue;
		}
		set
		{
			this.Get ("m_LoopTime").boolValue = value;
		}
	}
}
#endif