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


        private bool startTargetFollowing;
        private bool endTargetFollowing;
        private bool startMoveOn=true;
        private bool endMove;
        
        private float startMoveTimer;
        private float startMoveDuration = 1f;

        private float endMoveTimer;
        private float endMoveDuration = 1f;

        private Vector3 endPos;
        private Vector3 endLastPos;
        private Vector3 startPos;
        private Vector3 startLastPos;

        private Quaternion startRot;
        private Quaternion startLastRot;
        private Quaternion endRot;
        private Quaternion endLastRot;
        
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
        }

        private void LateUpdate()
        {
            if (GameManager.instance.isGameStarted && endMove)
            {
                EndMove();
            }
            if (GameManager.instance.isGameStarted)
            {
                xPos = Mathf.Lerp(transform.position.x, target.transform.position.x, Time.deltaTime * camSmoothness);
                transform.position = new Vector3(xPos, target.transform.position.y, target.transform.position.z);
            }
        }
        //StartMove yok

        //Oyun tamamen bittiğinde bu olacak TODO değerleri değiştir
        public void SetEndMove()
        {
            endPos = new Vector3(0, 6.5f, -9.5f);
            endLastPos = new Vector3(0, 4.2f, -5.5f);
            endRot = Quaternion.Euler(20f, 0, 0);
            endLastRot = Quaternion.Euler(18.5f, 0, 0);
            endMove = true;
        }
        

        private void EndMove()
        {
            if (endMoveTimer < endMoveDuration)
            {
                endMoveTimer += Time.deltaTime;
                mainCamera.transform.localPosition = Vector3.Lerp(endPos, endLastPos, endMoveTimer / endMoveDuration);
                mainCamera.transform.rotation = Quaternion.Lerp(endRot, endLastRot, endMoveTimer / endMoveDuration);
            }
            else
            {
                mainCamera.transform.localPosition = endLastPos;
                //endTargetFollowing = true;
                endMoveTimer = 0;
                endMove = false;
            }
        }

        public void CameraReset()
        {
            transform.position=Vector3.zero;
            mainCamera.transform.localPosition = startPos;
            mainCamera.transform.localRotation = startRot;
        }
    }
}