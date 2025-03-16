namespace Pipeline.App.Service
{
    using Pipeline.App.Entity;
    using System;
    using System.Threading.Tasks;

    public class LogParser : ILogParser
    {
        public async Task ParseLogAsync(string path)
        {
            var lines = await File.ReadAllLinesAsync(path);
            var dictinonary = new Dictionary<string, List<LogMessage>>();

            for (int i = 0; i < lines.Length; i++)
            {
                var currentLine = lines[i];
                var words = ParseLine(currentLine);

                if (words.Count == 5 && int.TryParse(words.Last(), out int _))
                {
                    CreatePipeline(dictinonary, CreateLogMessage(words));
                }
                else
                {
                    if (i + 1 < lines.Length)
                    {
                        string nextLine = lines[i + 1];

                        if (nextLine == "1")
                        {
                            string combinedLine = currentLine + nextLine;
                            words = ParseLine(combinedLine);
                            i++;
                        }
                        else
                        {
                            while (words.Count < 5)
                            {
                                currentLine += lines[++i];
                                words = ParseLine(currentLine);
                            }
                        }
                        CreatePipeline(dictinonary, CreateLogMessage(words));
                    }
                }
            }

            var orderedbyPipeline = dictinonary.OrderByDescending(x => x.Key).ToList();

            foreach (var pipeline in orderedbyPipeline)
            {
                Console.WriteLine("Pipeline " + pipeline.Key);

                foreach (var item in pipeline.Value.OrderByDescending(x => x.Id))
                {
                    Console.WriteLine("     " + item.Id + "|" + (item.Encoding == 0 ? item.Body : HexToString(item.Body)));
                }
            }
        }

        private static List<string> ParseLine(string line)
        {
            var currentPiece = string.Empty;
            var inBrackets = false;
            var pieces = new List<string>();

            for (var i = 0; i < line.Length; i++)
            {
                var character = line[i];

                if (character == '[')
                {
                    if (!string.IsNullOrWhiteSpace(currentPiece))
                    {
                        pieces.Add(currentPiece);
                        currentPiece = string.Empty;
                    }

                    inBrackets = true;
                    currentPiece += character;
                }
                else if (character == ']')
                {
                    currentPiece += character;
                    pieces.Add(currentPiece);
                    currentPiece = string.Empty;
                    inBrackets = false;
                }
                else if (character != ' ')
                {
                    currentPiece += character;
                }
                else if (!inBrackets)
                {
                    if (!string.IsNullOrWhiteSpace(currentPiece))
                    {
                        pieces.Add(currentPiece);
                        currentPiece = string.Empty;
                    }
                }
                else if (inBrackets && character == ' ')
                {
                    if (!string.IsNullOrWhiteSpace(currentPiece))
                    {
                        currentPiece += character;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(currentPiece))
            {
                pieces.Add(currentPiece);
            }

            return pieces;
        }

        private static Dictionary<string, List<LogMessage>> CreatePipeline(Dictionary<string, List<LogMessage>> dictionary, LogMessage logMessage)
        {
            if (!dictionary.TryGetValue(logMessage.PipelineId, out List<LogMessage>? value))
            {
                dictionary.Add(logMessage.PipelineId, new List<LogMessage> { logMessage });
            }
            else
            {
                value.Add(logMessage);
            }

            return dictionary;
        }

        private static LogMessage CreateLogMessage(List<string> words)
        {
            return new LogMessage
            {
                PipelineId = words[0],
                Id = words[1],
                Encoding = int.Parse(words[2]),
                Body = words[3],
                NextId = words[4]
            };
        }

        private static string HexToString(string hex)
        {
            hex = hex.Trim('[').Trim(']');

            if (string.IsNullOrEmpty(hex) || hex.Length % 2 != 0)
            {
                return hex + " is not a valid hexadecimal string.";
            }

            char[] chars = new char[hex.Length / 2];

            for (int i = 0; i < hex.Length; i += 2)
            {
                string hexPair = hex.Substring(i, 2);
                chars[i / 2] = (char)Convert.ToByte(hexPair, 16);
            }

            return new string(chars);
        }
    }
}
