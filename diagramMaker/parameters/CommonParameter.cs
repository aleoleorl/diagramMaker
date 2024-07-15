using diagramMaker.helpers.containers;
using diagramMaker.helpers.enumerators;
using System;
using System.Collections.Generic;

namespace diagramMaker.parameters
{
    public class CommonParameter : DefaultParameter
    {
        public double AppX { get; set; }
        public double AppY { get; set; }
        public int Id { get; set; }
        public int ParentId { get; set; }
        public Connection Connect { get; set; }
        public string Name { get; set; }        
        public EItem ItemType { get; set; }
        public EItemAttach ItemAttach { get; set; }

        public CommonParameter() 
        {
            Init();
        }
        public CommonParameter(
            double? appX = null,
            double? appY = null,
            int? parentId = null,
            Connection? connect = null,
            string? name = null,
            EItem? itemType = null,
            EItemAttach? itemAttach = null)
        {
            Init();
            if (appX != null)
            {
                AppX = Convert.ToDouble(appX);
            }
            if (appY != null)
            {
                AppY = Convert.ToDouble(appY);
            }
            if (parentId != null)
            {
                ParentId = (int)parentId;
            }

            if (connect != null)
            {
                Connect = new Connection();
                Connect.Users = new List<int>();
                Connect.Users.AddRange(connect.Users);
                Connect.Connectors.AddRange(connect.Connectors);
                Connect.IsConnector = connect.IsConnector;
                Connect.IsUser = connect.IsUser;
                foreach (var _supp in connect.support)
                {
                    Connect.support.Add(_supp.Key, _supp.Value);
                }
            }
            if (name != null)
            {
                Name = name;
            }
            if (itemType != null)
            {
                ItemType = (EItem)itemType;
            }
            if (itemAttach != null)
            {
                ItemAttach = (EItemAttach)itemAttach;
            }
        }

        private void Init()
        {
            AppX = 0;
            AppY = 0;
            ParentId = -1;
            Connect = new Connection();
            Connect.Users = new List<int>();
            Connect.Connectors = new List<int>();
            Name = "";
            ItemType = EItem.Default;
            ItemAttach = EItemAttach.Menu;
        }

        public override CommonParameter Clone()
        {
            /* 
             * Warning: 
             * Common parameters contains a lot of data that should be done in constructor, like Id or ParentId. 
             * That's why simple clone method is not recommended
             */
            CommonParameter _tmp = (CommonParameter)this.MemberwiseClone();
            _tmp.Connect = new Connection();
            _tmp.Connect.IsConnector = Connect.IsConnector;
            _tmp.Connect.IsUser = Connect.IsUser;
            _tmp.Connect.Users.AddRange(Connect.Users);
            _tmp.Connect.Connectors.AddRange(Connect.Connectors);
            _tmp.Connect.support = new Dictionary<EConnectorSupport, int>();

            return _tmp;
        }
    }
}