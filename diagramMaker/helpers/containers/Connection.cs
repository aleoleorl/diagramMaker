using diagramMaker.helpers.enumerators;
using System.Collections.Generic;

namespace diagramMaker.helpers.containers
{
    public class Connection
    {
        public bool IsConnector { get; set; }
        public bool IsUser { get; set; }
        public List<int> Users { get; set; }
        public List<int> Connectors { get; set; }

        public Dictionary<EConnectorSupport, int> support;
        public int GroupID { get; set; }
        private static int ID = 1;

        public static int GetID()
        {
            return ID++;
        }

        public Connection() 
        {             
            IsUser = false;
            Users = new List<int>();
            IsConnector = false;
            Connectors = new List<int>();            
            support = new Dictionary<EConnectorSupport, int>();
        }
    }
}