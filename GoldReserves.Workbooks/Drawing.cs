namespace GoldReserves.Workbooks
{
    public abstract class Drawing
    {
        protected Drawing()
        {

        }

        public abstract DrawingType DrawingType { get; }
        public Worksheet Worksheet { get; internal set; }
    }
}
