using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationService.Models
{
    public class ServerTestResult
    {
        public string Server { get; set; } = string.Empty;
        public bool Successfull { get; set; } = false;
        public string Error { get; set; } = string.Empty;
        public int ResponseTime { get; set; } = 0;
    }
}