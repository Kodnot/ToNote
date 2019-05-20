using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNote.Models
{
    public class NoteDto
    {
        public Note Note { get; set; }
        public List<NoteTextBoxDto> Files { get; set; }
        public List<NoteTodoDto> Todos { get; set; }
    }
}