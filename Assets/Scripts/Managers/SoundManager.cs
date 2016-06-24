using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UnityStandardAssets.Network {

    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour {

        public Dictionary<string, AudioClip> musics;
        public Dictionary<string, AudioClip> effects;

        // source
        AudioSource audioSource = null;
        public float effectVolume = 0.7f;

        // clips 
        public AudioClip menuMusic;
        public AudioClip gameMusic;

        //effect
        public AudioClip Attack;
        public AudioClip CollectObject;
        public AudioClip Death;
        public AudioClip Defend;
        public AudioClip DropObject;
        public AudioClip emptyGun;
        public AudioClip Grenade;
        public AudioClip gun;
        public AudioClip Running;
        public AudioClip bearTrap;
        public AudioClip draggingWorm;
        public AudioClip growlingWorm;
        public AudioClip Mutant;
        public AudioClip Swarm;
        public AudioClip Tank;
        public AudioClip audienceCheering;
        public AudioClip cardAvaible;
        public AudioClip pulseRecharge;
        public AudioClip spawningEnemies;
        public AudioClip button;




        string musicPlayed;


        void Awake () {
            //DontDestroyOnLoad(gameObject);
        }

        // Use this for initialization
        void Start () {
            audioSource = GetComponent<AudioSource>();

            musics = new Dictionary<string, AudioClip>();
            //musics.Add(menuMusic.name, menuMusic);
            //musics.Add(gameMusic.name, gameMusic);



            effects = new Dictionary<string, AudioClip>();
            /*effects.Add(Attack.name, Attack);
            effects.Add(CollectObject.name, CollectObject);
            effects.Add(Death.name, Death);
            effects.Add(Defend.name, Defend);
            effects.Add(DropObject.name, DropObject);
            effects.Add(emptyGun.name, emptyGun);
            effects.Add(Grenade.name, Grenade);
            effects.Add(gun.name, gun);
            effects.Add(Running.name, Running);
            effects.Add(bearTrap.name, bearTrap);
            effects.Add(draggingWorm.name, draggingWorm);
            effects.Add(growlingWorm.name, growlingWorm);
            effects.Add(Mutant.name, Mutant);
            effects.Add(Swarm.name, Swarm);
            effects.Add(Tank.name, Tank);
            effects.Add(audienceCheering.name, audienceCheering);
            effects.Add(cardAvaible.name, cardAvaible);
            effects.Add(pulseRecharge.name, pulseRecharge);
            effects.Add(spawningEnemies.name, spawningEnemies);
            effects.Add(button.name, button);*/







            audioSource = GetComponent<AudioSource>() as AudioSource;
            audioSource.loop = true;
        }

        // Update is called once per frame
        void Update () {
            float volume = PlayerPrefs.GetInt("Music", 1);
            SetVolume(volume);
            /*if (Input.GetKeyDown("space"))
                SetBackgroundMusic("gameMusic");
            if (Input.GetKeyDown(KeyCode.P))
                SetBackgroundMusic("menuMusic");
            if (Input.GetKeyDown(KeyCode.E))
                PlayEffect("cardAvaible");
            if (Input.GetKeyDown(KeyCode.R))
                PlayEffect("audienceCheering", 4f);*/

        }

        public void StopMusic () {
            audioSource.Stop();
        }

        public void SetVolume (float _volume) {
            audioSource.volume = _volume;
        }

        public void SetBackgroundMusic (string background) {
            if (background == musicPlayed) {
                return;
            }
            if (musics.ContainsKey(background)) {
                audioSource.clip = musics[background];
                audioSource.Play();
                musicPlayed = background;

            }



        }

        public void SetMusic (bool _music) {
            if (audioSource.isPlaying == true && _music == false) {
                audioSource.Pause();
            }
            if (audioSource.isPlaying == false && _music == true) {
                audioSource.Play();
            }
        }

        public void PlayEffect (string audioEffect, float waitTime) {
            StartCoroutine(PlayEffectIterator(audioEffect, waitTime));
        }

        public void PlayEffect (string audioEffect) {
            StartCoroutine(PlayEffectIterator(audioEffect, 0f));
        }



        IEnumerator PlayEffectIterator (string audioEffect, float waitTime) {
            //if (PlayerPrefs.GetInt("Music")==1) {    //play if music is on
            yield return new WaitForSeconds(waitTime);
            if (effects.ContainsKey(audioEffect)) {
                Debug.Log(effects[audioEffect].ToString() + " " + effects[audioEffect]);
                AudioSource audio = gameObject.AddComponent<AudioSource>();
                audio.clip = effects[audioEffect];
                audio.volume = effectVolume;
                audio.Play();
                Destroy(audio, audio.clip.length);
            } else { Debug.Log("Clip not present"); }
            //}
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
}