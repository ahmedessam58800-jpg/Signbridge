using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignBridge.Domain.Entities;
using SignBridge.Domain.Enums;

namespace SignBridge.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        await db.Database.EnsureCreatedAsync(cancellationToken);

        if (await db.Users.AnyAsync(cancellationToken))
            return;

        var hasher = new PasswordHasher<User>();

        var admin = CreateUser("Demo Admin", "admin@signbridge.local", UserRole.Admin, "Admin123!", hasher);
        var teacher = CreateUser("Demo Teacher", "teacher@signbridge.local", UserRole.Teacher, "Teacher123!", hasher);
        var parent = CreateUser("Demo Parent", "parent@signbridge.local", UserRole.Parent, "Parent123!", hasher);

        var child = CreateUser("Omar Demo", "child@signbridge.local", UserRole.Child, "Child123!", hasher);
        child.DateOfBirth = new DateOnly(2016, 5, 12);
        child.LinkCode = "KID-482193";

        db.Users.AddRange(admin, teacher, parent, child);

        db.ParentChildLinks.Add(new ParentChildLink
        {
            Parent = parent,
            Child = child
        });

        var course = BuildDemoCourse();
        db.Courses.Add(course);

        await db.SaveChangesAsync(cancellationToken);
    }

    private static User CreateUser(
        string name,
        string email,
        UserRole role,
        string password,
        PasswordHasher<User> hasher)
    {
        var user = new User
        {
            FullName = name,
            Email = email,
            Role = role
        };

        user.PasswordHash = hasher.HashPassword(user, password);
        return user;
    }

    private static Course BuildDemoCourse()
    {
        var course = new Course
        {
            Title = "Everyday Signs",
            Description = "A beginner course covering useful signs for everyday communication.",
            CoverImageUrl = "https://images.example.com/everyday-signs.jpg",
            IsPublished = true
        };

        var level = new Level
        {
            Title = "Level 1 - Getting Started",
            Order = 1
        };

        var greetings = new Lesson
        {
            Title = "Greetings",
            Summary = "Learn basic signs for hello, goodbye, please and thank you.",
            VideoUrl = "https://videos.example.com/greetings.mp4",
            ThumbnailUrl = "https://images.example.com/greetings.jpg",
            DurationMinutes = 6,
            Order = 1,
            XpReward = 50,
            MinimumPassingScore = 80,
            IsPublished = true
        };

        greetings.Quiz = new Quiz
        {
            Title = "Greetings Quiz",
            Questions =
            [
                BuildQuestion(1, "Which option means Hello?", ("Hello", true), ("School", false), ("Water", false)),
                BuildQuestion(2, "Which option means Thank you?", ("Thank you", true), ("Cat", false), ("Book", false))
            ]
        };

        var family = new Lesson
        {
            Title = "Family",
            Summary = "Learn signs for mother, father, sister and brother.",
            VideoUrl = "https://videos.example.com/family.mp4",
            ThumbnailUrl = "https://images.example.com/family.jpg",
            DurationMinutes = 7,
            Order = 2,
            XpReward = 60,
            MinimumPassingScore = 80,
            IsPublished = true
        };

        family.Quiz = new Quiz
        {
            Title = "Family Quiz",
            Questions =
            [
                BuildQuestion(1, "Choose the family-related sign.", ("Mother", true), ("Car", false), ("Red", false)),
                BuildQuestion(2, "Which option is another family member?", ("Brother", true), ("Table", false), ("Milk", false))
            ]
        };

        var food = new Lesson
        {
            Title = "Food and Drink",
            Summary = "Learn everyday signs for water, milk, food and hungry.",
            VideoUrl = "https://videos.example.com/food.mp4",
            ThumbnailUrl = "https://images.example.com/food.jpg",
            DurationMinutes = 8,
            Order = 3,
            XpReward = 70,
            MinimumPassingScore = 80,
            IsPublished = true
        };

        level.Lessons.Add(greetings);
        level.Lessons.Add(family);
        level.Lessons.Add(food);
        course.Levels.Add(level);

        return course;
    }

    private static Question BuildQuestion(
        int order,
        string text,
        params (string Text, bool Correct)[] answers)
    {
        return new Question
        {
            Order = order,
            Text = text,
            Answers = answers.Select(x => new Answer
            {
                Text = x.Text,
                IsCorrect = x.Correct
            }).ToList()
        };
    }
}
