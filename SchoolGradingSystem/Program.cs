using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    // Student Class
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Score { get; set; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100)
                return "A";
            else if (Score >= 70 && Score <= 79)
                return "B";
            else if (Score >= 60 && Score <= 69)
                return "C";
            else if (Score >= 50 && Score <= 59)
                return "D";
            else
                return "F";
        }

        public override string ToString()
        {
            return $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
        }
    }

    // Custom Exception Classes
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // Student Result Processor Class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            List<Student> students = new List<Student>();
            int lineNumber = 0;

            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    // Skip empty lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Split the line by comma
                    string[] fields = line.Split(',');

                    // Validate number of fields
                    if (fields.Length != 3)
                    {
                        throw new MissingFieldException(
                            $"Line {lineNumber}: Expected 3 fields (ID, Name, Score) but found {fields.Length}. Line content: '{line}'");
                    }

                    // Trim whitespace from fields
                    for (int i = 0; i < fields.Length; i++)
                    {
                        fields[i] = fields[i].Trim();
                    }

                    // Validate that fields are not empty
                    if (string.IsNullOrEmpty(fields[0]) || string.IsNullOrEmpty(fields[1]) || string.IsNullOrEmpty(fields[2]))
                    {
                        throw new MissingFieldException(
                            $"Line {lineNumber}: One or more fields are empty. Line content: '{line}'");
                    }

                    try
                    {
                        // Parse student ID
                        int id = int.Parse(fields[0]);

                        // Get full name
                        string fullName = fields[1];

                        // Parse score
                        int score = int.Parse(fields[2]);

                        // Validate score range (optional validation)
                        if (score < 0 || score > 100)
                        {
                            throw new InvalidScoreFormatException(
                                $"Line {lineNumber}: Score must be between 0 and 100. Found: {score}");
                        }

                        // Create and add student
                        students.Add(new Student(id, fullName, score));
                    }
                    catch (FormatException)
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: Unable to convert ID or Score to integer. Line content: '{line}'");
                    }
                    catch (OverflowException)
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: ID or Score value is too large. Line content: '{line}'");
                    }
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                // Write header
                writer.WriteLine("=== SCHOOL GRADING REPORT ===");
                writer.WriteLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Total Students: {students.Count}");
                writer.WriteLine(new string('-', 50));
                writer.WriteLine();

                // Write student results
                foreach (Student student in students)
                {
                    writer.WriteLine(student.ToString());
                }

                // Write summary statistics
                writer.WriteLine();
                writer.WriteLine(new string('-', 50));
                writer.WriteLine("=== GRADE DISTRIBUTION ===");

                int countA = 0, countB = 0, countC = 0, countD = 0, countF = 0;
                int totalScore = 0;

                foreach (Student student in students)
                {
                    totalScore += student.Score;

                    switch (student.GetGrade())
                    {
                        case "A": countA++; break;
                        case "B": countB++; break;
                        case "C": countC++; break;
                        case "D": countD++; break;
                        case "F": countF++; break;
                    }
                }

                writer.WriteLine($"Grade A (80-100): {countA} students");
                writer.WriteLine($"Grade B (70-79):  {countB} students");
                writer.WriteLine($"Grade C (60-69):  {countC} students");
                writer.WriteLine($"Grade D (50-59):  {countD} students");
                writer.WriteLine($"Grade F (0-49):   {countF} students");

                if (students.Count > 0)
                {
                    double averageScore = (double)totalScore / students.Count;
                    writer.WriteLine($"Class Average: {averageScore:F2}");
                }
            }
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== School Grading System ===\n");

            // File paths
            string inputFilePath = "students.txt";
            string outputFilePath = "grade_report.txt";

            // Create sample input file for demonstration
            CreateSampleInputFile(inputFilePath);

            try
            {
                // Create processor instance
                StudentResultProcessor processor = new StudentResultProcessor();

                Console.WriteLine($"Reading student data from: {inputFilePath}");

                // Read students from file
                List<Student> students = processor.ReadStudentsFromFile(inputFilePath);

                Console.WriteLine($"Successfully read {students.Count} student records.");

                // Write report to file
                Console.WriteLine($"Writing report to: {outputFilePath}");
                processor.WriteReportToFile(students, outputFilePath);

                Console.WriteLine("✓ Grade report generated successfully!");

                // Display some results on console
                Console.WriteLine("\n=== PREVIEW OF RESULTS ===");
                foreach (Student student in students)
                {
                    Console.WriteLine(student.ToString());
                }

                Console.WriteLine($"\nFull report saved to: {outputFilePath}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"✗ File Error: The input file was not found.");
                Console.WriteLine($"Details: {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"✗ Score Format Error: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"✗ Missing Field Error: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"✗ Access Error: Unable to access the file.");
                Console.WriteLine($"Details: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"✗ IO Error: Problem reading or writing file.");
                Console.WriteLine($"Details: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Unexpected Error: {ex.Message}");
                Console.WriteLine($"Type: {ex.GetType().Name}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        // Helper method to create sample input file for demonstration
        static void CreateSampleInputFile(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("101, Alice Smith, 85");
                    writer.WriteLine("102, Bob Johnson, 72");
                    writer.WriteLine("103, Carol Williams, 91");
                    writer.WriteLine("104, David Brown, 68");
                    writer.WriteLine("105, Emma Davis, 45");
                    writer.WriteLine("106, Frank Miller, 88");
                    writer.WriteLine("107, Grace Wilson, 76");
                }
                Console.WriteLine($"Sample input file '{filePath}' created with test data.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not create sample file. {ex.Message}");
            }
        }
    }
}
