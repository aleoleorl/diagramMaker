using Accessibility;
using diagramMaker.helpers.containers;
using diagramMaker.helpers.enumerators;
using diagramMaker.parameters;
using System.Linq;

namespace diagramMaker.managers.DefaultPreparation
{
    public class LayerControl
    {
        DataHub data;

        public LayerControl(DataHub data)
        {
            this.data = data;
        }

        public void ReDraw()
        {
            int _currZ = 0;
            for (int _i = 0; _i < data.layerInfoItems.Count; _i++)
            {
                if (data.layerInfoItems[_i].type == ENavigType.Item)
                {
                    data.items[data.GetItemIndexByID(data.layerInfoItems[_i].itemId)].ValueChanger(
                        EBindParameter.Z,
                        _i.ToString());
                    _currZ++;
                }
            }
            for (int _i = 0; _i < data.layerInfoDefPanels.Count; _i++)
            {
                data.items[data.GetItemIndexByID(data.layerInfoDefPanels[_i])].ValueChanger(
                    EBindParameter.Z,
                    (_i + _currZ).ToString());
            }
        }

        public void ClearUnexistItems()
        {

        }
    }
}