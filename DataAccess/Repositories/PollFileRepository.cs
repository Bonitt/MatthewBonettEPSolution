using Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataAccess.Repositories
{
    public class PollFileRepository : IPollRepository
    {
        private readonly string _filePath;
        private readonly object _fileLock = new object();

        public PollFileRepository(string filePath = "polls.json")
        {
            _filePath = filePath;
            Console.WriteLine($"Using storage file: {Path.GetFullPath(_filePath)}");

            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);  
            }
        }
        public void CreatePoll(Poll newPoll)
        {
            lock (_fileLock)
            {
                var polls = GetAllPolls().ToList();

                newPoll.Id = polls.Count > 0 ? polls.Max(p => p.Id) + 1 : 1;
                newPoll.DateCreated = DateTime.UtcNow;

                polls.Add(newPoll);

                SaveAllPolls(polls);
            }
        }

        public IReadOnlyList<Poll> GetPolls(Func<Poll, bool> filter = null)
        {
            var polls = GetAllPolls();

            if (filter != null)
            {
                polls = polls.Where(filter);
            }

            return polls.OrderByDescending(p => p.DateCreated).ToList();
        }

        public void Vote(int pollId, int optionNumber)
        {
            lock (_fileLock)
            {
                var polls = GetAllPolls().ToList();
                var poll = polls.FirstOrDefault(p => p.Id == pollId);

                if (poll == null)
                {
                    Console.WriteLine($"Poll with Id {pollId} not found.");
                    return;
                }

                switch (optionNumber)
                {
                    case 1:
                        poll.Option1VotesCount++;
                        break;
                    case 2:
                        poll.Option2VotesCount++;
                        break;
                    case 3:
                        poll.Option3VotesCount++;
                        break;
                    default:
                        throw new ArgumentException("Invalid option number");
                }

                SaveAllPolls(polls);
            }
        }

        private IEnumerable<Poll> GetAllPolls()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Poll>();  
            }

            var json = File.ReadAllText(_filePath);

            try
            {
                return JsonConvert.DeserializeObject<List<Poll>>(json) ?? new List<Poll>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error reading the file: {ex.Message}");
                return new List<Poll>(); 
            }
        }

        private void SaveAllPolls(List<Poll> polls)
        {
            var json = JsonConvert.SerializeObject(polls, Formatting.Indented);

            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, json);
            File.Move(tempFile, _filePath, overwrite: true);

            Console.WriteLine($"Polls saved to: {_filePath}");
        }
    }
}
