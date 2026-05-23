using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinAgend.Core.Interfaces;

public interface IEmailTemplateService
{
    Task<string> GetTemplateAsync(string templateName);
}