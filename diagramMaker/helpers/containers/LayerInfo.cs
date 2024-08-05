using System.Dynamic;

namespace diagramMaker.helpers.containers
{
    public enum ENavigType
    {
        None,
        Layer,
        Item
    }
    public struct LayerInfo
    {
        private static int PersonalId = 1;
        public static int GetPersonalId 
        { get
            {
                return PersonalId;
            }
        }
        //personalId: internal number of a layer
        public int personalId;
        //name: name of a layer. By default = "Layer " + personalId.ToString()
        public string name;
        //type: is it an item or a layer
        public ENavigType type;
        //parentLayerId: number of a parent personalId (-1 for menuNavigationPanelID)
        public int parentItemId;
        //itemId: id of item included into navigation panel
        public int itemId;
        //itemOnStageId: id of item on stage for items (-1 for layers)
        public int itemOnStageId;
        public LayerInfo(
            ENavigType type,
            int itemId,
            int parentItemId = -1,
            int itemOnStageId = -1)
        {
            this.personalId = PersonalId++;
            this.type = type;
            this.itemId = itemId;
            this.parentItemId = parentItemId;
            this.itemOnStageId = itemOnStageId;
            name = "Layer " + personalId.ToString();
        }
    }
}
