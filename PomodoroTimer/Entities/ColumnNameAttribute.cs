using System;
using System.Collections.Generic;
using System.Text;

namespace PomodoroTimer.Entities
{
    class ColumnNameAttribute : System.Attribute
    {
        public ColumnNameAttribute(string Name)
        { this.Name = Name; }

        public string Name { get; set; }
    }
}
