using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using PlayerScripts;
using UnityEngine;

namespace DefaultNamespace
{
    public class Cube : MonoBehaviour
    {
        #region Variables

        public bool cubeCanShoot;

        //Shoot
        [SerializeField] private Bullet bullet;
        [SerializeField] private int bulletCount;
        [SerializeField] private Transform bulletSpawnPoint;
        private List<Bullet> bulletPool = new List<Bullet>();
        private int bulletIndex = 0;
        private float cubeShootRate = 1;

        //Stack
        [SerializeField] private Transform stackPoint;
        [SerializeField] private Collider collider;
        public bool isStacked;
        private bool isFull;

        //Cache
        private Player _player => Player.instance;
        private CubeParent cubeParent;
        private Transform parentTransform;
        private HapticManager _hapticManager => HapticManager.instance;
        private GameManager _gameManager => GameManager.instance;

        #endregion

        #region UnityEvents

        private void Start()
        {
            StartSettings();
            SpawnBullets();
            StartCoroutine(Shoot());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cube"))
            {
                var cube = other.GetComponent<Cube>();

                CubeStack(cube, other);
            }
            else if (other.CompareTag("Barrel"))
            {
                ObstacleTrigger(other);
            }
        }

        #endregion

        #region Trigger Methods

        private void CubeStack(Cube cube, Collider other)
        {
            if (cube.isStacked) return;
            if (!isStacked && !cube.isStacked) return;
            collider.enabled = false;

            var parent = other.transform.parent;
            if(!parent) return;
            var parenOfcubes = parent.GetComponent<CubeParent>();
            if (!parenOfcubes) return;
            
            if (isFull)
            {
                Transform lastCube = _player.cubes[_player.cubes.Count - 1].stackPoint;
                parent.SetParent(lastCube);
            }
            else
            {
                parent.SetParent(stackPoint);
            }

            parent.transform.localPosition = Vector3.zero;
            var distance = (parent.localPosition.x - other.transform.localPosition.x);
            var newPos = parent.localPosition;
            newPos.x += distance;
            parent.localPosition = newPos;
            for (int i = 0; i < parent.transform.childCount - 1; i++)
            {
                var newCube = parent.transform.GetChild(i).GetComponent<Cube>();
                newCube.isStacked = true;
            }

            RiseEffect(parenOfcubes);
            collider.enabled = true;
            isFull = true;
            _player.cubes.Add(cube);
            _hapticManager.Success();
        }

        private void ObstacleTrigger(Collider other)
        {
            transform.SetParent(LevelManager.GetCurrentLevel().transform);
            var direction = (other.transform.position - transform.position).normalized;
            var targetPos = transform.position - direction;
            transform.DOMove(targetPos, .3f);
            DecreaseEffect();

            _player.cubes.Remove(this);
            if (cubeParent.transform.childCount == 1)
            {
                cubeParent.transform.SetParent(LevelManager.GetCurrentLevel().transform);
                cubeParent.gameObject.SetActive(false);
            }
            else
            {
                for (int i = 0; i < cubeParent.transform.childCount - 1; i++)
                {
                    if (cubeParent.transform.GetChild(i).gameObject.activeSelf)
                    {
                        var newTextPos = cubeParent.infoText.transform.localPosition;
                        newTextPos.x += (cubeParent.transform.GetChild(i).transform
                            .localPosition.x - cubeParent.infoText.transform.localPosition.x);

                        cubeParent.infoText.transform.localPosition = newTextPos;
                    }
                }
            }

            _hapticManager.Fail();
        }

        #endregion

        #region StartSettings

        private void StartSettings()
        {
            if (transform.parent != null)
                parentTransform = transform.parent;
            cubeParent = parentTransform.GetComponent<CubeParent>();
            if (cubeParent.effect == Effect.x2)
            {
                cubeShootRate /= 2f;
            }
            else if (cubeParent.effect == Effect.x3)
            {
                cubeShootRate /= 3f;
            }
        }

        #endregion

        #region Effect Methods

        private void RiseEffect(CubeParent parent)
        {
            switch (parent.effect)
            {
                case Effect.plus1:
                    _player.BulletCountPerSecond++;
                    break;
                case Effect.plus3:
                    _player.BulletCountPerSecond += 3;
                    break;
                case Effect.x2:
                    _player.BulletCountPerSecond = _player.BulletCountPerSecond * 2 + 2;
                    break;
                case Effect.x3:
                    _player.BulletCountPerSecond = _player.BulletCountPerSecond * 3 + 3;
                    break;
            }
        }

        private void DecreaseEffect()
        {
            switch (cubeParent.effect)
            {
                case Effect.plus1:
                    _player.BulletCountPerSecond--;
                    break;
                case Effect.plus3:
                    _player.BulletCountPerSecond--;
                    cubeParent.EffectValue--;
                    cubeParent.infoText.text = "+" + cubeParent.EffectValue;
                    break;
                case Effect.x2:
                    _player.BulletCountPerSecond = (_player.BulletCountPerSecond - 2) / 2;
                    cubeParent.EffectValue--;
                    cubeParent.infoText.text = "x" + cubeParent.EffectValue;
                    break;
                case Effect.x3:
                    _player.BulletCountPerSecond = (_player.BulletCountPerSecond - 3) / 3;
                    cubeParent.EffectValue--;
                    cubeParent.infoText.text = "x" + cubeParent.EffectValue;
                    break;
            }
        }

        #endregion

        #region Shooting

        public void SpawnBullets()
        {
            for (int i = 0; i < bulletCount; i++)
            {
                var spawnBullet = Instantiate(bullet, bulletSpawnPoint);
                bulletPool.Add(spawnBullet);
                spawnBullet.bulletParent = bulletSpawnPoint;
                spawnBullet.gameObject.SetActive(false);
            }
        }

        public IEnumerator Shoot()
        {
            while (true)
            {
                if (isStacked && _gameManager.isGameStarted)
                {
                    bulletPool[bulletIndex].gameObject.SetActive(true);
                    bulletPool[bulletIndex].transform.SetParent(LevelManager.GetCurrentLevel().transform);
                    GetNextIndex();
                }

                yield return new WaitForSeconds(cubeShootRate);
            }
        }

        public void GetNextIndex()
        {
            bulletIndex = bulletIndex >= bulletPool.Count - 1 ? 0 : bulletIndex + 1;
        }

        #endregion
    }
}