namespace ApathyAndMania.DayStats;

/// <summary>
/// Записка/изменение, влияющие на шкалу апатии/мании
/// </summary>
public class NoteChange
{
    /// <summary>
    /// Изменения по шкале. Плюс - изменения в сторону мании, минус - в сторону апатии
    /// </summary>
    public float Change { get; set; }

    public string TextNote { get; set; }

    public string SaveTime { get; set; }

    // Для десериализации
    public NoteChange()
    {
        Change = 0;
        TextNote = "";
        SaveTime = "";
    }

    public NoteChange(float change, string textNote, string saveTime)
    {
        Change = change;
        TextNote = textNote;
        SaveTime = saveTime;
    }
}