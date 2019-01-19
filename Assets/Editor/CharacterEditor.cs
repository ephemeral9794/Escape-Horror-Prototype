using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;
using System.IO;

public class CharacterEditor : EditorWindow
{
    private Texture2D m_Image;
    private int m_CropWidth;
    private int m_CropHeight;
    private string m_FileName;
    private string m_Folder;

    [MenuItem("GameObject/2D Object/Character", false, 0)]
    static void ShowWindow()
    {
        GetWindow<CharacterEditor>();
    }

    private void OnEnable()
    {
        m_CropWidth = 0;
        m_CropHeight = 0;
        m_FileName = "";
        m_Folder = "Assets/Resources/Animations/";
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        m_Image = (Texture2D)EditorGUILayout.ObjectField("Image", m_Image, typeof(Texture2D), false, null);
        if (EditorGUI.EndChangeCheck())
        {
            if (string.IsNullOrEmpty(m_FileName))
            {
                m_FileName = m_Image.name + ".controller";
                m_CropWidth = m_Image.width / 3;
                m_CropHeight = m_Image.height / 4;
            }
        }
        m_CropWidth = EditorGUILayout.IntField("Crop Width", m_CropWidth);
        m_CropHeight = EditorGUILayout.IntField("Crop Height", m_CropHeight);
        m_FileName = EditorGUILayout.TextField("File Name", m_FileName);
        m_Folder = EditorGUILayout.TextField("Folder Path", m_Folder);
        if (GUILayout.Button("Create Character")) {
            CreateCharacter();
        }
    }

