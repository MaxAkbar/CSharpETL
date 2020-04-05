using System;
using System.IO;

namespace CSharpETL
{
    class Program
    {
        static void Main(string[] args)
        {
            // Extract
            string fileName = "stringMovies.csv";
            string[] csvLines = ExtractCsvFile(fileName);

            // Transform
            MovieNActors[] movieNActors = Transform(csvLines);

            // Load
            LoadMovieNActors(movieNActors);
        }

        private static void LoadMovieNActors(MovieNActors[] movieNActors)
        {
            // save the new array struct in two files movies and actors
            for (int index = 0; index < movieNActors.Length; index++)
            {
                // write the first file
                File.AppendAllText("Movies.csv", string.Concat(movieNActors[index].Id, ",", movieNActors[index].MovieNames, Environment.NewLine));

                // write the second file
                if (movieNActors[index].Actors != null)
                {
                    for (int jIndex = 0; jIndex < movieNActors[index].Actors.Length; jIndex++)
                    {
                        string actor = movieNActors[index].Actors[jIndex];

                        File.AppendAllText("Actors.csv", string.Concat(movieNActors[index].Id, ",", actor, Environment.NewLine));
                    }
                }
            }
        }

        private static MovieNActors[] Transform(string[] csvLines)
        {
            MovieNActors[] movieNActors = new MovieNActors[csvLines.Length];

            // iterate through string array
            for (int index = 0; index < csvLines.Length; index++)
            {
                // skip the first line as it's a header
                if(index > 0)
                {
                    string line = csvLines[index];

                    // map the string array to a struct
                    movieNActors[index] = MapCsvLines(line);
                }

            }

            return movieNActors;
        }

        private static MovieNActors MapCsvLines(string line)
        {
            MovieNActors movieNActors = new MovieNActors();

            string[] columns = line.Split(",\"");
            string[] columns2;

            if (columns.Length.Equals(3))
            {
                columns2 = columns[2].Split(",");

                movieNActors.Id = int.Parse(columns[0]);
                movieNActors.MovieNames = columns2[1];
                movieNActors.Actors = ExtractMovieActors(columns[2]);
            }
            else
            {
                columns2 = columns[0].Split(",");

                movieNActors.Id = int.Parse(columns2[0]);
                movieNActors.MovieNames = columns2[1];
                movieNActors.Actors = ExtractMovieActors(columns[1]);
            }

            return movieNActors;
        }

        private static string[] ExtractMovieActors(string actorsColumn)
        {
            string[] jsonObject = actorsColumn.Split("},");
            string[] actors = new string[jsonObject.Length];

            for (int index = 0; index < jsonObject.Length; index++)
            {
                string actor = jsonObject[index];
                actors[index] = ExtractActor(actor);
            }

            return actors;
        }

        private static string ExtractActor(string actor)
        {
            int beginingOfName = actor.IndexOf("'name': '");
            string namePart = actor.Substring(beginingOfName + "'name': '".Length);
            int endOfName = namePart.IndexOf("',");

            if (beginingOfName > 0)
            {
                string actorName = namePart.Substring(0, endOfName);

                return actorName;
            }

            return null;
        }

        private static string[] ExtractCsvFile(string fileName)
        {
            // load the csv file and return array of strings.
            string[] lines = File.ReadAllLines(fileName);

            return lines;
        }
    }

    struct MovieNActors
    {
        public int Id;
        public string MovieNames;
        public string[] Actors;
    }
}
