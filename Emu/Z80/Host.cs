namespace Z80
{
    public interface Host
    {
        void updateScreen(object bitmap);
        object getInput();
    }
}