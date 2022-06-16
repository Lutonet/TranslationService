using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationService.Models
{
    public class Translation
    {
        public Dictionary<string, string> Phrase { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
    }
}