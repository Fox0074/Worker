namespace Interfaces.Users
{
    public interface IAdmin
    {
        bool TryFindObject(out object obj);
        int Bark(int nTimes);
    }
}
