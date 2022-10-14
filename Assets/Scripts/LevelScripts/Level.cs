using UnityEngine;

namespace LevelScripts
{
    public class Level : MonoBehaviour
    {
        public string levelName;

        public void DestroyLevel()
        {
            Destroy(gameObject);
        }
    }
}