using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diagramMaker.managers.DefaultPreparation
{
    public enum Itemtype
    {
        None,
        DefaultPanel,
        item
    }
    public struct LayerInfo
    {
        public Itemtype itemType;
        public int itemId;
        public int layerId;
        public int z;

        public LayerInfo(Itemtype itemType, int itemId, int layerId, int z)
        {
            this.itemType = itemType;
            this.itemId = itemId;
            this.layerId = layerId;
            this.z = z;
        }
    }
    public class LayerControl
    {
        DataHub data;

        public LayerControl(DataHub data) 
        {
            this.data = data;
        }
    }
}
