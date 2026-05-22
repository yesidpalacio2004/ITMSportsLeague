using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.Domain.Entities
{
    public class MatchLineup:AuditBase
    {
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public bool IsStarter {  get; set; } = true;
        public string Position { get; set; }= string.Empty;
        public Match Match { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
