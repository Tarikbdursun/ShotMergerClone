using System;
using LevelScripts;
using PlayerScripts;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        
        public bool isGameStarted;
        [SerializeField] private LevelList levelList;

        private PlayerMovement _playerMovement => PlayerMovement.instance;
        private Player _player => Player.instance;
        private UIManager _uiManager=>UIManager.instance;
        private CameraManager _cameraManager=>CameraManager.instance;
       
        #region SINGLETON
        public static GameManager instance;

        private void InitSingleton()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }
        #endregion

        private void Awake()
        {
            InitSingleton();
        }
        

        void Start()
        {
           LevelManager.SetLevelManager(levelList);
        }

        public void GameStart()
        {
            isGameStarted = true;
            _playerMovement.CanPlayerMove = true;
            _uiManager.GameStart();
        }

        public void GameEnd(bool won)
        {
            isGameStarted = false;
            _uiManager.GameEnd(won);
        }

        public void NextLevel()
        {
            LevelManager.NextLevel();
           _uiManager.NextLevel();
           _player.PlayerReset();
           _cameraManager.CameraReset();
        }

        public void RestartLevel()
        {
            LevelManager.RestartLevel();
            _uiManager.RestartLevel();
            _player.PlayerReset();
            _cameraManager.CameraReset();
        }
    }
}