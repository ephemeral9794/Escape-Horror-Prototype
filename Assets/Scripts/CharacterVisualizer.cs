using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EscapeHorror.Prototype { 
	public class CharacterVisualizer : MonoBehaviour {
		public enum Position {
			LEFT, CENTER, RIGHT
		}

		private Dictionary<string, CharacterPack.CharacterType> CharacterMatchList = new Dictionary<string, CharacterPack.CharacterType>{
			{"アカネ", CharacterPack.CharacterType.AKANE},
			{"ミキ", CharacterPack.CharacterType.MIKI},
			{"リョータ", CharacterPack.CharacterType.RYOTA},
			{"タケル", CharacterPack.CharacterType.TAKERU}
		}; 
		private CharacterPack[] characters;
		private GameObject[] positions; 

		private void Awake()
		{
			characters = Resources.LoadAll<CharacterPack>("Character");
			/*foreach (var c in characters)
			{
				Debug.Log(c.name);
			}*/
		}
		private void Start()
		{
			positions = new GameObject[3];
			positions[0] = transform.Find("Left").gameObject;
			positions[1] = transform.Find("Center").gameObject;
			positions[2] = transform.Find("Right").gameObject;
		}

		public void Invisible(Position pos)
		{
			positions[(int)pos].SetActive(false);
		}
		public void InvisibleAll()
		{
			for (int i = 0; i < 3; i++) {
				positions[i].SetActive(false);
			}
		}
		public void Visible(Position pos, string typeStr, int diffNum)
		{
			Debug.Log("Visible");
			var type = CharacterMatchList[typeStr];
			positions[(int)pos].SetActive(true);
			var image = positions[(int)pos].GetComponent<Image>();
			var trans = positions[(int)pos].GetComponent<RectTransform>();
			var character = characters.SingleOrDefault(c => c.Character == type);
			image.sprite = character.Sprites[diffNum];
			trans.sizeDelta = new Vector2(character.Size.width, character.Size.height);
			var local = trans.localPosition;
			local.y = character.Size.y;
			trans.localPosition = local;
		}
	}
}
