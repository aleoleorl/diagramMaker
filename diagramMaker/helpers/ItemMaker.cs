using diagramMaker.items;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diagramMaker.helpers
{
    public class ItemMaker
    {
        public EItem Item;
        public Dictionary<string, EParameter> Props;
        public List<ItemMaker> Children;
        public List<ItemMaker> Connector;

        public ItemMaker() 
        {
            Item = EItem.Default;
            Props = new Dictionary<string, EParameter>();
            Children = new List<ItemMaker>();
            Connector = new List<ItemMaker>();
        }

    }
}
