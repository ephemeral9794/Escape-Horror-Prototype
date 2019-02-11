using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EscapeHorror.Prototype { 
	public class TargetHint : MonoBehaviour {
		private TextMeshProUGUI m_Text;
		private Image m_Image;

		void Start () {
			m_Text = GetComponentInChildren<TextMeshProUGUI>();
			m_Image = GetComponentInChildren<Image>();
		}

		public void ChangeText(string text)
		{
			m_Text.text = $"目標:{text}";
		}
		public void ChangeText(string text, Color color)
		{
			ChangeText(text);
			m_Image.color = color;
		}
	}
}
