using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class AudioSync : NetworkBehaviour {
    private AudioSource source;

    public AudioClip[] clips;
    // Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
	}
	
	public void PlaySound(int id)
    {
        if(id>=0 && id < clips.Length)
        {
            CmdServerSoundID(id);
        }
    }

    [Command]
    void CmdServerSoundID(int id)
    {
        RpcServerSoundID(id);
    }

    [ClientRpc]
    void RpcServerSoundID(int id)
    {
        source.PlayOneShot(clips[id]);
    }

    public void PlayLoopSound(int id)
    {
        if (id >= 0 && id < clips.Length)
        {
            CmdServerLoopSoundID(id);
        }
    }

    [Command]
    void CmdServerLoopSoundID(int id)
    {
        RpcServerLoopSoundID(id);
    }

    [ClientRpc]
    void RpcServerLoopSoundID(int id)
    {
        source.Play();
    }
}
