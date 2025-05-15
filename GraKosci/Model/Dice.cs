namespace GraKosci.Model
{
    public class Dice
    {
        public int Value { get; set; }
        public string ImagePath => $"dice_{Value}.png";
        public bool IsSelected { get; set; }
    }
}
