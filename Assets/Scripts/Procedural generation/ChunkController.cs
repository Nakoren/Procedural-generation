using UnityEngine;

public class ChunkController : MonoBehaviour
{
    public Vector2 chunkIndex;

    public void DestroyChunk()
    {
        //Когда разберёшься с добавлением объектов их удаление тоже тут сделай
        Destroy(this.gameObject);
    }
}
