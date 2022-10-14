using System;
using MoreMountains.NiceVibrations;
using UnityEngine;

namespace Managers
{
    public class HapticManager : MonoBehaviour
    {
        #region SINGLETON

        public static HapticManager instance;

        private void InitSingleton()
        {
            if (instance==null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        #endregion

        private void Awake()
        {
            InitSingleton();
        }

        public bool HapticAllowed = true;

        private void Update()
        {
            if (HapticAllowed)
            {
                MMVibrationManager.SetHapticsActive(true);
            }
            else
            {
                MMVibrationManager.SetHapticsActive(false);
            }
        }
        
        public void Fail()
        {
            MMVibrationManager.Haptic(HapticTypes.Failure);
        }

        public void Success()
        {
            MMVibrationManager.Haptic(HapticTypes.Success);
        }

        public void None()
        {
            MMVibrationManager.Haptic(HapticTypes.None);
        }

        public void Selection()
        {
            MMVibrationManager.Haptic(HapticTypes.Selection);
        }

        public void Warning()
        {
            MMVibrationManager.Haptic(HapticTypes.Warning);
        }

        public void HeavyImpact()
        {
            MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
        }

        public void LightImpact()
        {
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }

        public void MediumImpact()
        {
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        }

        public void RigidImpact()
        {
            MMVibrationManager.Haptic(HapticTypes.RigidImpact);
        }

        public void SoftImpact()
        {
            MMVibrationManager.Haptic(HapticTypes.SoftImpact);
        }
    }
}