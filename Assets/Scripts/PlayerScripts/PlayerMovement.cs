using Managers;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerMovement : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Transform sideMovementRoot;
        [SerializeField] private Transform leftLimit;
        [SerializeField] private Transform rightLimit;

        [SerializeField] private float sideMovementSensitivity;
        [SerializeField] private float sideMovementLerpSpeed;
        [SerializeField] private float forwardSpeed;

        
        private Vector2 inputDrag;
        private Vector2 previousMousePosition;

        private float leftLimitX => leftLimit.localPosition.x;
        private float rightLimitX => rightLimit.localPosition.x;

        private float sideMovementTarget = 0;
        
        private float currentMoveSpeed;
        public float ForwardSpeed => forwardSpeed;
        private float _backToFrictionSpeed = 5f;
        private bool _canBackToZeroFriction = false;
        private float _friction = 0;

        private Vector2 mousePositionCM // Providing the same experience to everyone
        {
            get
            {
                Vector2 pixels = Input.mousePosition;
                var inches = pixels / Screen.dpi;
                var centimetres = inches * 2.54f; // 1 inch = 2.54 cm

                return centimetres;
            }
        }
        
        private GameManager _gameManager=>GameManager.instance;

        #endregion

        #region Singleton

        public static PlayerMovement instance;

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

        private void Update()
        {
            if (_gameManager.isGameStarted)
            {
                HandleForwardMovement();
                HandleSideMovement();
                BackToFriction();
            }
            HandleInput();
            
        }

        private void HandleForwardMovement()
        {
            currentMoveSpeed += Time.deltaTime * 10f;
            currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, currentMoveSpeed, ForwardSpeed);
            var speed = (currentMoveSpeed - _friction);
            var playerPos = transform.position; 
            playerPos = Vector3.MoveTowards(playerPos,
                new Vector3(playerPos.x, playerPos.y, playerPos.z + 1f),
                Time.deltaTime * speed);
            transform.position = playerPos;
        }
        
        public void SetFriction(float percentile, float backToSpeed, bool backToZeroFriction = true)
        {
            _friction = forwardSpeed * (percentile / 100f);
            _canBackToZeroFriction = backToZeroFriction;
            _backToFrictionSpeed = backToSpeed;
        }

        private void BackToFriction()
        {
            if (!_canBackToZeroFriction) return;

            _friction = Mathf.MoveTowards(_friction, 0, Time.deltaTime * _backToFrictionSpeed);

            if (_friction <= 0)
                _canBackToZeroFriction = false;
        }

        private void HandleSideMovement()
        {
            sideMovementTarget += inputDrag.x * sideMovementSensitivity;
            sideMovementTarget = Mathf.Clamp(sideMovementTarget, leftLimitX, rightLimitX);

            var localPos = sideMovementRoot.localPosition;

            localPos.x = Mathf.Lerp(localPos.x, sideMovementTarget, Time.deltaTime * sideMovementLerpSpeed);

            sideMovementRoot.localPosition = localPos;
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                previousMousePosition = mousePositionCM;
            }

            if (Input.GetMouseButton(0))
            {
                var deltaMouse = (Vector2) mousePositionCM - previousMousePosition;
                inputDrag = deltaMouse;
                previousMousePosition = mousePositionCM;
            }
            else
            {
                inputDrag = Vector2.zero;
            }
        }
    }
}