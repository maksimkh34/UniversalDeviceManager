namespace UDM.Model.SettingsService
{
    internal static class DataProvider
    {
        public static Dictionary<string, string> LoadDataDict(string fileName, char delimiter = '=')
        {
            Dictionary<string, string> result = new();
            string[] text;
            try
            {
                text = File.ReadAllLines(fileName);
            }
            catch (FileNotFoundException)
            {
                return new Dictionary<string, string>();
            }
            catch (DirectoryNotFoundException)
            {
                return new Dictionary<string, string>();
            }
            if (text.Length == 0) return result;

            foreach (var line in text)
            {
                result.Add(line.Split(delimiter)[0], line.Split(delimiter)[1]);
            }

            return result;
        }

        public static List<List<string>> LoadDataList(string fileName, char delimiter = '=')
        {
            List<List<string>> result = new();

            string[] text;
            try
            {
                text = File.ReadAllLines(fileName);
            }
            catch (FileNotFoundException)
            {
                return new List<List<string>>();
            }
            catch (DirectoryNotFoundException)
            {
                return new List<List<string>>();
            }
            if (text.Length == 0) return result;

            result.AddRange(text.Select(line => line.Split(delimiter)).Select(inputArray => new List<string>(inputArray)));

            return result;
        }

        public static void WriteDataDict(string fileName, Dictionary<string, string> data, char delimiter = '=')
        {
            var spittedPath = fileName.Split("\\");
            var path = string.Join("\\", spittedPath.Skip(0).Take(spittedPath.Length - 1));
            if(path != string.Empty) Directory.CreateDirectory(path);
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

        public static void WriteDataList(string fileName, List<List<string>> data, char delimiter = '=')
        {
            using StreamWriter writer = new(fileName);
            foreach (var dataLine in data)
            {
                var output = "";
                foreach (var line in dataLine)
                {
                    if (line.Contains(delimiter))
                    {
                        throw new InvalidDataProvidedException();
                    }
                    output += line + delimiter;
                }
                output = output.Remove(output.Length - 1, 1);
                writer.WriteLine(output);
            }
        }
    }

}