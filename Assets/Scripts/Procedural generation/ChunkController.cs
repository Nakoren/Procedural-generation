using UnityEngine;

public class ChunkController : MonoBehaviour
{
    public Vector2 chunkIndex;

    public void DestroyChunk()
    {
        Destroy(this.gameObject);
    }
}
