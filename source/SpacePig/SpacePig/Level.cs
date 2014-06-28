using System.IO;
using Microsoft.Xna.Framework;

namespace SpacePig
{
    enum FieldType
    {
        Nothing,
        Bridge,
        Startpoint,
        Guest,
        Goal
    }

    class Level
    {
        public const int ROWCOUNT = 24;
        public const int COLUMNCOUNT = 32;

        public Level(string filename)
        {
            string[] lines = File.ReadAllLines(filename);

            if (lines.Length != ROWCOUNT)
                throw new InvalidDataException("Falsche Anzahl Zeilen");

            this.Fields = new FieldType[COLUMNCOUNT, ROWCOUNT];
            bool startPointFound = false;
            bool guestPointFound = false;
            bool goalPointFound = false;

            int row = 0;
            foreach (string line in lines)
            {
                for (int column = 0; column < COLUMNCOUNT; column++)
                {
                    if (line.Length != COLUMNCOUNT)
                        throw new InvalidDataException("Falsche Anzahl Spalten in Reihe " + row);

                    switch (line[column])
                    {
                        case '.': this.Fields[column, row] = FieldType.Nothing; break;
                        case '#': this.Fields[column, row] = FieldType.Bridge; break;

                        case '1':
                            if (startPointFound)
                                throw new InvalidDataException("Mehr als ein Startpunkt vorhanden");

                            this.Fields[column, row] = FieldType.Startpoint;
                            this.StartPoint = new Vector2(column, row);
                            startPointFound = true;
                            break;

                        case '2':
                            if (guestPointFound)
                                throw new InvalidDataException("Mehr als ein Gastpunkt vorhanden");

                            this.Fields[column, row] = FieldType.Guest;
                            this.GuestPoint = new Vector2(column, row);
                            guestPointFound = true;
                            break;

                        case '3':
                            if (goalPointFound)
                                throw new InvalidDataException("Mehr als ein Zielpunkt vorhanden");

                            this.Fields[column, row] = FieldType.Goal;
                            this.GoalPoint = new Vector2(column, row);
                            goalPointFound = true;
                            break;
                    }
                }
                row++;
            }

            if (!startPointFound)
                throw new InvalidDataException("Kein Startpunkt definiert");
            if (!guestPointFound)
                throw new InvalidDataException("Kein Gastpunkt definiert");
            if (!goalPointFound)
                throw new InvalidDataException("Kein Zielpunkt definiert");
        }

        public FieldType[,] Fields { get; private set; }
        public Vector2 StartPoint { get; private set; }
        public Vector2 GuestPoint { get; private set; }
        public Vector2 GoalPoint { get; private set; } 
    }
}
