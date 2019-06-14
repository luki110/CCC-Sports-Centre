using CCCSportsCentreGradedUnit.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Data
{
    public class DummyData
    {
        public static async Task Initialize(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();


            string admin = "Admin";
            string member = "Member";
            string staff = "Staff";
            string bookingClerk = "BookingClerk";
            string membershipClerk = "MembershipClerk";
            string manager = "Manager";
            string managerAssistant = "ManagerAssistant";

            string password = "Pass1#";

            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole(admin));
            }
            if (await roleManager.FindByNameAsync("Member") == null)
            {
                await roleManager.CreateAsync(new IdentityRole(member));
            }
            if (await roleManager.FindByNameAsync("Staff") == null)
            {
                await roleManager.CreateAsync(new IdentityRole(staff));
            }
            if (await roleManager.FindByNameAsync("BookingClerk") == null)
            {
                await roleManager.CreateAsync(new IdentityRole(bookingClerk));
            }
            if (await roleManager.FindByNameAsync("MembershipClerk") == null)
            {
                await roleManager.CreateAsync(new IdentityRole(membershipClerk));
            }
            if (await roleManager.FindByNameAsync("Manager") == null)
            {
                await roleManager.CreateAsync(new IdentityRole(manager));
            }
            if (await roleManager.FindByNameAsync("ManagerAssistant") == null)
            {
                await roleManager.CreateAsync(new IdentityRole(managerAssistant));
            }

            //CREATE MEMBERSHIP TYPES            
            var Adult = new MembershipType
            {
                Name = "Adult",
                Price = 30
            };
            context.MembershipTypes.Add(Adult);
            
            var Junior = new MembershipType
            {
               
                Name = "Junior",
                Price = 15
            };
            context.MembershipTypes.Add(Junior);
            var Family = new MembershipType
            {
                Name = "Family",
                Price = 75
            };
            context.MembershipTypes.Add(Family);


            //CREATE ROOMS
            var Court1 = new Room
            {
                Name = "Court1",
                Capacity = 10
            };
            context.Rooms.Add(Court1);
            var Court2 = new Room
            {
                Name = "Court2",
                Capacity = 10
            };
            context.Rooms.Add(Court2);
            var Court3 = new Room
            {
                Name = "Court3",
                Capacity = 10
            };
            context.Rooms.Add(Court3);
            var Court4 = new Room
            {
                Name = "Court4",
                Capacity = 10
            };
            context.Rooms.Add(Court4);
            var Court5 = new Room
            {
                Name = "Court5",
                Capacity = 10
            };
            context.Rooms.Add(Court5);
            var Court6 = new Room
            {
                Name = "Court6",
                Capacity = 10
            };
            context.Rooms.Add(Court6);

            var HalfHall1 = new Room
            {
                Name = "Half Hall - East",
                Capacity = 30
            };
            context.Rooms.Add(HalfHall1);
            var HalfHall2 = new Room
            {
                Name = "Half Hall - West",
                Capacity = 30
            };
            context.Rooms.Add(HalfHall2);

            var FullHall = new Room
            {
                Name = "Full Hall",
                Capacity = 60
            };
            context.Rooms.Add(FullHall);

            var FunctionRoom = new Room
            {
                Name = "Function room",
                Capacity = 20
            };
            context.Rooms.Add(FunctionRoom);

            var GymStudio = new Room
            {
                Name = "Gym studio",
                Capacity = 20
            };
            context.Rooms.Add(GymStudio);

            //CREATE ClassesType
            var Yoga = new FitnessClassCategory
            {
                Name = "Yoga",
                Description = "Learn how to lick your elbow",
                Image = @"\images\ClassImages\defaultClassImage.jpg"
            };
            context.FitnessClassCategories.Add(Yoga);

            var Pilates = new FitnessClassCategory
            {
                Name = "Pilates",
                Description = "Pilates is a great way to condition the whole body.",
                Image = @"\images\ClassImages\defaultClassImage.jpg"
            };
            context.FitnessClassCategories.Add(Pilates);
            var Zumba = new FitnessClassCategory
            {
                Name = "Zumba",
                Description = "Zumba is all about those exotic Latin rhythms, and soon you'll find you're partying yourself into shape.",
                Image = @"\images\ClassImages\defaultClassImage.jpg"
            };
            context.FitnessClassCategories.Add(Zumba);

            //CREATE ActivityTypes
            var Badminton = new FitnessActivityCategory
            {
                Name = "Badminton",
                Description = "Racquet sport played using racquets to hit a shuttlecock across a net.",
                Image = @"\images\ActivityImages\defaultActivityImage.jpg"
            };
            context.FitnessActivityCategories.Add(Badminton);

            var TableTennis = new FitnessActivityCategory
            {
                Name = "Table Tennis",
                Description = "Also known as ping-pong, is a sport in which two or four players hit a lightweight ball back and forth across a table using small rackets.",
                Image = @"\images\ActivityImages\defaultActivityImage.jpg"
            };
            context.FitnessActivityCategories.Add(TableTennis);

            var Basketball = new FitnessActivityCategory
            {
                Name = "Basketball",
                Description = "Basketball",
                Image = @"\images\ActivityImages\defaultActivityImage.jpg"
            };
            context.FitnessActivityCategories.Add(Basketball);

            var Football = new FitnessActivityCategory
            {
                Name = "Football",
                Description = "Football 5-a-side",
                Image = @"\images\ActivityImages\defaultActivityImage.jpg"
            };
            context.FitnessActivityCategories.Add(Football);
            

            //Create fitness class
            var fitnessClass1 = new FitnessClass
            {
                Available = true,
                NoOfPeopleBooked = 0,
                StartDate = DateTime.Today,
                StartTime = DateTime.Today.AddHours(18),
                EndTime = DateTime.Today.AddHours(19),
                Duration = 60,
                Price = 10,
                FitnessClassCategoryId = Yoga.Id,
                RoomId = GymStudio.Id

            };
            context.FitnessClasses.Add(fitnessClass1);

            var fitnessClass2 = new FitnessClass
            {
                Available = true,
                NoOfPeopleBooked = 0,
                StartDate = DateTime.Today,
                StartTime = DateTime.Today.AddHours(13),
                EndTime = DateTime.Today.AddHours(14),
                Duration = 60,
                Price = 10,
                FitnessClassCategoryId = Zumba.Id,
                RoomId = Court1.Id

            };
            context.FitnessClasses.Add(fitnessClass2);

            var fitnessClass3 = new FitnessClass
            {
                Available = true,
                NoOfPeopleBooked = 0,
                StartDate = DateTime.Today.AddDays(1),
                StartTime = DateTime.Today.AddDays(1).AddHours(11),
                EndTime = DateTime.Today.AddDays(1).AddHours(12),
                Duration = 60,
                Price = 10,
                FitnessClassCategoryId = Pilates.Id,
                RoomId = Court2.Id

            };
            context.FitnessClasses.Add(fitnessClass3);

            var fitnessClass4 = new FitnessClass
            {
                Available = true,
                NoOfPeopleBooked = 0,
                StartDate = DateTime.Today.AddDays(2),
                StartTime = DateTime.Today.AddDays(2).AddHours(10),
                EndTime = DateTime.Today.AddDays(1).AddHours(11),
                Duration = 60,
                Price = 10,
                FitnessClassCategoryId = Pilates.Id,
                RoomId = Court3.Id

            };
            context.FitnessClasses.Add(fitnessClass4);


            //CREATE FITNESS ACTIVITIES
            var fitnessActivity = new FitnessActivity
            {
                Available = true,
                StartDate = DateTime.Today,
                StartTime = DateTime.Today.AddHours(10),
                EndTime = DateTime.Today.AddHours(11),
                Duration = 60,
                Price = 100,
                RoomId = Court5.Id,
                FitnessActivityCategoryId = Badminton.Id
            };
            context.FitnessActivities.Add(fitnessActivity);

            var fitnessActivity2 = new FitnessActivity
            {
                Available = true,
                StartDate = DateTime.Today,
                StartTime = DateTime.Today.AddHours(12),
                EndTime = DateTime.Today.AddHours(13),
                Duration = 60,
                Price = 100,
                RoomId = Court5.Id,
                FitnessActivityCategoryId = Football.Id
            };
            context.FitnessActivities.Add(fitnessActivity2);

            var fitnessActivity3 = new FitnessActivity
            {
                Available = true,
                StartDate = DateTime.Today.AddDays(1),
                StartTime = DateTime.Today.AddDays(1).AddHours(13),
                EndTime = DateTime.Today.AddDays(1).AddHours(14),
                Duration = 60,
                Price = 100,
                RoomId = Court5.Id,
                FitnessActivityCategoryId = TableTennis.Id
            };
            context.FitnessActivities.Add(fitnessActivity3);

            var fitnessActivity4 = new FitnessActivity
            {
                Available = true,
                StartDate = DateTime.Today.AddDays(2),
                StartTime = DateTime.Today.AddDays(2).AddHours(16),
                EndTime = DateTime.Today.AddDays(2).AddHours(17),
                Duration = 60,
                Price = 100,
                RoomId = Court5.Id,
                FitnessActivityCategoryId = Basketball.Id
            };
            context.FitnessActivities.Add(fitnessActivity4);

            //create ADMIN
            if (await userManager.FindByNameAsync("admin@cccsportcentre.com") == null)
            {
                var administrator = new Staff
                {
                    UserName = "admin@cccsportcentre.com",
                    Email = "admin@cccsportcentre.com",
                    EmailConfirmed = true,
                    FirstName = "Lukasz",
                    LastName = "Bonkowski",
                    Street = "Fake st",
                    HouseNumber = "1",
                    City = "Glasgow",
                    Country = "UK",
                    PostCode = "G6 66G",
                    PhoneNumber = "6969696969",
                    JobTitle = "Administrator",
                    EmergencyContact = "Bill Gates",
                    EmergencyContDetails = "0666-666-666",
                    CurrentQualification = "Administrator",
                    RoleType = Role.Admin
                };
                var result = await userManager.CreateAsync(administrator);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(administrator, password);
                    await userManager.AddToRoleAsync(administrator, admin);
                }
            }

            //Create BOOKING CLERK
            if (await userManager.FindByNameAsync("jeff@cccsportcentre.com") == null)
            {
                var staff1 = new Staff
                {
                    UserName = "jeff@cccsportcentre.com",
                    Email = "jeff@cccsportcentre.com",
                    EmailConfirmed = true,
                    FirstName = "Jeff",
                    LastName = "Slow",
                    Street = "Fake st",
                    HouseNumber = "1",
                    City = "Glasgow",
                    Country = "UK",
                    PostCode = "G6 66G",
                    PhoneNumber = "6969696969",
                    JobTitle = "BookingClerk",
                    EmergencyContact = "Bill Gates",
                    EmergencyContDetails = "0666-666-666",
                    CurrentQualification = "BookingClerk",
                    RoleType = Role.BookingClerk,
                };
                var result = await userManager.CreateAsync(staff1);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(staff1, password);
                    await userManager.AddToRoleAsync(staff1, bookingClerk);
                }
            }
            //Create MEMBERSHIP CLERK
            if (await userManager.FindByNameAsync("tom@cccsportcentre.com") == null)
            {
                var staff2 = new Staff
                {
                    UserName = "tom@cccsportcentre.com",
                    Email = "tom@cccsportcentre.com",
                    EmailConfirmed = true,
                    FirstName = "Tom",
                    LastName = "Fast",
                    HouseNumber = "1",
                    Street = "Fake st",
                    City = "Glasgow",
                    Country = "UK",
                    PostCode = "G6 66G",
                    PhoneNumber = "6969696969",
                    JobTitle = "Membership Clerk",
                    EmergencyContact = "Bill Gates",
                    EmergencyContDetails = "0666-666-666",
                    CurrentQualification = "MembershipClerk",
                    RoleType = Role.MembershipClerk
                };
                var result = await userManager.CreateAsync(staff2);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(staff2, password);
                    await userManager.AddToRoleAsync(staff2, membershipClerk);
                }
            }

            //Create MANAGER
            if (await userManager.FindByNameAsync("chris@cccsportcentre.com") == null)
            {
                var staff3 = new Staff
                {
                    UserName = "chris@cccsportcentre.com",
                    Email = "chris@cccsportcentre.com",
                    EmailConfirmed = true,
                    FirstName = "Chris",
                    LastName = "Boss",
                    HouseNumber = "1",
                    Street = "Fake st",
                    City = "Glasgow",
                    Country = "UK",
                    PostCode = "G6 66G",
                    PhoneNumber = "6969696969",
                    JobTitle = "Manager",
                    EmergencyContact = "Bill Gates",
                    EmergencyContDetails = "0666-666-666",
                    CurrentQualification = "Manager",
                    RoleType = Role.Manager
                };
                var result = await userManager.CreateAsync(staff3);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(staff3, password);
                    await userManager.AddToRoleAsync(staff3, manager);
                }
            }

            //Create MANAGER'S ASSISTANT
            if (await userManager.FindByNameAsync("jessica@cccsportcentre.com") == null)
            {
                var staff4 = new Staff
                {
                    UserName = "jessica@cccsportcentre.com",
                    Email = "jessica@cccsportcentre.com",
                    EmailConfirmed = true,
                    FirstName = "Jessica",
                    LastName = "Fancy",
                    HouseNumber = "1",
                    Street = "Fake st",
                    City = "Glasgow",
                    Country = "UK",
                    PostCode = "G6 66G",
                    PhoneNumber = "6969696969",
                    JobTitle = "Manager's Assistant",
                    EmergencyContact = "Bill Gates",
                    EmergencyContDetails = "0666-666-666",
                    CurrentQualification = "Manager'sAssistans",
                    RoleType = Role.ManagerAssistant
                };
                var result = await userManager.CreateAsync(staff4);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(staff4, password);
                    await userManager.AddToRoleAsync(staff4, managerAssistant);
                }
            }

            //Create MEMBER
            if (await userManager.FindByNameAsync("hurdler.run@gmail.com") == null)
            {
                var member1 = new Member
                {
                    UserName = "hurdler.run@gmail.com",
                    Email = "hurdler.run@gmail.com",
                    EmailConfirmed = true,
                    MemberTitle = Title.Mr,
                    FirstName = "Wojtek",
                    LastName = "Bonkowski",
                    HouseNumber = "1",
                    Street = "Fake st",
                    City = "Glasgow",
                    Country = "UK",
                    PostCode = "G6 66G",
                    PhoneNumber = "6969696969",
                    BirthDate = DateTime.Parse("22/04/1990"),
                    GenderType = Gender.Male,
                    RegistrationDate = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddYears(1),
                    CanMakeBooking = true,
                    MembershipTypeId = Adult.Id,
                    PaymentConfirmed = true,
                    PaymentDate = DateTime.Now
                   
                };
                var result = await userManager.CreateAsync(member1);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(member1, password);
                    await userManager.AddToRoleAsync(member1, member);
                }
            }

            //Create MEMBER
            if (await userManager.FindByNameAsync("natalie@gmail.com") == null)
            {
                var member2 = new Member
                {
                    UserName = "natalie@gmail.com",
                    Email = "natalie@gmail.com",
                    EmailConfirmed = true,
                    MemberTitle = Title.Mrs,
                    FirstName = "Natalie",
                    LastName = "Portman",
                    HouseNumber = "1",
                    Street = "Fake st",
                    City = "Glasgow",
                    Country = "UK",
                    PostCode = "G6 66G",
                    PhoneNumber = "6969696969",
                    BirthDate = DateTime.Parse("22/04/1990"),
                    GenderType = Gender.Female,
                    RegistrationDate = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddYears(1),
                    CanMakeBooking = true,
                    MembershipType = Adult,
                    PaymentConfirmed = true,
                    PaymentDate = DateTime.Now
                };
                var result = await userManager.CreateAsync(member2);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(member2, password);
                    await userManager.AddToRoleAsync(member2, member);
                }
            }

            //Create MEMBER
            if (await userManager.FindByNameAsync("agnieszka@gmail.com") == null)
            {
                var member3 = new Member
                {
                    UserName = "agnieszka@gmail.com",
                    Email = "agnieszka@gmail.com",
                    EmailConfirmed = true,
                    MemberTitle = Title.Miss,
                    FirstName = "Agnieszka",
                    LastName = "Sekula",
                    HouseNumber = "1",
                    Street = "Fake st",
                    City = "Glasgow",
                    Country = "UK",
                    PostCode = "G6 66G",
                    PhoneNumber = "6969696969",
                    BirthDate = DateTime.Parse("21/01/1994"),
                    GenderType = Gender.Female,
                    RegistrationDate = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddYears(1),
                    CanMakeBooking = true,
                    MembershipType = Adult,
                    PaymentConfirmed = true,
                    PaymentDate = DateTime.Now
                };
                var result = await userManager.CreateAsync(member3);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(member3, password);
                    await userManager.AddToRoleAsync(member3, member);
                }
            }

        }

    }
}

