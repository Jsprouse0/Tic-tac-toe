
namespace Tic_tac_toe
{
    public class Gameresult
    {
        public Player Winner { get; set; }
        public Wininfo? Wininfo { get; set; }
        public Win Win { get; internal set; }
    }
}
