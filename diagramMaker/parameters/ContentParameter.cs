using diagramMaker.helpers;
using System.Windows;

namespace diagramMaker.parameters
{
    public class ContentParameter : DefaultParameter
    {
        public string? Content { get; set; }
        public HorizontalAlignment? HorAlign { get; set; }
        public VerticalAlignment? VerAlign { get; set; }

        public bool isTextChanged = false;
        public bool IsTextChanged 
        {
            get { return isTextChanged; }
            set { isTextChanged = value; }
        }
        public EBindParameter BindParameter { get; set; }
        public int BindID { get; set; }
        public bool IsDigitsOnly { get; set; }
        public int Count { get; set; }

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
            this.Content = content;
            this.HorAlign = horAlign;
            this.VerAlign = verAlign;
            this.IsTextChanged = isTextChanged;
            this.BindParameter = bindParameter;
            this.BindID = bindID;
            this.IsDigitsOnly = isDigitsOnly;
            this.Count = count;
        }

        public override ContentParameter Clone()
        {
            return (ContentParameter)this.MemberwiseClone();
        }
    }
}
