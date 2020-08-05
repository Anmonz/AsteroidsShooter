using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.AndryKram
{
    public class AudioButton : MonoBehaviour
    {
        [SerializeField] private GameObject _audioOff;
        [SerializeField] private GameObject _audioOn;

        private void Awake()
        {
            if (PlayerPrefs.HasKey("MasterVolume"))
            {
                if(PlayerPrefs.GetFloat("MasterVolume") == 0)
                {
                    _audioOn.SetActive(true);
                    _audioOff.SetActive(false);
                }
                else
                {
                    _audioOn.SetActive(false);
                    _audioOff.SetActive(true);
                }
            }
        }
    }
}
