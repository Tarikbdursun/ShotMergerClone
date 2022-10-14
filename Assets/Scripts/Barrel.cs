using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class Barrel : MonoBehaviour
    {
        [SerializeField] private int health;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private ParticleSystem smokeParticle;

        private void Awake()
        {
            healthText.text = health.ToString();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Bullet"))
            {
                if(!HasHealth()) return;
                health--;
                healthText.text = health.ToString();
                if (!HasHealth())
                {
                    smokeParticle.transform.SetParent(null);
                    smokeParticle.Play();
                    gameObject.SetActive(false);
                }
            }
        }

        private void DeactivateBarrel()
        {
            gameObject.SetActive(false);
        }
        
        private bool HasHealth()
        {
            return health > 0;
        }
    }
}