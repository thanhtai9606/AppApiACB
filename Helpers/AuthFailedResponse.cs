using System.Collections.Generic;

namespace App.Helpers
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}