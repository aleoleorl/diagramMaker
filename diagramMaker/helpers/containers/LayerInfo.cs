namespace diagramMaker.helpers.containers
{
    public struct LayerInfo
    {
        public int layerId;
        public int itemId;
        public int curZ;
        public LayerInfo(int layerId, int itemId, int curZ)
        {
            this.layerId = layerId;
            this.itemId = itemId;
            this.curZ = curZ;
        }
    }
}
