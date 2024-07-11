using diagramMaker.helpers;

namespace diagramMaker.parameters
{
    public class CommonParameter : DefaultParameter
    {
        public double AppX { get; set; }
        public double AppY { get; set; }
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int ConnectorId { get; set; }
        public string Name { get; set; }        
        public EItem ItemType { get; set; }
        public EItemAttach ItemAttach { get; set; }

        public CommonParameter() 
        {
            AppX = 0;
            AppY = 0;
            ParentId = -1;
            ConnectorId = -1;
            Name = "";
            ItemType = EItem.Default;
            ItemAttach = EItemAttach.Menu;
        }
        public CommonParameter(
            double appX, 
            double appY, 
            int parentId, 
            int connectorId, 
            string name, 
            EItem itemType,
            EItemAttach itemAttach)
        {
            AppX = appX;
            AppY = appY;
            ParentId = parentId;
            ConnectorId = connectorId;
            Name = name;
            ItemType = itemType;
            ItemAttach = itemAttach;
        }

        public override CommonParameter Clone()
        {
            return (CommonParameter)this.MemberwiseClone();
        }
    }
}
