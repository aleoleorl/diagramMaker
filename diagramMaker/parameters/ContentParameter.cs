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
        public bool IsDigitsOnly;

        public ContentParameter(
            string? content = null,
            HorizontalAlignment? horAlign = null,
            VerticalAlignment? verAlign = null,
            bool isTextChanged = false,
            EBindParameter bindParameter = EBindParameter.None,
            int bindID = -1,
            bool isDigitsOnly = false)
        {
            this.content = content;
            this.horAlign = horAlign;
            this.verAlign = verAlign;
            this.IsTextChanged = isTextChanged;
            this.BindParameter = bindParameter;
            this.BindID = bindID;
            this.IsDigitsOnly = isDigitsOnly;
        }
    }
}
