namespace ApathyAndMania.DayStats;

public class Day
{
    public string Date { get; set; }
    public List<NoteChange> NoteChanges { get; set; }

    public Day()
    {
        Date = DateTime.Today.ToShortDateString();
        NoteChanges = new List<NoteChange>();
    }
    
    public async Task AddNewNote(float change, string textNote = "")
    {
        NoteChanges.Add(new NoteChange(change, textNote, DateTime.Now.ToShortTimeString()));
        await JsonManager.SaveDay(this);
    }
}