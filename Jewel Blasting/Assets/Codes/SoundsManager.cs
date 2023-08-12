using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;
    public AudioSource[] sounds;
    private void Awake()
    {
        instance = this;
    }
    public void PlaySound(int whichSound, float pich = 1)
    {
        sounds[whichSound].Play();
        sounds[whichSound].pitch = pich;
    }
}
