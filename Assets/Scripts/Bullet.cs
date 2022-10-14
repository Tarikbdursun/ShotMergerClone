using System;
using Managers;
using PlayerScripts;
using UnityEngine;

namespace DefaultNamespace
{
    public class Bullet : MonoBehaviour
    {
        public float bulletSpeed = 5;

        [SerializeField] private float forwardLimit = 50.0f;
        public Transform bulletParent;
        private Vector3 startPos = new Vector3();
        /// <summary>
        /// TODO BURASI DÜZGÜN ÇALIŞMIYOR FIXLE ACİL !!! ACİL !!!
        /// </summary>
        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            if (IsEnable()) BulletMovement();
            DeactiveBullet();
        }

        private void BulletMovement()
        {
            transform.position += transform.forward * bulletSpeed * Time.deltaTime;
        }

        private bool IsEnable()
        {
            return gameObject.activeSelf;
        }

        private void DeactiveBullet()
        {
            if (transform.localPosition.z > startPos.z+forwardLimit)
            {
                
                gameObject.SetActive(false);
                transform.SetParent(bulletParent);
                transform.localPosition = Vector3.zero;
            }
        }
    }
}