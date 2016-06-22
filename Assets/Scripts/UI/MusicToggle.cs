using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MusicToggle : MonoBehaviour {
    Switch musicSwitch;

	// Use this for initialization
	void Start () {
        musicSwitch = GetComponent<Switch>();
        bool enabled = PlayerPrefs.GetInt("Music") == 1 ? true : false;
        musicSwitch.isOn = enabled;
    }
	
	public void SaveMusicPrefs(bool enabled) {
        PlayerPrefs.SetInt("Music", enabled ? 1 : 0);
    }
}
