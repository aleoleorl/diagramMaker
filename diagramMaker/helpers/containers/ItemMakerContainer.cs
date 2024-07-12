using System.Collections.Generic;
using diagramMaker.helpers.enumerators;

namespace diagramMaker.helpers.containers
{
    public class ItemMakerContainer
    {
        public EItem Item;
        public Dictionary<string, EParameter> Props;
        public List<ItemMakerContainer> Children;
        public List<ItemMakerContainer> Connector;
        public List<EEvent> Events;

        public ItemMakerContainer()
        {
            Item = EItem.Default;
            Props = new Dictionary<string, EParameter>();
            Children = new List<ItemMakerContainer>();
            Connector = new List<ItemMakerContainer>();
            Events = new List<EEvent>();
        }
    }
}