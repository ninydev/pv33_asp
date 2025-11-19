namespace ConsoleApp;

public interface IEdu
{
    void AddCommand(string commandName, ICommand command);
    void ExecuteCommand(string commandName);
    void RemoveCommand(string commandName);
}