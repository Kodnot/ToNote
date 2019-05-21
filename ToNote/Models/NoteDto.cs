using System.Collections.Generic;

namespace ToNote.Models
{
    public class NoteDto
    {
        public Note Note { get; set; }
        public List<NoteTextBoxDto> Files { get; set; }
        public List<NoteTodoDto> Todos { get; set; }
    }
}