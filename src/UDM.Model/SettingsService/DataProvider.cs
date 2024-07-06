namespace UDM.Model.SettingsService
{
    internal static class DataProvider
    {
        public static Dictionary<string, string> LoadDataDict(string fileName, char delimeter = '=')
        {
            Dictionary<string, string> result = new();
            string[] text;
            try
            {
                text = File.ReadAllLines(fileName);
            }
            catch (FileNotFoundException)
            {
                return new();
            }
            catch (DirectoryNotFoundException)
            {
                return new();
            }
            if (text.Length == 0) return result;

            foreach (string line in text)
            {
                result.Add(line.Split(delimeter)[0], line.Split(delimeter)[1]);
            }

            return result;
        }

        public static List<List<string>> LoadDataList(string fileName, char delimeter = '=')
        {
            List<List<string>> result = new();

            string[] text;
            try
            {
                text = File.ReadAllLines(fileName);
            }
            catch (FileNotFoundException)
            {
                return new();
            }
            catch (DirectoryNotFoundException)
            {
                return new();
            }
            if (text.Length == 0) return result;

            result.AddRange(text.Select(line => line.Split(delimeter)).Select(inputArray => new List<string>(inputArray)));

            return result;
        }

        public static void WriteDataDict(string fileName, Dictionary<string, string> data, char delimiter = '=')
        {
            var spittedPath = fileName.Split("\\");
            var path = string.Join("\\", spittedPath.Skip(0).Take(spittedPath.Length - 1));
            Directory.CreateDirectory(path);
            using StreamWriter writer = new(fileName);
            foreach (var key in data.Keys)
            {
                if (key.Contains(delimiter) || data[key].Contains(delimiter))
                {
                    throw new InvalidDataProvidedException();
                }
                writer.WriteLine(key + delimiter + data[key]);
            }
        }

        public static void WriteDataList(string fileName, List<List<string>> data, char delimeter = '=')
        {
            using StreamWriter writer = new(fileName);
            foreach (List<string> dataLine in data)
            {
                string output = "";
                foreach (string line in dataLine)
                {
                    if (line.Contains(delimeter))
                    {
                        throw new InvalidDataProvidedException();
                    }
                    output += line + delimeter;
                }
                output = output.Remove(output.Length - 1, 1);
                writer.WriteLine(output);
            }
        }
    }

    public class InvalidDataProvidedException : Exception
    {
        public InvalidDataProvidedException()
        {
        }

        public InvalidDataProvidedException(string message)
            : base(message)
        {
        }

        public InvalidDataProvidedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}