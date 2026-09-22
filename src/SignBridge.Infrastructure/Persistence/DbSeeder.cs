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

        await SeedUsersAsync(db, cancellationToken);
        await RefreshSeedProfileNamesAsync(db, cancellationToken);
        await SeedCoursesAsync(db, cancellationToken);
        await SeedDictionaryAsync(db, cancellationToken);
        await SeedAchievementsAsync(db, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedUsersAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Users.AnyAsync(cancellationToken))
            return;

        var hasher = new PasswordHasher<User>();

        var admin = CreateUser("Platform Admin", "admin@signbridge.local", UserRole.Admin, "Admin123!", hasher);
        var teacher = CreateUser("Course Teacher", "teacher@signbridge.local", UserRole.Teacher, "Teacher123!", hasher);
        var parent = CreateUser("Sara Hassan", "parent@signbridge.local", UserRole.Parent, "Parent123!", hasher);
        var child = CreateUser("Omar Hassan", "child@signbridge.local", UserRole.Child, "Child123!", hasher);

        child.DateOfBirth = new DateOnly(2016, 5, 12);
        child.LinkCode = "KID-482193";

        db.Users.AddRange(admin, teacher, parent, child);
        db.ParentChildLinks.Add(new ParentChildLink { Parent = parent, Child = child });

        await db.SaveChangesAsync(cancellationToken);
    }


    private static async Task RefreshSeedProfileNamesAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        var parent = await db.Users.FirstOrDefaultAsync(x => x.Email == "parent@signbridge.local", cancellationToken);
        if (parent is not null && parent.FullName == "Demo Parent")
            parent.FullName = "Sara Hassan";

        var child = await db.Users.FirstOrDefaultAsync(x => x.Email == "child@signbridge.local", cancellationToken);
        if (child is not null && child.FullName == "Omar Demo")
            child.FullName = "Omar Hassan";

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedCoursesAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Courses.AnyAsync(cancellationToken))
            return;

        db.Courses.AddRange(BuildCourseLibrary());
    }

    private static IReadOnlyList<Course> BuildCourseLibrary()
    {
        return new List<Course>
        {
            Course(
                "Arabic Alphabet - Visual Learning",
                "A visual introduction to Arabic letters from Alif onward. This course uses a child-friendly Arabic alphabet video as a literacy resource; it is not presented as Arabic Sign Language instruction.",
                "/course-covers/arabic-alphabet.svg",
                Level("Arabic Letters", 1,
                    Lesson(
                        "Arabic Alphabet from Alif to Ya",
                        "Watch the Arabic alphabet visually, then answer a short recognition quiz. Video source: Mila Kids Arabic.",
                        "https://www.youtube.com/watch?v=_ZUxN0DWhkA",
                        1, 6, 60,
                        Q(1, "Which letter comes first in the Arabic alphabet?", "ا", "ب", "ت"),
                        Q(2, "Which letter comes after ب?", "ت", "ا", "ج"),
                        Q(3, "Which of these is an Arabic letter?", "م", "B", "7"))
                )
            ),

            Course(
                "English Alphabet in ASL",
                "Learn A-Z fingerspelling in American Sign Language using a complete beginner alphabet lesson.",
                "/course-covers/asl-alphabet.svg",
                Level("ASL Fingerspelling", 1,
                    Lesson(
                        "A-Z Fingerspelling",
                        "Practice all 26 letters of the ASL manual alphabet. Video source: Learn How to Sign.",
                        "https://www.youtube.com/watch?v=DBQINq0SsAw",
                        1, 13, 80,
                        Q(1, "How many letters are in the English alphabet?", "26", "20", "30"),
                        Q(2, "Which letter comes after F?", "G", "E", "H"),
                        Q(3, "Which sequence is in alphabetical order?", "A, B, C", "C, A, B", "B, C, A"))
                )
            ),

            Course(
                "Numbers in ASL",
                "Learn number signs from 1 upward, with a strong focus on the numbers children use every day.",
                "/course-covers/asl-numbers.svg",
                Level("Counting", 1,
                    Lesson(
                        "Numbers 1 to 20",
                        "Practice ASL numbers 1-20 and watch how handshapes change. Video source: Learn How to Sign.",
                        "https://www.youtube.com/watch?v=Y4stD_ypaAI",
                        1, 12, 75,
                        Q(1, "What number comes after 9?", "10", "8", "11"),
                        Q(2, "What number comes before 20?", "19", "18", "21"),
                        Q(3, "Which number is greater?", "15", "5", "3"))
                )
            ),

            Course(
                "School & Colors in ASL",
                "Build useful school vocabulary and learn common color signs in American Sign Language.",
                "/course-covers/school-colors.svg",
                Level("School Vocabulary", 1,
                    Lesson(
                        "School Signs",
                        "Learn signs including book, write, student, teacher, class and bus. Video source: Learn How to Sign.",
                        "https://www.youtube.com/watch?v=J5H_HDSipbY&t=65s",
                        1, 7, 70,
                        Q(1, "Who teaches a class?", "Teacher", "Student", "Bus"),
                        Q(2, "Which item is used for reading?", "Book", "Class", "Teacher")),
                    Lesson(
                        "Colors in ASL",
                        "Learn signs for black, blue, brown, green, orange, pink, purple, red, white and yellow. Video source: Learn How to Sign.",
                        "https://www.youtube.com/watch?v=J5H_HDSipbY&t=211s",
                        2, 5, 70,
                        Q(1, "Which option is a color?", "Blue", "Teacher", "Bus"),
                        Q(2, "Which option is another color?", "Yellow", "Book", "Student"))
                )
            ),

            Course(
                "Family Signs in ASL",
                "Learn beginner family signs including mom, dad, parent, sister and brother.",
                "/course-covers/family-signs.svg",
                Level("My Family", 1,
                    Lesson(
                        "Family Members",
                        "Practice common ASL family signs with a beginner-friendly tutorial. Video source: ASL with Destiny.",
                        "https://www.youtube.com/watch?v=yJGdDwFXBtA",
                        1, 4, 65,
                        Q(1, "Which word names a family member?", "Mother", "Blue", "Bus"),
                        Q(2, "Which word means a male sibling?", "Brother", "Teacher", "Water"),
                        Q(3, "Which word means a female sibling?", "Sister", "School", "Red"))
                )
            ),

            Course(
                "Math Basics in Sign Language",
                "A visual introduction to early arithmetic vocabulary and simple addition/subtraction for Deaf learners.",
                "/course-covers/math-sign.svg",
                Level("Grade 1 Math", 1,
                    Lesson(
                        "Addition & Subtraction",
                        "See a child work with addition and subtraction in ASL. Video source: Handspeak.",
                        "https://www.youtube.com/watch?v=Qayy0veJjQA",
                        1, 5, 85,
                        Q(1, "What is 2 + 3?", "5", "4", "6"),
                        Q(2, "What is 5 - 2?", "3", "2", "4"),
                        Q(3, "What is 4 + 1?", "5", "3", "6")),
                    Lesson(
                        "Four Basic Math Operations",
                        "Learn the signs used for addition, subtraction, multiplication, division and equals. The source demonstrates FSL and ASL math vocabulary.",
                        "https://www.youtube.com/watch?v=aOfP3lU2b1M",
                        2, 3, 90,
                        Q(1, "Which symbol means addition?", "+", "-", "÷"),
                        Q(2, "Which symbol means subtraction?", "-", "+", "×"),
                        Q(3, "Which symbol means equals?", "=", "÷", "+"))
                )
            )
        };
    }

    private static Course Course(string title, string description, string coverImageUrl, params Level[] levels)
    {
        var course = new Course
        {
            Title = title,
            Description = description,
            CoverImageUrl = coverImageUrl,
            IsPublished = true
        };

        foreach (var level in levels)
            course.Levels.Add(level);

        return course;
    }

    private static Level Level(string title, int order, params Lesson[] lessons)
    {
        var level = new Level { Title = title, Order = order };

        foreach (var lesson in lessons)
            level.Lessons.Add(lesson);

        return level;
    }

    private static Lesson Lesson(
        string title,
        string summary,
        string videoUrl,
        int order,
        int durationMinutes,
        int xpReward,
        params Question[] questions)
    {
        return new Lesson
        {
            Title = title,
            Summary = summary,
            VideoUrl = videoUrl,
            DurationMinutes = durationMinutes,
            Order = order,
            XpReward = xpReward,
            MinimumPassingScore = 80,
            IsPublished = true,
            Quiz = new Quiz
            {
                Title = $"{title} Quiz",
                Questions = questions.ToList()
            }
        };
    }

    private static Question Q(int order, string text, string correct, string wrong1, string wrong2)
    {
        return BuildQuestion(order, text, (correct, true), (wrong1, false), (wrong2, false));
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

    private static async Task SeedDictionaryAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.SignEntries.AnyAsync(cancellationToken))
            return;

        db.SignEntries.AddRange(
            S("Hello", "مرحبا", "Greetings", "A common greeting."),
            S("Goodbye", "وداعا", "Greetings", "Used when leaving."),
            S("Please", "من فضلك", "Greetings", "A polite request word."),
            S("Thank you", "شكرا", "Greetings", "A polite expression of thanks."),
            S("Mother", "أم", "Family", "A family member."),
            S("Father", "أب", "Family", "A family member."),
            S("Brother", "أخ", "Family", "A male sibling."),
            S("Sister", "أخت", "Family", "A female sibling."),
            S("School", "مدرسة", "School", "A place for learning."),
            S("Teacher", "معلم", "School", "A person who teaches."),
            S("Student", "طالب", "School", "A person who learns."),
            S("Book", "كتاب", "School", "A classroom learning item."),
            S("Red", "أحمر", "Colors", "A basic color."),
            S("Blue", "أزرق", "Colors", "A basic color."),
            S("Green", "أخضر", "Colors", "A basic color."),
            S("Yellow", "أصفر", "Colors", "A basic color."),
            S("One", "واحد", "Numbers", "The number one."),
            S("Two", "اثنان", "Numbers", "The number two."),
            S("Three", "ثلاثة", "Numbers", "The number three."),
            S("Add", "جمع", "Math", "The addition operation."),
            S("Subtract", "طرح", "Math", "The subtraction operation."),
            S("Equals", "يساوي", "Math", "Shows that two values are equal.")
        );
    }

    private static SignEntry S(string word, string arabic, string category, string description)
    {
        return new SignEntry
        {
            Word = word,
            ArabicWord = arabic,
            Category = category,
            Difficulty = "Beginner",
            Description = description,
            IsPublished = true
        };
    }

    private static async Task SeedAchievementsAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Achievements.AnyAsync(cancellationToken))
            return;

        db.Achievements.AddRange(
            new Achievement { Name = "First Step", Description = "Reach 50 XP.", Icon = "🌱", RequiredXp = 50 },
            new Achievement { Name = "On a Roll", Description = "Reach 100 XP.", Icon = "🔥", RequiredXp = 100 },
            new Achievement { Name = "Sign Explorer", Description = "Reach 250 XP.", Icon = "🧭", RequiredXp = 250 },
            new Achievement { Name = "Bridge Builder", Description = "Reach 500 XP.", Icon = "🏆", RequiredXp = 500 }
        );
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
}
