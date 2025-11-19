namespace ConsoleApp;

public class DogSeat : Dog
{
    
    public DogSeat(string name) : base(name)
    {
        
    }

    public void Seat()
    {
        Console.WriteLine("Dog " + this.Name + " Seat");
    }
    
}