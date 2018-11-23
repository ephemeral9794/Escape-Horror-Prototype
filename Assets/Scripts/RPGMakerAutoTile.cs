using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps {
	[Serializable]
	public class RPGMakerAutoTile : TileBase {
		// RPGツクール仕様のオートタイル画像（タイル分割済み）を設定
		[SerializeField]
		public Sprite[] m_RawTileSprites;

		// タイルパターン（47種類）
		public Sprite[] m_PatternedSprites;

		public override void RefreshTile(Vector3Int position, ITilemap tilemap)
		{
			// 
			for (int y = -1; y <= 1; y++) {
				for (int x = -1; x <= 1; x++) {
					var location = new Vector3Int(position.x + x, position.y + y, position.z);
					if (TileValue(tilemap, position))
						tilemap.RefreshTile(position);
				}
			}
		}

		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			UpdateTile(position, tilemap, ref tileData);
			return;
		}

		private bool TileValue(ITilemap tileMap, Vector3Int position)
        {
            TileBase tile = tileMap.GetTile(position);
            return (tile != null && tile == this);
        }

		// タイル情報の更新
		private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		{
			// パターン生成がされていない場合、生成
			if (m_PatternedSprites == null) {
				if (m_RawTileSprites[0] && m_RawTileSprites[1] && m_RawTileSprites[2] && m_RawTileSprites[3] && m_RawTileSprites[4] && m_RawTileSprites[5]) {
					GeneratePatterns();
				} else {
					return;
				}
			}
			tileData.transform = Matrix4x4.identity;
			tileData.color = Color.white;

			// 種類判別用マスク
			int mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
			mask += TileValue(tileMap, location + new Vector3Int(1, 1, 0)) ? 2 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 4 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, -1, 0)) ? 8 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 16 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 64 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;

			int index = GetIndex((byte)mask);
		}

		private int GetIndex(byte mask)
		{
			string[] patternTexts =
			{
				"11111111",	// 8方向隣接
				"01111111",	// 左斜め上のみ隣接なし
				"11111011",	// 右斜め上のみ隣接なし
				"11011111",	// 左斜め下のみ隣接なし
				"11111110",	// 右斜め下のみ隣接なし
				"01111101",	// 斜め上のみ隣接なし
				"11010111", // 斜め下のみ隣接なし
				""
			};
		}
	}
}
