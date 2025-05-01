using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace BuildForReal.Utilities
{
    public class ABCConverter
    {
        public static string ConvertToABC(List<NoteEvent> noteEvents)
        {
            var abcBuilder = new System.Text.StringBuilder();
            abcBuilder.AppendLine("X:1");
            abcBuilder.AppendLine("T:User Composition");
            abcBuilder.AppendLine("M:4/4");
            abcBuilder.AppendLine("L:1/4");
            abcBuilder.AppendLine("V:V1");
            abcBuilder.AppendLine("V:V2 clef=bass");
            abcBuilder.AppendLine("K:C");

            var trebleNotes = new List<string>();
            var bassNotes = new List<string>();

            foreach (var note in noteEvents)
            {
                string abcNote = MapNoteToABC(note.NoteName);
                string duration = MapDurationToABC(note.Duration);

                if (note.NoteName.Contains(",") || note.NoteName.Contains(",,")) // Bass clef
                    bassNotes.Add($"{abcNote}{duration}");
                else
                    trebleNotes.Add($"{abcNote}{duration}");
            }

            // Pad the shorter voice with rests
            int maxCount = Math.Max(trebleNotes.Count, bassNotes.Count);
            while (trebleNotes.Count < maxCount) trebleNotes.Add("z");
            while (bassNotes.Count < maxCount) bassNotes.Add("z");

            // Split notes into measures of 4
            List<string> SplitIntoMeasures(List<string> notes)
            {
                var measures = new List<string>();
                for (int i = 0; i < notes.Count; i += 4)
                {
                    var measureNotes = notes.Skip(i).Take(4).ToList();
                    // If a measure has less than 4 notes, pad with rests
                    while (measureNotes.Count < 4)
                    {
                        measureNotes.Add("z"); // Add rests for missing notes
                    }
                    measures.Add(string.Join(" ", measureNotes));
                }
                return measures;
            }

            var trebleMeasures = SplitIntoMeasures(trebleNotes);
            var bassMeasures = SplitIntoMeasures(bassNotes);
            int totalMeasures = Math.Max(trebleMeasures.Count, bassMeasures.Count);

            // Add notes to the output, two measures per line
            for (int i = 0; i < totalMeasures; i += 2)  // Output two measures per line
            {
                var trebleGroup = trebleMeasures.Skip(i).Take(2).ToList();
                var bassGroup = bassMeasures.Skip(i).Take(2).ToList();

                // Build the line for the treble and bass staves
                string trebleLine = $"[V:V1] {string.Join(" | ", trebleGroup)} |";
                string bassLine = $"[V:V2] {string.Join(" | ", bassGroup)} |";

                // Append to the builder and add a line break for the next measures
                abcBuilder.AppendLine(trebleLine);
                abcBuilder.AppendLine(bassLine);
            }

            return abcBuilder.ToString();
        }


        private static string MapNoteToABC(string noteName)
        {
            if (noteName.Length < 2)
                return "z";

            string note = noteName.Substring(0, noteName.Length - 1);
            int octave;

            if (!int.TryParse(noteName.Substring(noteName.Length - 1), out octave))
                return "z"; // fallback to rest

            // Clamp octave to ABC-compatible range (1 to 6)
            octave = Math.Max(1, Math.Min(6, octave));

            string accidental = "";
            string baseNote = note[0].ToString().ToUpper();

            if (note.Contains("#")) accidental = "^";
            else if (note.Contains("b")) accidental = "_";

            string abcNote = accidental + baseNote;

            if (octave < 4)
            {
                return abcNote + new string(',', 4 - octave);
            }
            else if (octave == 4)
            {
                return abcNote;
            }
            else
            {
                return abcNote.ToLower() + new string('\'', octave - 4);
            }
        }

        private static string MapDurationToABC(float duration)
        {
            if (duration >= 2.0f)
                return "2";
            else if (duration >= 1.0f)
                return "1";
            else if (duration >= 0.5f)
                return "/2";
            else if (duration >= 0.25f)
                return "/4";
            else
                return "/4";
        }
    }
}

