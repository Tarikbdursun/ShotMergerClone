using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = System.Random;

namespace PlayerScripts
{
    public class Player : MonoBehaviour
    {
        #region Variables

        //Shoot
        [SerializeField] private Bullet bullet;
        [SerializeField] private int bulletCount;
        [SerializeField] private Transform bulletSpawnPoint;

        private List<Bullet> bulletPool = new List<Bullet>();
        private int bulletIndex = 0;

        //Bullet Count Per Second
        public float BulletCountPerSecond = 1;

        //Time
        private float playerShootRate = 1;

        //Player UI
        [SerializeField] private TextMeshProUGUI shootInfoText;


        //Stack
        [SerializeField] private Transform stackPoint;

        //Cubes List
        public List<Cube> cubes = new List<Cube>();
        public int lastCubeIndex => cubes.Count - 1;
        
        //Parts Of Gun
        private Sequence gunParts;
        [SerializeField] private Transform top;
        [SerializeField] private Transform under;
        
        private Vector3 firstPosOfTop => top.transform.localPosition;
        private Vector3 firstPosOfUnder => under.transform.localPosition;
        
        //Cache
        private PlayerMovement _playerMovement => PlayerMovement.instance;
        private GameManager _gameManager => GameManager.instance;
        private HapticManager _hapticManager=>HapticManager.instance;

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

        private void Awake()
        {
            InitSingleton();
        }

        private void Start()
        {
            SpawnBullets();
            StartCoroutine(Shoot());
            SetShootInfo();
            gunParts = DOTween.Sequence();
        }

        private void Update()
        {
            SetShootInfo();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cube"))
            {
                var cube = other.GetComponent<Cube>();
                CubeStack(cube,other);
                _hapticManager.Success();
            }
            else if (other.CompareTag("Barrel"))
            {
                PlayerFail();
                _hapticManager.Fail();
            }
            else if (other.CompareTag("End"))
            {
                PlayerWin();
                _hapticManager.Success();
            }
        }

        #endregion

        #region Operations

        private void CubeStack(Cube cube,Collider other)
        {
            var cubeParent = other.transform.parent;
            if (!cubeParent) return;
            var CubeParentEffect = cubeParent.GetComponent<CubeParent>();
                
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

            cubes.Add(cube);
            Effect(CubeParentEffect);
        }
        private void Effect(CubeParent cubeParent)
        {
            switch (cubeParent.effect)
            {
                case DefaultNamespace.Effect.plus1:
                    BulletCountPerSecond++;
                    break;
                case DefaultNamespace.Effect.plus3:
                    BulletCountPerSecond += 3;
                    break;
                case DefaultNamespace.Effect.x2:
                    BulletCountPerSecond = BulletCountPerSecond * 2 + 2;
                    break;
                case DefaultNamespace.Effect.x3:
                    BulletCountPerSecond = BulletCountPerSecond * 3 + 3;
                    break;
            }
        }

        private void PlayerFail()
        {
            _playerMovement.SetFriction(120, 1f, false);
            _playerMovement.CanPlayerMove = false;
            
            // * MovementOfTop and MovementOfUnder
            
            CameraManager.instance.SetEndMove(false);
            float targetYPosOfTop = UnityEngine.Random.Range(-.75f,-1f );
            float targetXPosOfUnder = -0.7f;
            gunParts.Append(top.DOMoveX(targetYPosOfTop, .5f)
                .OnComplete(() => top.DOMoveX(targetYPosOfTop + 0.3f, .25f)));
            gunParts.Join(under.DOLocalMoveZ(targetXPosOfUnder, .5f)
                .OnComplete(() =>
                {
                    under.DOLocalMove(firstPosOfUnder, .3f);
                    _gameManager.GameEnd(false);
                }));
        }

        private void PlayerWin()
        {
            _playerMovement.CanPlayerMove = false;
            CameraManager.instance.SetEndMove(true);
            _gameManager.GameEnd(true);
        }

        private void SetShootInfo()
        {
            shootInfoText.text = BulletCountPerSecond.ToString() + "/sec";
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
                if (_gameManager.isGameStarted)
                {
                    bulletPool[bulletIndex].gameObject.SetActive(true);
                    bulletPool[bulletIndex].transform.SetParent(LevelManager.GetCurrentLevel().transform);
                    GetNextIndex();
                }

                yield return new WaitForSeconds(playerShootRate);
            }
        }

        public void GetNextIndex()
        {
            bulletIndex = bulletIndex >= bulletPool.Count - 1 ? 0 : bulletIndex + 1;
        }

        #endregion

        #region Reset Player

        public void PlayerReset()
        {
            _playerMovement.transform.position = Vector3.zero;
            transform.localPosition=Vector3.zero;
            foreach (var cube in cubes)
            {
                Destroy(cube.transform.parent.gameObject);
            }
            cubes.Clear();
            BulletCountPerSecond = 1;
            
            top.localPosition = firstPosOfTop;
            under.localPosition = firstPosOfUnder;
        }

        #endregion
    }
}