using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace com.AndryKram
{
    /// <summary>
    /// Глушит звук мастер-группы AudioMixer
    /// </summary>
    public class AudioMixerManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;//микшер

        /// <summary>
        /// Проверяет установленное значение MasterVolume
        /// </summary>
        private void Awake()
        {
            if(PlayerPrefs.HasKey("MasterVolume"))
            {
                SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
            }
        }

        /// <summary>
        /// Глушит звук мастер микшера
        /// </summary>
        public void MuteMasterVolume()
        {
            PlayerPrefs.SetFloat("MasterVolume", -80f);
            _audioMixer.SetFloat("MasterVolume", -80f);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// возобнавляет звук мастер микшера
        /// </summary>
        public void RemuteMasterVolume()
        {
            PlayerPrefs.SetFloat("MasterVolume", 0f);
            _audioMixer.SetFloat("MasterVolume", 0f);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Устанавливает звук мастер микшера
        /// </summary>
        public void SetMasterVolume(float value)
        {
            PlayerPrefs.SetFloat("MasterVolume", value);
            _audioMixer.SetFloat("MasterVolume", value);
            PlayerPrefs.Save();
        }
    }
}
