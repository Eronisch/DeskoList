using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models.Poll;
using Database;
using Database.Entities;

namespace Core.Business.Poll
{
    public class PollService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;

        public PollService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
        }

        public IEnumerable<PollResultModel> GetPolls()
        {
            return
                _unitOfWorkRepository.PollRepository.GetAll().OrderByDescending(p => p.Id).ToList()
                    .Select(p => new PollResultModel(p.Id, p.Question, p.PollAnswers.ToList()));
        }

        public bool Remove(int id)
        {
            var poll = _unitOfWorkRepository.PollRepository.GetById(id);

            if (poll != null)
            {
                _unitOfWorkRepository.PollRepository.Remove(id);
                _unitOfWorkRepository.SaveChanges();
            }

            return poll != null;
        }

        public void Update(int id, string question, Dictionary<int, string> answers)
        {
            var poll = _unitOfWorkRepository.PollRepository.GetById(id);

            if (poll != null)
            {
                poll.Question = question;

                foreach (var pAnswer in poll.PollAnswers)
                {
                    pAnswer.Answer = answers[pAnswer.Id];
                }
                
                _unitOfWorkRepository.SaveChanges();
            }
        }

        public void Add(string question, IEnumerable<string> answers)
        {
            _unitOfWorkRepository.PollRepository.DisactivatePolls();
            _unitOfWorkRepository.PollRepository.Add(question, answers);
            _unitOfWorkRepository.SaveChanges();
        }

        public PollResultModel GetById(int id)
        {
            var poll =
                _unitOfWorkRepository.PollRepository.GetById(id);

            return new PollResultModel(poll.Id, poll.Question, poll.PollAnswers.ToList());
        }
    }
}
