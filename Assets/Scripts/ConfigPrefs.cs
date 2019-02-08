using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace EscapeHorror.Prototype { 
	[CreateAssetMenu(menuName = "ConfigData")]
	public class ConfigPrefs : ScriptableObject
	{
		[SerializeField]
		public AudioMixer Mixer;
		[SerializeField]
		public float MasterVolume = 1.0f;
		[SerializeField]
		public float BGMVolume = 1.0f;
		[SerializeField]
		public float SEVolume = 1.0f;

		public void Apply() {
			Mixer.SetFloat("MasterVolume", Mathf.Lerp(-80.0f, 0.0f, MasterVolume));
			Mixer.SetFloat("BGMVolume", Mathf.Lerp(-80.0f, 0.0f, BGMVolume));
			Mixer.SetFloat("SEVolume", Mathf.Lerp(-80.0f, 0.0f, SEVolume));
		}
	}
}
