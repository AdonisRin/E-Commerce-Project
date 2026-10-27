/*
 The accounts for testing, they all share the same password

 Users:

First account:
 email     :    athlete@lift.com
 password  :    Testing1

Second account:   
email      :   girl@lift.com
password   :   Testing1
 
 Admins: 

First account:
 email     :    admin@lift.com
 password  :    Testing1

 Second account: 
 email     :   alex@lift.com
 password  :    Testing1
*/
using LiftAndAesthetics.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<LiftContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            if (!context.Memberships.Any())
            {
                context.Memberships.AddRange(
                    new Membership { Name = "Newbie", Price = 120.00m, Description = "Just starting out? This is for you.", Features = "Locker Access, Showers, Basic Equipment" },
                    new Membership { Name = "PRO", Price = 250.00m, Description = "For the dedicated lifter.", Features = "All Newbie features, Sauna Access, Free Towel" },
                    new Membership { Name = "Elite", Price = 300.00m, Description = "The ultimate fitness experience.", Features = "All PRO features, Personal trainers" }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    // Supplements
                    new Product { Name = "100% Whey Protein Gold 2kg", Price = 280.00m, ImageUrl = "/photos/protein.png", Description = "24g of pure protein per scoop for maximum gains.", Category = "Supplements" },
                    new Product { Name = "Micronized Creatine Monohydrate", Price = 67.00m, ImageUrl = "/photos/creatine.png", Description = "Explosive power and muscle cellular ATP fullness.", Category = "Supplements" },
                    new Product { Name = "Pre-Workout C4 Extreme", Price = 130.00m, ImageUrl = "/photos/preworkout.png", Description = "Wake up and tear the gym apart. Extreme energy.", Category = "Supplements" },
                    new Product { Name = "Protein Bar Crunch Box", Price = 84.00m, ImageUrl = "/photos/proteinbar.png", Description = "12 high protein bars for post-workout recovery.", Category = "Supplements" },
                    new Product { Name = "BCAA 2:1:1 Amino Recovery", Price = 95.00m, ImageUrl = "/photos/protein.png", Description = "Branched-chain amino acids with added electrolytes.", Category = "Supplements" },
                    new Product { Name = "Elite Athlete Multivitamin Pack", Price = 65.00m, ImageUrl = "/photos/creatine.png", Description = "Complete daily micronutrient vitamins and minerals.", Category = "Supplements" },
                    new Product { Name = "Omega 3 Ultra Fish Oil", Price = 55.00m, ImageUrl = "/photos/protein.png", Description = "Heart health, joint mobility and recovery.", Category = "Supplements" },
                    new Product { Name = "Slow-Release Casein Protein", Price = 240.00m, ImageUrl = "/photos/protein.png", Description = "Nighttime slow-digesting protein matrix.", Category = "Supplements" },

                    // Gym Clothing
                    new Product { Name = "Heavyweight Oversized Pump Cover", Price = 140.00m, ImageUrl = "/photos/lift.png", Description = "100% combed cotton 280 GSM heavyweight drop-shoulder tee.", Category = "Clothing" },
                    new Product { Name = "Aesthetics Deep-Cut Stringer", Price = 85.00m, ImageUrl = "/photos/lift.png", Description = "Razorback cut with low armholes to accentuate back and shoulders.", Category = "Clothing" },
                    new Product { Name = "5-Inch Inseam Training Shorts", Price = 110.00m, ImageUrl = "/photos/cardio1.png", Description = "Stretch fabric with zippered phone pocket for unrestricted squats.", Category = "Clothing" },
                    new Product { Name = "Seamless Muscle Compression Tights", Price = 125.00m, ImageUrl = "/photos/cardio2.png", Description = "Targeted compression and moisture-wicking nylon blend.", Category = "Clothing" },
                    new Product { Name = "Raw Edge Fleece Workout Hoodie", Price = 190.00m, ImageUrl = "/photos/lift.png", Description = "Brushed fleece interior with athletic tapered fit.", Category = "Clothing" },
                    new Product { Name = "Ribbed High-Support Sports Bra", Price = 95.00m, ImageUrl = "/photos/stretching.png", Description = "Maximum comfort and mobility during heavy lifting sets.", Category = "Clothing" },
                    new Product { Name = "Tapered Performance Joggers", Price = 160.00m, ImageUrl = "/photos/cardio.png", Description = "French terry blend with zippered deep pockets.", Category = "Clothing" },
                    new Product { Name = "Cushioned Crew Lifting Socks (3-Pack)", Price = 45.00m, ImageUrl = "/photos/stretching1.png", Description = "Reinforced arch compression and deadlift shin padding.", Category = "Clothing" },

                    // Lifting Equipment
                    new Product { Name = "10mm Heavy-Duty Lever Lifting Belt", Price = 380.00m, ImageUrl = "/photos/weightlifting.png", Description = "IPF grade genuine cowhide leather with alloy stainless steel lever.", Category = "Equipment" },
                    new Product { Name = "Figure 8 Heavy Deadlift Straps", Price = 75.00m, ImageUrl = "/photos/weightlifting1.png", Description = "Triple-stitched heavy cotton webbing tested past 500kg.", Category = "Equipment" },
                    new Product { Name = "Stiff Wrist Wraps (24 Inch)", Price = 80.00m, ImageUrl = "/photos/weightlifting.png", Description = "Heavy elastic cast-grade wraps to stabilize heavy presses.", Category = "Equipment" },
                    new Product { Name = "7mm Neoprene Knee Sleeves", Price = 210.00m, ImageUrl = "/photos/weightlifting1.png", Description = "Joint warmth, compression, and rebound out of the squat hole.", Category = "Equipment" },
                    new Product { Name = "Anhydrous Liquid Chalk (250ml)", Price = 55.00m, ImageUrl = "/photos/lift.png", Description = "Fast-drying magnesium carbonate with zero dust mess.", Category = "Equipment" },
                    new Product { Name = "Resistance Loop Bands (5-Pack)", Price = 120.00m, ImageUrl = "/photos/stretching.png", Description = "Latex bands with tensions from 15 lbs to 125 lbs.", Category = "Equipment" },
                    new Product { Name = "Leather Dip Belt with Chain", Price = 175.00m, ImageUrl = "/photos/weightlifting.png", Description = "Heavy steel chain for weighted dips and pull-ups.", Category = "Equipment" },
                    new Product { Name = "Dual-Wheel Ab Carver Roller", Price = 95.00m, ImageUrl = "/photos/cardio2.png", Description = "Internal carbon steel spring for core stabilization.", Category = "Equipment" },

                    // Accessories
                    new Product { Name = "Stainless Steel Shaker Bottle 750ml", Price = 85.00m, ImageUrl = "/photos/protein.png", Description = "Double-wall vacuum insulation keeps liquids ice-cold 24h.", Category = "Accessories" },
                    new Product { Name = "Half Gallon Hydration Jug (2.2L)", Price = 65.00m, ImageUrl = "/photos/cardio1.png", Description = "BPA-free shatterproof jug with leak-proof straw spout.", Category = "Accessories" },
                    new Product { Name = "Silicone Rubber Palm Lifting Grips", Price = 50.00m, ImageUrl = "/photos/lift.png", Description = "Ergonomic palm protection against calluses and slipping.", Category = "Accessories" },
                    new Product { Name = "Tactical Gym Duffle Bag (45L)", Price = 230.00m, ImageUrl = "/photos/cardio.png", Description = "Waterproof nylon with isolated ventilated shoe pocket.", Category = "Accessories" },
                    new Product { Name = "Insulated 3-Meal Prep Cooler Bag", Price = 185.00m, ImageUrl = "/photos/ricemeat.png", Description = "Includes 3 leak-proof BPA-free containers and ice packs.", Category = "Accessories" },
                    new Product { Name = "High-Density Foam Barbell Neck Pad", Price = 60.00m, ImageUrl = "/photos/weightlifting.png", Description = "Neck groove relief for heavy barbell squats and hip thrusts.", Category = "Accessories" }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Trainers.Any())
            {
                context.Trainers.AddRange(
                    new Trainer { Name = "David Goggins", Specialization = "Bodybuilding & Hypertrophy", Motto = "They don't know me son!", PhotoUrl = "/photos/david.jpg" },
                    new Trainer { Name = "Maria Fitness", Specialization = "Yoga & Mobility", Motto = "Flexibility is the key to longevity.", PhotoUrl = "/photos/maria.jpg" },
                    new Trainer { Name = "Alex Cross", Specialization = "CrossFit & Conditioning", Motto = "Push past your breaking point.", PhotoUrl = "/photos/alex.jpg" }
                );
                await context.SaveChangesAsync();
            }

            var trainers = context.Trainers.ToList();
            int trainerId1 = trainers.Count > 0 ? trainers[0].TrainerId : 1;
            int trainerId2 = trainers.Count > 1 ? trainers[1].TrainerId : trainerId1;
            int trainerId3 = trainers.Count > 2 ? trainers[2].TrainerId : trainerId1;

            if (!context.FitnessClasses.Any())
            {
                context.FitnessClasses.AddRange(
                    new FitnessClass
                    {
                        ClassName = "Hardcore Workout",
                        Description = "Push past your limits.",
                        MainPhotoUrl = "/photos/weightlifting.png",
                        ExtraPhoto1Url = "/photos/lift.png",

                        VideoUrl = "https://www.youtube.com/watch?v=S7Q1yC_diOw",
                        ScheduleTime = DateTime.Now.AddDays(1),
                        TrainerId = trainerId1
                    },
                    new FitnessClass
                    {
                        ClassName = "Intense Cardio",
                        Description = "Melt fat and improve your stamina. No excuses.",
                        MainPhotoUrl = "/photos/cardio.png",
                        ExtraPhoto1Url = "/photos/cardio1.png",
                        ExtraPhoto2Url = "/photos/cardio2.png",
                        VideoUrl = "https://www.youtube.com/watch?v=wIynl3at0Rs",
                        ScheduleTime = DateTime.Now.AddDays(2),
                        TrainerId = trainerId2
                    },
                    new FitnessClass
                    {
                        ClassName = "Stretching & Relaxing",
                        Description = "Start your day with deep stretches, mobility work, and core control.",
                        MainPhotoUrl = "/photos/stretching1.png",
                        ExtraPhoto1Url = "/photos/stretching.png",
                        VideoUrl = "https://www.youtube.com/watch?v=FI51zRzgIe4",
                        ScheduleTime = DateTime.Now.AddDays(3),
                        TrainerId = trainerId3
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Set<NutritionPlan>().Any())
            {
                context.Set<NutritionPlan>().AddRange(
                    new NutritionPlan { Title = "Lean Rice & Meat", Calories = 650, Description = "The classic bro meal. High protein, clean carbs.", ImageUrl = "/photos/ricemeat.png", Category = "Bulking" },
                    new NutritionPlan { Title = "Dirty Bulk Burger", Calories = 1200, Description = "When you need calories and you need them now.", ImageUrl = "/photos/burger.png", Category = "Bulking" },
                    new NutritionPlan { Title = "Cutting Soup", Calories = 250, Description = "Low calorie, high volume. Keep the hunger away.", ImageUrl = "/photos/soup.png", Category = "Cutting" }
                );
                await context.SaveChangesAsync();
            }


            string defaultPassword = "Testing1";

            var usersToCreate = new[]
            {
                new { Email = "admin@lift.com", Name = "Zeus ", Role = "Admin", Wallet = 99999.00m, Pic = "/photos/david.jpg" },
                new { Email = "alex@lift.com", Name = "Alex ", Role = "Admin", Wallet = 5000.00m, Pic = "/photos/alex.jpg" },
                new { Email = "athlete@lift.com", Name = "Chad Gymbro", Role = "User", Wallet = 6767.00m, Pic = "/photos/david.jpg" },
                new { Email = "girl@lift.com", Name = "Gym Girl", Role = "User", Wallet = 6767.00m, Pic = "/photos/maria.jpg" }
            };

            foreach (var u in usersToCreate)
            {
                if (await userManager.FindByEmailAsync(u.Email) == null)
                {
                    var identityUser = new IdentityUser { UserName = u.Email, Email = u.Email };
                    var result = await userManager.CreateAsync(identityUser, defaultPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(identityUser, u.Role);

                        if (!context.Members.Any(m => m.Email == u.Email))
                        {
                            context.Members.Add(new Members
                            {
                                Name = u.Name,
                                Email = u.Email,
                                Password = identityUser.PasswordHash,
                                Role = u.Role,
                                WalletBalance = u.Wallet,
                                ProfilePictureUrl = u.Pic
                            });
                        }
                    }
                }
            }
            await context.SaveChangesAsync();



            if (!context.Reviews.Any())
            {
                var chad = context.Members.FirstOrDefault(m => m.Email == "athlete@lift.com");
                var gymGirl = context.Members.FirstOrDefault(m => m.Email == "girl@lift.com");
                var zeus = context.Members.FirstOrDefault(m => m.Email == "admin@lift.com");

                if (chad != null && gymGirl != null && zeus != null)
                {
                    context.Reviews.AddRange(
                         new Review
                         {
                             MemberId = chad.MemberId,
                             TargetCategory = "Shop",
                             Rating = 5,
                             Comment = "The protein bars taste like cardboard, but the pump is insane.",
                             CreatedAt = DateTime.Now.AddDays(-2)
                         },
                         new Review
                         {
                             MemberId = gymGirl.MemberId,
                             TargetCategory = "Trainers",
                             Rating = 4,
                             Comment = "Trainer Alex made me do exercises until I saw stars. Great workout, but I hate him so much.",
                             CreatedAt = DateTime.Now.AddDays(-5)
                         },
                         new Review
                         {
                             MemberId = chad.MemberId,
                             TargetCategory = "FitnessClasses",
                             Rating = 2,
                             Comment = "Hot water died exactly when I hit the showers after cardio class... other than that, it's all good.",
                             CreatedAt = DateTime.Now.AddDays(-10),
                             AdminReply = "A pipe burst on the 2nd floor but it's fixed now . See you at the gym!",
                             ReplyDate = DateTime.Now.AddDays(-9)
                         },
                         new Review
                         {
                             MemberId = zeus.MemberId,
                             TargetCategory = "Shop",
                             Rating = 1,
                             Comment = "Whoever left the 50KG dumbbells on the floor is getting INSTA BANNED. Don't make me check the security cams.",
                             CreatedAt = DateTime.Now.AddHours(-3)
                         }
                     );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}