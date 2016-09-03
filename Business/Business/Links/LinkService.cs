using System.Collections.Generic;
using System.Linq;
using Database;

namespace Core.Business.Links
{
    public class LinkService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;

        public LinkService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();
        }

        public IEnumerable<Database.Entities.Links> GetLinks(int amount)
        {
            return _unitOfWorkRepository.LinkRepository.GetAll(amount).OrderBy(x => x.Name);
        }

        public IEnumerable<Database.Entities.Links> GetLinks()
        {
            return _unitOfWorkRepository.LinkRepository.GetAll().OrderByDescending(x => x.Id);
        }

        public Database.Entities.Links GetById(int id)
        {
            return _unitOfWorkRepository.LinkRepository.GetById(id);
        }

        public void Update(int id, string name, string link)
        {
            var record = GetById(id);

            if (record != null)
            {
                record.Name = name;
                record.Link = link;

                _unitOfWorkRepository.SaveChanges();
            }
        }

        public bool Remove(int id)
        {
            var record = GetById(id);

            if (record != null)
            {
                _unitOfWorkRepository.LinkRepository.Remove(id);
                _unitOfWorkRepository.SaveChanges();
            }

            return record != null;
        }

        public void Add(string name, string link)
        {
            _unitOfWorkRepository.LinkRepository.Add(name, link);
            _unitOfWorkRepository.SaveChanges();
        }
    }
}
