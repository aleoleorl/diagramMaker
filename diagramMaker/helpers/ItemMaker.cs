using System.Collections.Generic;

namespace diagramMaker.helpers
{
    public class ItemMaker
    {
        public EItem Item;
        public Dictionary<string, EParameter> Props;
        public List<ItemMaker> Children;
        public List<ItemMaker> Connector;
        public List<EEvent> Events;

        public ItemMaker() 
        {
            Item = EItem.Default;
            Props = new Dictionary<string, EParameter>();
            Children = new List<ItemMaker>();
            Connector = new List<ItemMaker>();
            Events = new List<EEvent>();
        }
    }
}