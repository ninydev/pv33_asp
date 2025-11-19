namespace ConsoleApp;

public class Dog : IEdu
{
    protected string Name;
    
    public Dog(string name)
    {
        Name = name;
        Console.WriteLine($"Dog {Name} created");
    }
    
    private Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

    public void AddCommand(string commandName, ICommand command)
    {
        _commands.Add(commandName, command);
    }

    public void ExecuteCommand(string commandName)
    {
        _commands[commandName].DoCommand();
    }

    public void RemoveCommand(string commandName)
    {
        _commands.Remove(commandName);
    }
}