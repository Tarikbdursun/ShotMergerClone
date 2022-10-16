using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class CubeParent : MonoBehaviour
    {
        public Effect effect;
        public TextMeshProUGUI infoText;
        public int EffectValue;
    }

    public enum Effect
    {
        plus1,
        plus3,
        x2,
        x3
    }
}