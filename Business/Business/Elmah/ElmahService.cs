using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Database.Entities;

namespace Core.Business.Elmah
{
    /// <summary>
    /// Manager for elmah errors
    /// </summary>
    public class ElmahService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository;

        public ElmahService()
        {
            _unitOfWorkRepository = new UnitOfWorkRepository();    
        }

        /// <summary>
        /// Remove all the errors from the database
        /// </summary>
        public void CleanErrors()
        {
           _unitOfWorkRepository.Truncate("ELMAH_ERROR");
        }

        /// <summary>
        /// Gets all the elmah errors
        /// </summary>
        public IList<ELMAH_Error> GetAll()
        {
            return _unitOfWorkRepository.ElmahRepository.GetAll().ToList();
        }

        public ELMAH_Error GetById(Guid id)
        {
            return _unitOfWorkRepository.ElmahRepository.GetById(id);
        }
    }
}
