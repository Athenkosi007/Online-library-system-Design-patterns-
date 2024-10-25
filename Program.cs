using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONT412_Assignment
{
    // ============================
    // State Design Pattern
    // ============================
    // Interface for different states of a book
    public interface IBookState
    {
        void Borrow(User user, Book book, List<Book> books, List<Book> borrowedBooks);
        void Return(Book book, List<Book> books, List<Book> borrowedBooks);
        void Reserve(User user, Book book, List<Book> books, List<Book> reservedBooks); // New method for reserving a book
    }

    // Available state of a book
    public class AvailableState : IBookState
    {
        public void Borrow(User user, Book book, List<Book> books, List<Book> borrowedBooks)
        {
            // Check if the user is allowed to borrow the book
            if (book.IsPremium && !user.IsPremium)
            {
                Console.WriteLine($"{user.Name} is not allowed to borrow the premium book: {book.Title}");
            }
            else
            {
                // If the user is premium, verify their password
                if (user.IsPremium)
                {
                    Console.Write("Please enter your password: ");
                    string password = Console.ReadLine();

                    if (password != ((PremiumUser)user).Password) // Check password
                    {
                        Console.WriteLine("Incorrect password. You cannot borrow this book.");
                        return;
                    }
                }

                Console.WriteLine($"{user.Name} borrowed the book: {book.Title}");
                book.SetState(new BorrowedState()); // Change the book's state to Borrowed
                books.Remove(book); // Remove the book from the list of available books
                borrowedBooks.Add(book); // Add the book to the borrowed books list
            }
        }

        public void Return(Book book, List<Book> books, List<Book> borrowedBooks)
        {
            Console.WriteLine("The book is already available. No need to return.");
        }

        public void Reserve(User user, Book book, List<Book> books, List<Book> reservedBooks)
        {
            Console.WriteLine($"{user.Name} reserved the book: {book.Title}");
            book.SetState(new ReservedState()); // Change the book's state to Reserved
            // Remove the book from the list of available books only when it is borrowed
            // books.Remove(book); // Commented out to keep it in the available list
            reservedBooks.Add(book); // Add the book to the reserved books list
            book.ReservedBy = user; // Set the user who reserved the book
        }
    }

    // Borrowed state of a book
    public class BorrowedState : IBookState
    {
        public void Borrow(User user, Book book, List<Book> books, List<Book> borrowedBooks)
        {
            Console.WriteLine($"The book {book.Title} is already borrowed.");
        }

        public void Return(Book book, List<Book> books, List<Book> borrowedBooks)
        {
            Console.WriteLine($"The book {book.Title} has been returned.");
            book.SetState(new AvailableState()); // Change the book's state back to Available
            borrowedBooks.Remove(book); // Remove the book from the borrowed books list
            books.Add(book); // Add the book back to the list of available books
        }

        public void Reserve(User user, Book book, List<Book> books, List<Book> reservedBooks)
        {
            Console.WriteLine($"The book {book.Title} is already borrowed and cannot be reserved.");
        }
    }

    // Reserved state of a book
    public class ReservedState : IBookState
    {
        public void Borrow(User user, Book book, List<Book> books, List<Book> borrowedBooks)
        {
            if (book.ReservedBy == user) // Check if the user is the one who reserved the book
            {
                Console.WriteLine($"{user.Name} borrowed the reserved book: {book.Title}");
                book.SetState(new BorrowedState()); // Change the book's state to Borrowed
                borrowedBooks.Add(book); // Add the book to the borrowed books list
                books.Remove(book); // Remove the book from the list of available books
            }
            else
            {
                Console.WriteLine($"The book {book.Title} is reserved by {book.ReservedBy.Name} and cannot be borrowed by {user.Name}.");
            }
        }

        public void Return(Book book, List<Book> books, List<Book> borrowedBooks)
        {
            Console.WriteLine($"The reserved book {book.Title} has been returned.");
            book.SetState(new AvailableState()); // Change the book's state back to Available
        }

        public void Reserve(User user, Book book, List<Book> books, List<Book> reservedBooks)
        {
            Console.WriteLine($"The book {book.Title} is already reserved.");
        }
    }

    // ============================
    // The Book class utilizes the State pattern
    // ============================
    public class Book
    {
        public string Title { get; private set; }
        public bool IsPremium { get; private set; }
        private IBookState _state;
        public User ReservedBy { get; set; } // Change from private to public

        public Book(string title, bool isPremium)
        {
            Title = title;
            IsPremium = isPremium;
            _state = new AvailableState(); // By default, books are in Available state
        }

        public void SetState(IBookState state)
        {
            _state = state;
        }

        public void Borrow(User user, List<Book> books, List<Book> borrowedBooks)
        {
            _state.Borrow(user, this, books, borrowedBooks); // Delegate borrowing action to the current state
        }

        public void Return(List<Book> books, List<Book> borrowedBooks)
        {
            _state.Return(this, books, borrowedBooks); // Delegate returning action to the current state
        }

        public void Reserve(User user, List<Book> books, List<Book> reservedBooks)
        {
            _state.Reserve(user, this, books, reservedBooks); // Delegate reserving action to the current state
            ReservedBy = user; // Set the user who reserved the book
        }
    }

    // ============================
    // Proxy Design Pattern
    // ============================
    // The User base class acts as a proxy to determine if the user is allowed to borrow premium books
    public abstract class User
    {
        public string Name { get; private set; }
        public abstract bool IsPremium { get; }

        public User(string name)
        {
            Name = name;
        }
    }

    // Regular user cannot borrow premium books
    public class RegularUser : User
    {
        public RegularUser(string name) : base(name) { }
        public override bool IsPremium => false; // Non-premium user
    }

    // Premium user can borrow any book, including premium books
    public class PremiumUser : User
    {
        public string Password { get; private set; } // Password for premium user

        public PremiumUser(string name, string password) : base(name)
        {
            Password = password; // Set the password for the premium user
        }

        public override bool IsPremium => true; // Premium user
    }

    // ============================
    // Iteration Design Pattern
    // ============================
    // Main class that manages the interaction with users and the library system
    class Program
    {
        static void Main(string[] args)
        {
            // Sample users
            User premiumUser = new PremiumUser("Alice", "password123"); // Premium user with a password
            User regularUser = new RegularUser("Bob");

            // Sample books
            Book book1 = new Book("The Great Gatsby", false);  // Non-premium book
            Book book2 = new Book("1984", true);  // Premium book
            Book book3 = new Book("To Kill a Mockingbird", false);  // Non-premium book
            Book book4 = new Book("The Catcher in the Rye", false);  // Non-premium book
            Book book5 = new Book("Pride and Prejudice", false);  // Non-premium book
            Book book6 = new Book("The Lord of the Rings", true);  // Premium book

            // Book collections
            List<Book> books = new List<Book> { book1, book2, book3, book4, book5, book6 }; // Available books
            List<Book> borrowedBooks = new List<Book>(); // Borrowed books
            List<Book> reservedBooks = new List<Book>(); // Reserved books

            // Simple command-line menu for library interaction
            bool running = true;
            while (running)
            {
                Console.Clear(); // Clear the console for a fresh display
                Console.WriteLine("=========================================");
                Console.WriteLine("   Welcome to the Online Library System  ");
                Console.WriteLine("=========================================");
                Console.WriteLine("1. Browse Books"); // Option to view available books
                Console.WriteLine("2. Borrow a Book"); // Option to borrow a book
                Console.WriteLine("3. Return a Book"); // Option to return a borrowed book
                Console.WriteLine("4. Reserve a Book"); // New option to reserve a book
                Console.WriteLine("5. Exit"); // Move Exit option to the last position
                Console.Write("Please choose an option (1-5): ");
                string choice = Console.ReadLine(); // Read user input for menu choice

                // Handle user choice
                switch (choice)
                {
                    case "1":
                        BrowseBooks(books); // Call method to display books
                        break;
                    case "2":
                        BorrowBook(books, borrowedBooks, premiumUser, regularUser); // Call method to borrow a book
                        break;
                    case "3":
                        ReturnBook(books, borrowedBooks); // Call method to return a book
                        break;
                    case "4":
                        ReserveBook(books, reservedBooks, premiumUser, regularUser); // Call method to reserve a book
                        break;
                    case "5":
                        running = false; // Exit the loop to terminate the program
                        Console.WriteLine("Exiting the library system... Thank you for visiting!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again."); // Handle invalid input
                        break;
                }
                Console.WriteLine("Press any key to continue..."); // Prompt to continue
                Console.ReadKey(); // Wait for user input
            }
        }

        // Function to browse books using an Iterator pattern
        static void BrowseBooks(List<Book> books)
        {
            Console.WriteLine("\nBook Catalog:"); // Header for the book catalog
            Console.WriteLine("--------------");
            foreach (var book in books)
            {
                // Display each book's title and whether it is premium or reserved
                string status = book.ReservedBy != null ? "Reserved" : (book.IsPremium ? "Premium" : "Available");
                Console.WriteLine($"- {book.Title} (Status: {status})");
            }
            Console.WriteLine("--------------");
        }

        // Function to simulate borrowing a book
        static void BorrowBook(List<Book> books, List<Book> borrowedBooks, User premiumUser, User regularUser)
        {
            Console.WriteLine("\nEnter the user type (1 for Premium, 2 for Regular): ");
            string userType = Console.ReadLine(); // Read user type input
            User user = userType == "1" ? premiumUser : regularUser; // Determine user type

            // Check and inform the user about their membership status
            if (user.IsPremium)
            {
                Console.WriteLine($"{user.Name} is a premium member.");
            }
            else
            {
                Console.WriteLine($"{user.Name} is a regular member and cannot borrow premium books.");
            }

            Console.WriteLine("Enter the title of the book to borrow: ");
            string bookTitle = Console.ReadLine(); // Read the book title to borrow

            // Find the book in the collection by title
            Book book = books.Find(b => b.Title.Equals(bookTitle, StringComparison.OrdinalIgnoreCase));

            if (book != null)
            {
                book.Borrow(user, books, borrowedBooks); // Attempt to borrow the book and pass the lists
            }
            else
            {
                Console.WriteLine("Book not found. Please check the title and try again."); // Handle book not found
            }
        }

        // Function to simulate returning a book
        static void ReturnBook(List<Book> books, List<Book> borrowedBooks)
        {
            Console.WriteLine("\nEnter the title of the book to return: ");
            string bookTitle = Console.ReadLine(); // Read the book title to return

            // Find the book in the borrowed books list by title
            Book book = borrowedBooks.Find(b => b.Title.Equals(bookTitle, StringComparison.OrdinalIgnoreCase));

            if (book != null)
            {
                book.Return(books, borrowedBooks); // Attempt to return the book and pass the lists
            }
            else
            {
                Console.WriteLine("Book not found in borrowed books or may have been returned. Please check the title and try again."); // Handle book not found
            }
        }

        // Function to simulate reserving a book
        static void ReserveBook(List<Book> books, List<Book> reservedBooks, User premiumUser, User regularUser)
        {
            Console.WriteLine("\nEnter the user type (1 for Premium, 2 for Regular): ");
            string userType = Console.ReadLine(); // Read user type input
            User user = userType == "1" ? premiumUser : regularUser; // Determine user type

            Console.WriteLine("Enter the title of the book to reserve: ");
            string bookTitle = Console.ReadLine(); // Read the book title to reserve

            // Find the book in the collection by title
            Book book = books.Find(b => b.Title.Equals(bookTitle, StringComparison.OrdinalIgnoreCase));

            if (book != null)
            {
                book.Reserve(user, books, reservedBooks); // Attempt to reserve the book and pass the lists
            }
            else
            {
                Console.WriteLine("Book not found. Please check the title and try again."); // Handle book not found
            }
        }
    }
}
// ============================
// End of Program
// ============================