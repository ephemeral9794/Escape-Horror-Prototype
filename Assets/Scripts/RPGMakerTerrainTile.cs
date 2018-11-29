using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps { 
    [Serializable]
    [CreateAssetMenu(fileName = "New RPGMaker TerrainTile", menuName = "Tiles/RPGMaker TerrainTile")]
    public class RPGMakerTerrainTile : TileBase {
        [SerializeField]
        public Sprite[] m_RawTilesSprites;
        
        Sprite[] m_Sprites;

        public override void RefreshTile(Vector3Int location, ITilemap tileMap)
        {
            for (int yd = -1; yd <= 1; yd++)
                for (int xd = -1; xd <= 1; xd++)
                {
                    Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                    if (TileValue(tileMap, position))
                        tileMap.RefreshTile(position);
                }
        }

        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            UpdateTile(location, tileMap, ref tileData);
        }

        private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            if (m_Sprites == null) {
                if (m_RawTilesSprites[0] && m_RawTilesSprites[1] && m_RawTilesSprites[2] && m_RawTilesSprites[3] && m_RawTilesSprites[4] && m_RawTilesSprites[5]) {
                    GeneratePatterns();
                }
            }
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;

            int mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, 1, 0)) ? 2 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 4 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(1, -1, 0)) ? 8 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 16 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 64 : 0;
            mask += TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;

            byte original = (byte)mask;
            if ((original | 254) < 255) { mask = mask & 125; }
            if ((original | 251) < 255) { mask = mask & 245; }
            if ((original | 239) < 255) { mask = mask & 215; }
            if ((original | 191) < 255) { mask = mask & 95; }

            int index = GetIndex((byte)mask);
            if (index >= 0 && index < m_Sprites.Length && TileValue(tileMap, location))
            {
                tileData.sprite = m_Sprites[index];
                tileData.transform = GetTransform((byte)mask);
                tileData.color = Color.white;
                tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
                tileData.colliderType = Tile.ColliderType.Sprite;
            }
        }

        private bool TileValue(ITilemap tileMap, Vector3Int position)
        {
            TileBase tile = tileMap.GetTile(position);
            return (tile != null && tile == this);
        }

        private int GetIndex(byte mask)
        {
            switch (mask)
            {
                case 0: return 0;
                case 1:
                case 4:
                case 16:
                case 64: return 1;
                case 5:
                case 20:
                case 80:
                case 65: return 2;
                case 7:
                case 28:
                case 112:
                case 193: return 3;
                case 17:
                case 68: return 4;
                case 21:
                case 84:
                case 81:
                case 69: return 5;
                case 23:
                case 92:
                case 113:
                case 197: return 6;
                case 29:
                case 116:
                case 209:
                case 71: return 7;
                case 31:
                case 124:
                case 241:
                case 199: return 8;
                case 85: return 9;
                case 87:
                case 93:
                case 117:
                case 213: return 10;
                case 95:
                case 125:
                case 245:
                case 215: return 11;
                case 119:
                case 221: return 12;
                case 127:
                case 253:
                case 247:
                case 223: return 13;
                case 255: return 14;
            }
            return -1;
        }

        private Matrix4x4 GetTransform(byte mask)
        {
            switch (mask)
            {
                case 4:
                case 20:
                case 28:
                case 68:
                case 84:
                case 92:
                case 116:
                case 124:
                case 93:
                case 125:
                case 221:
                case 253:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f), Vector3.one);
                case 16:
                case 80:
                case 112:
                case 81:
                case 113:
                case 209:
                case 241:
                case 117:
                case 245:
                case 247:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -180f), Vector3.one);
                case 64:
                case 65:
                case 193:
                case 69:
                case 197:
                case 71:
                case 199:
                case 213:
                case 215:
                case 223:
                    return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -270f), Vector3.one);
            }
            return Matrix4x4.identity;
        }

        Sprite[,] Segments = new Sprite[6, 4];
        int[][] Patterns = new int[][]
        {
            new int[] {0,0,0,0},
            new int[] {4,5,0,0},
            new int[] {4,1,4,4},
            new int[] {4,4,4,4},
            new int[] {2,3,4,5},
            new int[] {2,1,4,1},
            new int[] {2,2,4,1},
            new int[] {2,1,4,4},
            new int[] {2,2,4,4},
            new int[] {1,1,1,1},
            new int[] {1,4,1,1},
            new int[] {1,4,1,2},
            new int[] {1,4,3,1},
            new int[] {1,4,3,5},
            new int[] {4,5,2,3},
        };

        public void GeneratePatterns()
        {
            for (int i = 0; i < 6; i++) {
                var sprite = m_RawTilesSprites[i];
                var texture = sprite.texture;
                int y = (int)sprite.rect.y;
                int x = (int)sprite.rect.x;
                int height = (int)sprite.rect.height;
                int width = (int)sprite.rect.width;
                int height_half = height / 2;
                int width_half = width / 2;
                Segments[i, 0] = Sprite.Create(texture, new Rect(x, y, width_half, height_half), Vector2.zero);
                Segments[i, 1] = Sprite.Create(texture, new Rect(x + width_half, y, width_half, height_half), Vector2.zero);
                Segments[i, 2] = Sprite.Create(texture, new Rect(x, y + height_half, width_half, height_half), Vector2.zero);
                Segments[i, 3] = Sprite.Create(texture, new Rect(x + width_half, y + height_half, width_half, height_half), Vector2.zero);
            }

            m_Sprites = new Sprite[15];
            for (int i = 0; i < m_Sprites.Length; i++)
            {
                m_Sprites[i] = CombineTextures(Patterns[i]);
                //m_Sprites[i] = m_RawTilesSprites[0];
            }
            //m_Sprites[0] = m_RawTilesSprites[0];
        }

        private Sprite CombineTextures(int[] TypeIndex)
        {
            int[] fixedArray = new int[4];
            fixedArray[0] = TypeIndex[2];
            fixedArray[1] = TypeIndex[3];
            fixedArray[2] = TypeIndex[0];
            fixedArray[3] = TypeIndex[1];

            Color[][] texs = new Color[4][];
            for (int i = 0; i < 4; i++)
            {

                int x = (int)Segments[fixedArray[i], i].rect.x;
                int y = (int)Segments[fixedArray[i], i].rect.y;
                int w = (int)Segments[fixedArray[i], i].rect.width;
                int h = (int)Segments[fixedArray[i], i].rect.height;
                texs[i] = Segments[fixedArray[i], i].texture.GetPixels(x, y, w, h);
            }

            int width_half = (int)Segments[0, 0].rect.width;
            int height_half = (int)Segments[0, 0].rect.height;
            int width = width_half * 2;
            int height = height_half * 2;

            Color[] texArray = new Color[width * height];
            for (int i = 0; i < height_half; i++)
            {
                Array.Copy(texs[0], i * width_half, texArray, i * width, width_half);
            }

            for (int i = 0; i < height_half; i++)
            {
                Array.Copy(texs[1], i * width_half, texArray, i * width + width_half, width_half);
            }

            for (int i = 0; i < height_half; i++)
            {
                Array.Copy(texs[2], i * width_half, texArray, (i + height_half) * width, width_half);
            }

            for (int i = 0; i < height_half; i++)
            {
                Array.Copy(texs[3], i * width_half, texArray, (i + height_half) * width + width_half, width_half);
            }
            Texture2D ret = new Texture2D(width, height, TextureFormat.ARGB32, false);
            ret.filterMode = FilterMode.Point;
            ret.wrapMode = TextureWrapMode.Clamp;
            ret.SetPixels(texArray);
            return Sprite.Create(ret, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), width);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RPGMakerTerrainTile))]
    public class RPGMakerTerrainTileEditor : Editor
    {
        private RPGMakerTerrainTile tile { get { return (target as RPGMakerTerrainTile); } }

        public void OnEnable()
        {
            if (tile.m_RawTilesSprites == null || tile.m_RawTilesSprites.Length != 6)
            {
                tile.m_RawTilesSprites = new Sprite[6];
                EditorUtility.SetDirty(tile);
            }
        }


        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Place sprites shown based on the contents of the sprite.");
            EditorGUILayout.Space();

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 210;

            EditorGUI.BeginChangeCheck();
            tile.m_RawTilesSprites[0] = (Sprite)EditorGUILayout.ObjectField("代表パターン", tile.m_RawTilesSprites[0], typeof(Sprite), false, null);
            tile.m_RawTilesSprites[1] = (Sprite)EditorGUILayout.ObjectField("四隅に境界を持つパターン", tile.m_RawTilesSprites[1], typeof(Sprite), false, null);
            tile.m_RawTilesSprites[2] = (Sprite)EditorGUILayout.ObjectField("集合パターン左上", tile.m_RawTilesSprites[2], typeof(Sprite), false, null);
            tile.m_RawTilesSprites[3] = (Sprite)EditorGUILayout.ObjectField("集合パターン右上", tile.m_RawTilesSprites[3], typeof(Sprite), false, null);
            tile.m_RawTilesSprites[4] = (Sprite)EditorGUILayout.ObjectField("集合パターン左下", tile.m_RawTilesSprites[4], typeof(Sprite), false, null);
            tile.m_RawTilesSprites[5] = (Sprite)EditorGUILayout.ObjectField("集合パターン右下", tile.m_RawTilesSprites[5], typeof(Sprite), false, null);
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(tile);

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
#endif
}
