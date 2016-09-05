namespace Z80
{
    public interface Host
    {
        void Startup();
        void UpdateScreen(object bitmap);
        object GetInput();
    }
}