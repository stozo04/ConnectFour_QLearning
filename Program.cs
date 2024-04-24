internal class Program
{
    private static void Main()
    {
        // Create board instance
        Board board = new Board();
        board.GameLoop();

        Console.Read();
    }
}