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

        public bool isTextChanged = false;
        public EBindParameter bindParameter;
        public int bindID;
        public bool isDigitsOnly;
        public int count;

        public ContentParameter(
            string? content = null,
            HorizontalAlignment? horAlign = null,
            VerticalAlignment? verAlign = null,
            bool isTextChanged = false,
            EBindParameter bindParameter = EBindParameter.None,
            int bindID = -1,
            bool isDigitsOnly = false,
            int count = -1)
        {
            this.content = content;
            this.horAlign = horAlign;
            this.verAlign = verAlign;
            this.isTextChanged = isTextChanged;
            this.bindParameter = bindParameter;
            this.bindID = bindID;
            this.isDigitsOnly = isDigitsOnly;
            this.count = count;
        }

        public override ContentParameter Clone()
        {
            return (ContentParameter)this.MemberwiseClone();
        }
    }
}
