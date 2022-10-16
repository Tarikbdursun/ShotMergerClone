using System;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        #region Panels
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject failPanel;
        [SerializeField] private TextMeshProUGUI levelInfoText;
        
        #endregion
        
        #region SINGLETON
        public static UIManager instance;

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

        public void GameStart() 
        {
            startPanel.SetActive(false);
            hudPanel.SetActive(true);
            levelInfoText.text =$"Level {LevelManager.GetCurrentLevelNumber()}" ;
        }
        
        public void GameEnd(bool won) 
        {
            hudPanel.SetActive(false);
            if (won)
            {
                winPanel.SetActive(true);
                hudPanel.SetActive(false);
            }
            else
            {
                failPanel.SetActive(true);
                hudPanel.SetActive(false);
            }
        }
        public void NextLevel() 
        {
            winPanel.SetActive(false);
            startPanel.SetActive(true);
            hudPanel.SetActive(true);
        }

        public void RestartLevel() 
        {
            failPanel.SetActive(false);
            startPanel.SetActive(true);
            hudPanel.SetActive(true);
        }
    }
}