namespace ConsoleApp.Commands;

public class SeatCommand : ICommand
{
    public void DoCommand()
    {
        Console.WriteLine("Seat");
    }
}