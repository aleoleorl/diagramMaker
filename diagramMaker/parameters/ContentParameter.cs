using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using diagramMaker.helpers;

namespace diagramMaker.parameters
{
    public class ContentParameter : DefaultParameter
    {
        public string? content;
        public HorizontalAlignment? horAlign;
        public VerticalAlignment? verAlign;

        public bool IsTextChanged = false;
        public EBindParameter BindParameter;
        public int BindID;

        public ContentParameter(
            string? content = null,
            HorizontalAlignment? horAlign = null,
            VerticalAlignment? verAlign = null,
            bool IsTextChanged = false,
            EBindParameter BindParameter = EBindParameter.None,
            int BindID = -1)
        {
            this.content = content;
            this.horAlign = horAlign;
            this.verAlign = verAlign;
            this.IsTextChanged = IsTextChanged;
            this.BindParameter = BindParameter;
            this.BindID = BindID;
        }
    }
}
