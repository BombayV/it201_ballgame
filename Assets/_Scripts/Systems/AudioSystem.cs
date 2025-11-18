using UnityEngine;

public class AudioSystem : Singleton<AudioSystem>
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    public void PlayMusic(float volume = 0.1f)
    {
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }
    
    public void ResumeMusic()
    {
        musicSource.UnPause();
    }
    
    public void StopMusic()
    {
        musicSource.Stop();
    }
    
    public void PlaySound(float volume = 0.1f)
    {
        sfxSource.PlayOneShot(sfxSource.clip, volume);
    }
}
