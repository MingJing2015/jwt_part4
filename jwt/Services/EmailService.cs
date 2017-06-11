using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace jwt.Services {
    public class EmailService : IIdentityMessageService {


        public async Task SendAsync(IdentityMessage message) {
            await configSendGridasync(message);
        }
        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task configSendGridasync(IdentityMessage message) {
        }



















    }
}