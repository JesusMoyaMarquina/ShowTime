using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    //Keys
    public const string MAIN_KEY = "mainVolume";
    public const string MUSIC_KEY = "musicVolume";
    public const string FX_KEY = "fxVolume";

    public static SoundManager instance;

    public AudioSource source;
    public AudioMixer main, music, fx;
    public AudioClip gameThemeMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayGameThemeMusic()
    {
        source.PlayOneShot(gameThemeMusic);
    }
}
