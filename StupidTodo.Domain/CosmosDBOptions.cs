﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class CosmosDBOptions
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
    }
}
