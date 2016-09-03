using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Database.Entities;

namespace Database.Repositories
{
    public class PollRepository : IRepository<Poll>
    {
        private readonly DbSet<Poll> _query;

        public PollRepository(DbSet<Poll> settings)
        {
            _query = settings;
        }

        public Poll GetById(int id)
        {
            return _query.FirstOrDefault(x => x.Id == id);
        }

        public Poll GetActivePoll()
        {
            return _query.FirstOrDefault(x => x.IsActive);
        }

        public IQueryable<Poll> GetAll()
        {
            return _query;
        }

        public void Remove(int id)
        {
            _query.Remove(_query.First(p => p.Id == id));
        }

        public void Add(string question, IEnumerable<string> answers)
        {
            var poll = new Poll
            {
                Question = question,
                IsActive = true,
                PollAnswers = answers.Select(a => new PollAnswers
                {
                    Answer = a
                }).ToList()
            };

            _query.Add(poll);
        }

        public void DisactivatePolls()
        {
            _query.ToList().ForEach(p => p.IsActive = false);
        }
    }
}
