namespace ConsoleApp.Fight;

public class Fight
{
    private IWeapon _left;
    private IWeapon _right;

    public void DoLeft()
    {
        if (_left != null) _left.Fire();
        else Console.WriteLine("No Weapon");
    }

    public void DoRight()
    {
        if (_right != null) _right.Fire();
        else Console.WriteLine(" No Weapon");
    }

    public void ChangeLeft(IWeapon weapon)
    {
        _left = weapon;
    }

    public void ChangeRight(IWeapon weapon)
    {
        _right = weapon;
    }
}