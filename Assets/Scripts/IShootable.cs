using System.Collections;

namespace DefaultNamespace
{
    public interface IShootable
    {
        void SpawnBullets();
        IEnumerator Shoot();
        void GetNextIndex();
    }
}