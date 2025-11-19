namespace ConsoleApp;

public class NeoDog
{
    private readonly IEnumerable<ICommand> _commands;

    // ОБРАТИ ВНИМАНИЕ: Мы просим IEnumerable<ICommand>.
    // Контейнер настолько умен, что найдет ВСЕ зарегистрированные команды
    // и передаст их списком. Это очень красивый паттерн плагинов.
    public NeoDog(IEnumerable<ICommand> commands)
    {
        _commands = commands;
    }

    public void ShowOff()
    {
        Console.WriteLine("Собака начинает представление:");
        foreach (var command in _commands)
        {
            command.DoCommand();
        }
    }
}