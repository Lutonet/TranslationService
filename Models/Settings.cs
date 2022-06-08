using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace TranslationService.Models
{
    public class Settings
    {
        public List<string> TranslationFolders { get; set; } = new List<string>();
        public string DefaultLanguage { get; set; } = string.Empty;
        public List<string> IgnoreLanguages { get; set; } = new List<string>();
        public List<string> TranslationServers { get; set; } = new List<string>();
    }
}