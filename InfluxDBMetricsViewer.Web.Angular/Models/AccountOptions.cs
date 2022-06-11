using System.DirectoryServices.AccountManagement;
using Microsoft.AspNetCore.Authentication;

namespace MetaMetricsViewer.Web.Angular.Models
{
    public class AccountOptions
    {
        public const string Section = "Account";
        public ContextType ContextType { get; set; }
        public string Domain { get; set; }
    }
}