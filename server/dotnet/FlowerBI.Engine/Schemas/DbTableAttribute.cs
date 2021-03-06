﻿using System;

namespace FlowerBI
{
    public class DbTableAttribute : Attribute
    {
        public DbTableAttribute(string dbTableOrViewName)
        {
            Name = dbTableOrViewName;
        }

        public string Name { get; }
    }
}
