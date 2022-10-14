using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayerScripts;
using UnityEngine;

namespace DefaultNamespace
{
    public class Cube : MonoBehaviour,IShootable
    {
        #region Variables
        
        public bool cubeCanShoot;
        //Shoot
        [SerializeField] private Bullet bullet;
        [SerializeField] private int bulletCount;
        [SerializeField] private Transform bulletSpawnPoint;
        

        private List<Bullet> bulletPool = new List<Bullet>();
        private int bulletIndex = 0;
        
        //Stack
        [SerializeField] private Transform stackPoint;
        [SerializeField] private Collider collider;
        public bool isStacked;
        private bool isFull;
        //Effect
        [SerializeField] private EffectType effectType;
        [SerializeField] private int effect;
        
        //Cache
        private Player _player=>Player.instance;
        #endregion

        #region UnityEvents

        private void Start()
        {
            if (effectType == EffectType.multiplier)
            {
                SpawnBullets();
                StartCoroutine(Shoot());
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cube"))
            {
                var cube = other.GetComponent<Cube>();
                
                if(cube.isStacked) return;
                if(!isStacked&&!cube.isStacked) return;
                collider.enabled = false;
                
                var cubeParent = other.transform.parent;
                if (isFull)
                {
                    Transform lastCube =_player.cubes[_player.lastCubeIndex].stackPoint;
                    cubeParent.SetParent(lastCube);
                }
                else
                {
                    cubeParent.SetParent(stackPoint);
                }
                //cubeParent.SetParent(stackPoint);
                cubeParent.localPosition=Vector3.zero;
                var distance= (cubeParent.transform.localPosition.x-other.transform.localPosition.x); //Mathf.Abs
                var newPos = cubeParent.localPosition;
                newPos.x += distance;
                cubeParent.localPosition = newPos;
                for (int i = 0; i < cubeParent.childCount-1; i++)
                {
                    var newCube = cubeParent.GetChild(i).GetComponent<Cube>();
                    newCube.isStacked = true;
                    if (newCube.effectType == EffectType.increaser) 
                        _player.time /= 2;
                }
                collider.enabled = true;
                isFull = true;
                _player.cubes.Add(cube); //Find the most far cube from the player
            }
        }

        #endregion

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
                if (isStacked && effectType==EffectType.multiplier)
                {
                    bulletPool[bulletIndex].gameObject.SetActive(true);
                    bulletPool[bulletIndex].transform.SetParent(null);
                    GetNextIndex();
                }

                yield return new WaitForSeconds(_player.time);
            }
        }

        public void GetNextIndex()
        {
            bulletIndex = bulletIndex >= bulletPool.Count - 1 ? 0  : bulletIndex + 1;
        }
    }

    public enum EffectType
    {
        increaser,
        multiplier
    }
}