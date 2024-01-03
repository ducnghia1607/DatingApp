using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var usersData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var users = JsonSerializer.Deserialize<List<AppUser>>(usersData); // This deserializes the json data to an object of whatever type we specify here. As the Json data properties match the format of our AppUser properties, it converts it correctly to a list of Appusers

        if (users == null) return;

        var roles = new List<AppRole> // Here we create a list which is expecting a list of AppRoles. We intialise the list and add 3 new AppRoles and assign values to there name properties
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"}
            };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        foreach (var user in users)
        {

            user.UserName = user.UserName.ToLower();
            user.Photos.First().isMain = true;
            await userManager.CreateAsync(user, "Pa$$w0rd"); // Using this method on the userManager takes the user we are creating as an AppUser and we can also add a password as the 2nd parameter. If we wanted to allow the creation of simpler, less secure passwords, we could add some further configuration in our startup class

            await userManager.AddToRoleAsync(user, "Member"); // This then adds the user within this for loop and assigns them to the member role we created within Idetities RoleManager       
        }

        var admin = new AppUser // Here we are creating a new AppUser so that we can assign them to be an admin 
        {
            UserName = "admin",
            KnownAs = "Admin",
            City = " ",
            Country = " ",
            Gender = " ",
            DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow)
        };



        await userManager.CreateAsync(admin, "Pa$$w0rd");

        await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" }); // Here we are assigning multiple roles to the admin user we do that by creating a new array and initialising the array with two strings. This works as the 'AddToRolesAsync' method is expecting an IEnumerable of strings as it's 2nd argument

    }
}

// Now, this is going to create and save this change in the database.
// So we didn't need a separate method to save changes inside here.