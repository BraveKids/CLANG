using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {

    public Dictionary<string, AudioClip> musics;
    public Dictionary<string, AudioClip> effects;

    // source
    AudioSource audioSource = null;

    // clips 
    public AudioClip menuMusic;
    public AudioClip gameMusic;




    string musicPlayed;


    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start() {

        audioSource = GetComponent<AudioSource>();

        musics = new Dictionary<string, AudioClip>();
        musics.Add(menuMusic.name, menuMusic);
        //musics.Add(gameMusic.name, gameMusic);
        

        
        effects = new Dictionary<string, AudioClip>();


        audioSource = GetComponent<AudioSource>() as AudioSource;
        audioSource.loop = true;
    }

    // Update is called once per frame
    void Update() {
        /*if (PlayerPrefs.GetInt("Music") == 0) SetVolume(0f);
        else SetVolume(1f);       */ 
    }

    public void StopMusic() {
        audioSource.Stop();
    }

    public void SetVolume(float _volume) {
        audioSource.volume = _volume;
    }

    public void SetBackgroundMusic(string background) {
        if (background == musicPlayed) {
            return;
        }
        if (musics.ContainsKey(background)) {
            audioSource.clip = musics[background];
            audioSource.Play();
            musicPlayed = background;

        }



    }

    public void SetMusic(bool _music) {
        if (audioSource.isPlaying == true && _music == false) {
            audioSource.Pause();
        }
        if (audioSource.isPlaying == false && _music == true) {
            audioSource.Play();
        }
    }

    public void PlayEffect(string audioEffect) {
        if (PlayerPrefs.GetInt("Music")==1) {    //play if music is on
            Debug.Log(effects[audioEffect].ToString());
            AudioSource audio = new AudioSource();
            audio.clip = effects[audioEffect];
            audio.Play();
            Destroy(audio, audio.clip.length);
        }
    }

    /*public AudioSource playLoopEffect(string audioEffect) {
        AudioSource audio = new AudioSource();
        audio.loop = true;
        audio.clip = effects[audioEffect];
        audio.Play();
        return audio;
    }

    public void stopEffect(AudioSource source) {
        if (source != null) {
            source.Stop();
        }

    }*/
}