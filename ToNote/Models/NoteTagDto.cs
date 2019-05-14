using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNote.Models
{
    public class NoteTagDto
    {
        public Note Note { get; set; }

        public string Tag { get; set; }
    }
}
