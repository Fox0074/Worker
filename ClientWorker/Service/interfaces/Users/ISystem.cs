namespace Interfaces.Users
{
    public interface ISystem
    {
        void CutTheText(ref string Text);
        string[] GetListUsers();
    }
}
