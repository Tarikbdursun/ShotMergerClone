using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Managers;
using UnityEngine;

namespace PlayerScripts
{
    public class Player : MonoBehaviour, IShootable
    {
        #region Variables

        //Shoot
        [SerializeField] private Bullet bullet;
        [SerializeField] private int bulletCount;
        [SerializeField] private Transform bulletSpawnPoint;

        public Transform BulletSpawnPoint => bulletSpawnPoint;

        private List<Bullet> bulletPool = new List<Bullet>();
        private int bulletIndex = 0;


        //Time
        public float time = 1.0f;

        //Stack
        [SerializeField] private Transform stackPoint;

        //Cubes List
        public List<Cube> cubes = new List<Cube>();
        public int lastCubeIndex => cubes.Count - 1;

        //Cache
        private PlayerMovement _playerMovement => PlayerMovement.instance;
        private GameManager _gameManager => GameManager.instance;

        #endregion

        #region SINGLETON

        public static Player instance;

        private void InitSingleton()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        #endregion

        #region Unity Events

        private void Start()
        {
            SpawnBullets();
            StartCoroutine(Shoot());
        }

        private void Awake()
        {
            InitSingleton();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cube"))
            {
                var cube = other.GetComponent<Cube>();
                var cubeParent = other.transform.parent;
                if (cube.isStacked) return;

                cubeParent.SetParent(stackPoint);
                cubeParent.localPosition = Vector3.zero;
                var distance = (cubeParent.transform.localPosition.x - other.transform.localPosition.x);
                var newPos = cubeParent.localPosition;
                newPos.x += distance;
                cubeParent.localPosition = newPos;
                for (int i = 0; i < cubeParent.childCount - 1; i++) //except canvas
                {
                    cubeParent.GetChild(i).GetComponent<Cube>().isStacked = true;
                }
            }
            else if (other.CompareTag("Barrel"))
            {
                Debug.Log("Baller"); //Öldün çık
                //DoTween ile güzel bi ölüm olsun //Ya da bi animasyon yap
            }
        }

        #endregion


        #region IShootable Operations

        public void SpawnBullets()
        {
            for (int i = 0; i < bulletCount; i++)
            {
                var spawnBullet = Instantiate(bullet, bulletSpawnPoint);
                bulletPool.Add(spawnBullet);
                spawnBullet.gameObject.SetActive(false);
            }
        }

        public IEnumerator Shoot()
        {
            while (true)
            {
                if (_gameManager.isGameStarted)
                {
                    bulletPool[bulletIndex].gameObject.SetActive(true);
                    bulletPool[bulletIndex].transform.SetParent(null);
                    GetNextIndex();
                }

                yield return new WaitForSeconds(time);
            }
        }

        public void GetNextIndex()
        {
            bulletIndex = bulletIndex >= bulletPool.Count - 1 ? 0  : bulletIndex + 1;
        }

        #endregion


        
    }
}