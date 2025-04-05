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

            // Ensure directory exists
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);  // Create directory if it doesn't exist
            }
        }

        // Add a new poll to the JSON file
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

        // Retrieve all polls from the JSON file
        public IReadOnlyList<Poll> GetPolls(Func<Poll, bool> filter = null)
        {
            var polls = GetAllPolls();

            if (filter != null)
            {
                polls = polls.Where(filter);
            }

            return polls.OrderByDescending(p => p.DateCreated).ToList();
        }

        // Update the vote count for a poll option
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

                // Update the vote count based on the option selected
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

                // Save the updated polls list back to the file
                SaveAllPolls(polls);
            }
        }

        // Get all polls from the JSON file
        private IEnumerable<Poll> GetAllPolls()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Poll>();  // Return empty list if file does not exist
            }

            var json = File.ReadAllText(_filePath);

            try
            {
                return JsonConvert.DeserializeObject<List<Poll>>(json) ?? new List<Poll>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error reading the file: {ex.Message}");
                return new List<Poll>();  // Return empty list in case of deserialization error
            }
        }

        // Save all polls to the JSON file
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
