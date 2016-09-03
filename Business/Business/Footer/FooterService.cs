using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;

namespace Business.Business.Footer
{
    public class FooterService
    {
        private readonly UnitOfWorkRepository _unitOfWorkRepository = new UnitOfWorkRepository();

        public string GetFooter()
        {
            return _unitOfWorkRepository.FooterRepository.GetFooterText();
        }
    }
}
