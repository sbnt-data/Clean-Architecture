using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipJobPortal.Domain.Entities;

namespace ShipJobPortal.Domain.Interfaces;

public interface IShiphireRepository
{
    Task<ReturnResult> SentcandidatetoShiphire(ShiphireApplicantModel model);

}
