using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace jwt.Infrastructure {
public class ApplicationUser : IdentityUser {
    [Required]
    [MaxLength(100)]
    public string   FirstName { get; set; }
    [Required]
    [MaxLength(100)]
    public string   LastName { get; set; }
    [Required]
    public byte     Level { get; set; }
    [Required]
    public DateTime JoinDate { get; set; }

        //Rest of code is removed for brevity

        //add the helper method which will be responsible to get the authenticated user identity(all roles and claims mapped to the user). 
        //The “UserManager” class contains a method named “CreateIdentityAsync” to do this task, it will basically query the DB 
        // and get all the roles and claims for this user, to implement this open class “ApplicationUser”

public async Task<ClaimsIdentity> GenerateUserIdentityAsync(

    UserManager<ApplicationUser> manager, string authenticationType) {

	var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

	// Add custom user claims here
	return userIdentity;
}
}
}