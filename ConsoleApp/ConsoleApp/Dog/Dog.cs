namespace ConsoleApp;

public class Dog
{
    protected string Name;
    
    public Dog(string name)
    {
        Name = name;
        Console.WriteLine($"Dog {Name} created");
    }
}