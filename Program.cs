using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONT412_Assignment
{
    using System;
    using System.Collections.Generic;

    // State Design Pattern for handling different states of a book
    public interface IBookState
    {
        void Borrow(User user, Book book);
        void Return(Book book);
    }

    public class AvailableState : IBookState
    {
        public void Borrow(User user, Book book)
        {
            // Check if the user is allowed to borrow the book
            if (book.IsPremium && !user.IsPremium)
            {
                Console.WriteLine($"{user.Name} is not allowed to borrow the premium book: {book.Title}");
            }
            else
            {
                Console.WriteLine($"{user.Name} borrowed the book: {book.Title}");
                book.SetState(new BorrowedState()); // Change the book's state to Borrowed
            }
        }

        public void Return(Book book)
        {
            Console.WriteLine("The book is already available. No need to return.");
        }
    }

    public class BorrowedState : IBookState
    {
        public void Borrow(User user, Book book)
        {
            Console.WriteLine($"The book {book.Title} is already borrowed.");
        }

        public void Return(Book book)
        {
            Console.WriteLine($"The book {book.Title} has been returned.");
            book.SetState(new AvailableState()); // Change the book's state back to Available
        }
    }

    public class ReservedState : IBookState
    {
        public void Borrow(User user, Book book)
        {
            Console.WriteLine($"The book {book.Title} is reserved and cannot be borrowed.");
        }

        public void Return(Book book)
        {
            Console.WriteLine($"The reserved book {book.Title} has been returned.");
            book.SetState(new AvailableState());
        }
    }

    // The Book class utilizes the State pattern to handle different states of the book
    public class Book
    {
        public string Title { get; private set; }
        public bool IsPremium { get; private set; }
        private IBookState _state;

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

        public void Borrow(User user)
        {
            _state.Borrow(user, this);
        }

        public void Return()
        {
            _state.Return(this);
        }
    }

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
        public override bool IsPremium => false;
    }

    // Premium user can borrow any book, including premium books
    public class PremiumUser : User
    {
        public PremiumUser(string name) : base(name) { }
        public override bool IsPremium => true;
    }

    // Main class that manages the interaction with users and the library system
    class Program
    {
        static void Main(string[] args)
        {
            // Sample users
            User premiumUser = new PremiumUser("Alice");
            User regularUser = new RegularUser("Bob");

            // Sample books
            Book book1 = new Book("The Great Gatsby", false);  // Non-premium book
            Book book2 = new Book("1984", true);  // Premium book

            // Book collection (Iterator pattern is used here to iterate over the book collection)
            List<Book> books = new List<Book> { book1, book2 };

            // Simple command-line menu for library interaction
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Online Library System");
                Console.WriteLine("1. Browse Books");
                Console.WriteLine("2. Borrow a Book");
                Console.WriteLine("3. Return a Book");
                Console.WriteLine("4. Exit");
                Console.Write("Please choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        BrowseBooks(books);
                        break;
                    case "2":
                        BorrowBook(books, premiumUser, regularUser);
                        break;
                    case "3":
                        ReturnBook(books);
                        break;
                    case "4":
                        running = false;
                        Console.WriteLine("Exiting the library system...");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        // Function to browse books using an Iterator pattern
        static void BrowseBooks(List<Book> books)
        {
            Console.WriteLine("\nBook Catalog:");
            foreach (var book in books)
            {
                Console.WriteLine($"- {book.Title} (Premium: {book.IsPremium})");
            }
        }

        // Function to simulate borrowing a book
        static void BorrowBook(List<Book> books, User premiumUser, User regularUser)
        {
            Console.WriteLine("\nEnter the user type (1 for Premium, 2 for Regular): ");
            string userType = Console.ReadLine();
            User user = userType == "1" ? premiumUser : regularUser;

            Console.WriteLine("Enter the title of the book to borrow: ");
            string bookTitle = Console.ReadLine();

            Book book = books.Find(b => b.Title.Equals(bookTitle, StringComparison.OrdinalIgnoreCase));

            if (book != null)
            {
                book.Borrow(user);
            }
            else
            {
                Console.WriteLine("Book not found.");
            }
        }

        // Function to simulate returning a book
        static void ReturnBook(List<Book> books)
        {
            Console.WriteLine("\nEnter the title of the book to return: ");
            string bookTitle = Console.ReadLine();

            Book book = books.Find(b => b.Title.Equals(bookTitle, StringComparison.OrdinalIgnoreCase));

            if (book != null)
            {
                book.Return();
            }
            else
            {
                Console.WriteLine("Book not found.");
            }
        }
    }

}
