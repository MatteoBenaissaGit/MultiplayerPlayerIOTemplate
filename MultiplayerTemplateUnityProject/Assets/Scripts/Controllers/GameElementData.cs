using UnityEngine;

namespace Controllers
{
    public class GameElementData
    {
        public string Type { get; internal set; }
        public string ID { get; internal set; }
        public string ElementOwnerID { get; internal set; }
        public Vector2Int Coordinates { get; internal set; }
        public int Team { get; internal set; }
    }
}