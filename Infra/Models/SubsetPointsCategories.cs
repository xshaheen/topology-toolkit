namespace Infra.Models
{
    public class SubsetPointsCategories
    {
        public int Index { get; set; }
        public string Subset { get; set; }

        public string Limit { get; set; }
        public string Closure { get; set; }
        public string Interior { get; set; }
        public string Exterior { get; set; }
        public string Boundary { get; set; }

        public double Accuracy { get; set; }
    }
}