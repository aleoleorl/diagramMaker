using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace diagramMaker.parameters
{
    public class ContentParameter : DefaultParameter
    {
        public string? content;
        public HorizontalAlignment? horAlign;
        public VerticalAlignment? verAlign;

        public ContentParameter(
            string? content = null,
            HorizontalAlignment? horAlign = null,
            VerticalAlignment? verAlign = null)
        {
            this.content = content;
            this.horAlign = horAlign;
            this.verAlign = verAlign;
        }
    }
}
