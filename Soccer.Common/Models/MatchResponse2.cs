﻿using System;
using System.Collections.Generic;

namespace Soccer.Common.Models
{
    public class MatchResponse2
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateLocal => Date.ToLocalTime();
        public TeamResponse Local { get; set; }
        public TeamResponse Visitor { get; set; }
        public int? GoalsLocal { get; set; }
        public int? GoalsVisitor { get; set; }
        public bool IsClosed { get; set; }
        public GroupResponse2 Group { get; set; }
        public String DateName { get; set; }
    }
}