    private void CreateCharacter()
    {
        var obj = new GameObject("Character");
        var animator = obj.AddComponent<Animator>();
        var renderer = obj.AddComponent<SpriteRenderer>();
        // ぼやけを除去
        this.m_Image.filterMode = FilterMode.Point;

        // 15フレームに一回アニメーションする
        float frameLength = 15f / 60f;
        // 画像は横にアニメーションする
        int frameCount = this.m_Image.width / this.m_CropWidth;
        // 画像は縦に種類が並んでいる
        int stateCount = this.m_Image.height / this.m_CropHeight;
        bool extendedCharaChipSet = false;

        // ただし、8方向のチップセットが存在するので、それに対処する
        if (frameCount == 6 || frameCount == 10)
        {
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
        Texture2D[,] textures = new Texture2D[stateCount, frameCount];
        for (int y = 0; y < stateCount; y++)
        {
            for (int x = 0; x < frameCount; x++)
            {
                //指定された幅高さでテスクチャからスプライトを切り出す
                var rect = new Rect(x * this.m_CropWidth, y * this.m_CropHeight, this.m_CropWidth, this.m_CropHeight);
                var texture = new Texture2D(m_CropWidth, m_CropHeight);
                var colors = m_Image.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
                texture.SetPixels(colors);
                var bin = texture.EncodeToPNG();
                File.WriteAllBytes(m_Folder + $"{m_Image.name + "_" + y + x}.png", bin);
                AssetDatabase.Refresh();
                //var sprite = Sprite.Create(this.m_Image, rect, new Vector2(0.5f, 0.5f), this.m_CropHeight);
                /*var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), m_CropHeight);
                sprite.name = this.m_Image.name + "_" + y + x;
                sprite.texture.filterMode = FilterMode.Point;
                sprites[y, x] = sprite;*/
                //AssetDatabase.CreateAsset(texture, m_Folder + $"{sprite.name}.png");
            }
        }
        for (int y = 0; y < stateCount; y++)
        {
            for (int x = 0; x < frameCount; x++)
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(m_Folder + $"{m_Image.name + "_" + y + x}.png");
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), m_CropHeight);
                sprite.name = this.m_Image.name + "_" + y + x;
                sprite.texture.filterMode = FilterMode.Point;
                sprites[y, x] = sprite;
            } 
        }
        // アニメータコントローラの作成
        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(m_Folder + m_FileName);
        // 名前はテクスチャ名にする
        animatorController.name = this.m_Image.name;
        //向きと状態指定用のパラメータを追加
        animatorController.AddParameter("DirectionX", AnimatorControllerParameterType.Float);
        animatorController.AddParameter("DirectionY", AnimatorControllerParameterType.Float);
        animatorController.AddParameter("Walking", AnimatorControllerParameterType.Bool);

        // アニメータコントローラにベースレイヤを追加
        //animatorController.AddLayer("Base");

        // 止まり状態を追加する
        BlendTree waitBlendTree;
        AnimatorState waitState = animatorController.CreateBlendTreeInController("Wait", out waitBlendTree);
        waitBlendTree.name = "Blend Tree";

        // 止まり状態はアニメーションしないが
        // 方向によってスプライトが変化する
        waitBlendTree.blendType = BlendTreeType.SimpleDirectional2D;
        waitBlendTree.blendParameter = "DirectionX";
        waitBlendTree.blendParameterY = "DirectionY";

        // 状態の数だけループ
        for (int i = 0; i < stateCount; i++)
        {
            // アニメーションしないので、キーフレームを1つだけ用意する
            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[1];
            keyFrames[0] = new ObjectReferenceKeyframe
            {
                time = 0.0f,// 止まりのスプライトを指定
                value = sprites[i, 1]
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
            // AnimationClipをAssetとして保存
            AssetDatabase.CreateAsset(clip, m_Folder + $"{Path.GetFileNameWithoutExtension(m_FileName)}_{clip.name}.anim");
        }

        // 歩き状態を追加する
        BlendTree walkBlendTree;
        AnimatorState walkState = animatorController.CreateBlendTreeInController("Walk", out walkBlendTree);
        walkBlendTree.name = "Blend Tree";

        // 歩き状態は歩く方向によってアニメーションが変化する
        walkBlendTree.blendType = BlendTreeType.SimpleDirectional2D;
        walkBlendTree.blendParameter = "DirectionX";
        walkBlendTree.blendParameterY = "DirectionY";

        // 状態の数だけループ
        for (int y = 0; y < stateCount; y++)
        {
            //キーフレーム配列を作成
            // 右足、止まり、左足、止まり、右足とアニメーションする
            // ループするので、最後に右足を入れないと左足の後の止まりが短く不自然になる
            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[frameCount + 2];
            // 8方向拡張対応
            int offsetX = 0, offsetY = 0;
            if (extendedCharaChipSet && y >= (stateCount >> 1))
            {
                offsetX = frameCount;
                offsetY = -(stateCount >> 1);
            }
            // 普通のアニメーションの作成
            for (int x = 0; x < frameCount; x++)
            {
                // キーフレームの追加
                keyFrames[x] = new ObjectReferenceKeyframe { time = x * frameLength, value = sprites[y, x] };
            }
            // 2枚目->1枚目とアニメーションする
            for (int x = 1; x >= 0; x--)
            {
                int index = frameCount + 1 - x;
                keyFrames[index] = new ObjectReferenceKeyframe { time = index * frameLength, value = sprites[y, x] };
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
            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);
            // LoopTimeが直接設定出来ないので一旦シリアライズする
            SerializedObject serializedClip = new SerializedObject(clip);
            // アニメーションクリップ設定の取り出し
            AnimationClipSettings animationClipSettings = new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings"))
            {
                // LoopTimeを有効にする
                LoopTime = true
            };
            // 設定を書き戻す
            serializedClip.ApplyModifiedProperties();
            // 状態を歩き状態のブレンドツリーに追加
            walkBlendTree.AddChild(clip, positions[y]);
            // AnimationClipをAssetとして保存
            AssetDatabase.CreateAsset(clip, m_Folder + $"{Path.GetFileNameWithoutExtension(m_FileName)}_{clip.name}.anim");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 止まり->歩きのトランジションを作成
        AnimatorStateTransition waitToWalk = waitState.AddTransition(walkState);
        // Walkingフラグがtrueの時
        waitToWalk.AddCondition(AnimatorConditionMode.If, 0, "Walking");

        // 歩き->止まりのトランジションを作成
        AnimatorStateTransition walkToWait = walkState.AddTransition(waitState);
        // Walkingフラグがfalseの時
        walkToWait.AddCondition(AnimatorConditionMode.IfNot, 0, "Walking");

        // アニメータコントローラに設定
        animator.runtimeAnimatorController = (animatorController as RuntimeAnimatorController);

        // とりあえず0,0のスプライトをデフォルトにセット
        renderer.sprite = sprites[0, 0];
    }
}

/// <summary>
/// AnimationClipSettingsの設定用クラス
/// </summary>
class AnimationClipSettings
{
    SerializedProperty m_property;

    public AnimationClipSettings(SerializedProperty prop)
    {
        this.m_property = prop;
    }

    private SerializedProperty Get(string property)
    {
        return this.m_property.FindPropertyRelative(property);
    }

    public bool LoopTime {
        get {
            return this.Get("m_LoopTime").boolValue;
        }
        set {
            this.Get("m_LoopTime").boolValue = value;
        }
    }
}