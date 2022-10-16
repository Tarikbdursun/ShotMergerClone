using DG.Tweening;
using PlayerScripts;
using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private GameObject target;
        public ParticleSystem confettiParticle;

        public float camSmoothness = 6;
        private float xPos = 0;
        
        //Start Settings and EndMove 
        private Sequence gameEndMove;
        
        private bool canEndMove;
        private float endMoveDuration = 1f;

        private Vector3 endPos;
        private Vector3 startPos;
        
        private Quaternion startRot;
        private Quaternion endRot;
        
        private GameManager _gameManager=>GameManager.instance;

        private PlayerMovement _playerMovement => PlayerMovement.instance;

        #region SINGLETON
        public static CameraManager instance;

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

        private void Start()
        {

            startPos = mainCamera.transform.localPosition;
            startRot = mainCamera.transform.localRotation;
            gameEndMove = DOTween.Sequence();
        }

        private void LateUpdate()
        {
            if (canEndMove)
            {
                EndMove();
            }
            if (GameManager.instance.isGameStarted)
            {
                xPos = Mathf.Lerp(transform.position.x, target.transform.position.x, Time.deltaTime * camSmoothness);
                transform.position = new Vector3(xPos, target.transform.position.y, target.transform.position.z);
            }
        }

        public void SetEndMove(bool isWon)
        {
             endPos = new Vector3(0, 8f, -8.4f);
             endRot = Quaternion.Euler(40f, 0, 0);
             canEndMove = true;
             if(isWon)
                 confettiParticle.Play();
        }

        
        private void EndMove()
        {
            gameEndMove.Append(mainCamera.transform.DOLocalMove(endPos, endMoveDuration));
            gameEndMove.Join(mainCamera.transform.DORotateQuaternion(endRot, endMoveDuration)
                .OnComplete(()=>canEndMove=false));
        }

        public void CameraReset()
        {
            transform.position=Vector3.zero;
            mainCamera.transform.localPosition = startPos;
            mainCamera.transform.localRotation = startRot;
        }
    }
}