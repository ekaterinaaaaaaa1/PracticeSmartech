using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace LibraryAPI.Controllers
{
    [Route("/api/[controller]")]
    public class BooksController : Controller
    {
        private static List<Book> listBooks = ReadDB();

        public static List<Book> ReadDB()
        {
            string connectionString = "Server = (localdb)\\MSSQLLocalDB; Database=LibraryAPI; Trusted_Connection=True;";

            List<Book> listBooksDB = new List<Book>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "USE [LibraryAPI] SELECT * FROM [books]";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Book book = new Book();
                            book.Id = reader.GetInt32(0);
                            book.BookName = reader.GetString(1);
                            book.Author = reader.GetString(2);
                            book.CountOfPages = reader.GetInt32(3);

                            listBooksDB.Add(book);
                        }
                    }
                }
            }

            return listBooksDB;
        }

        [HttpGet]
        public IEnumerable<Book> Get() => listBooks;

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var book = listBooks.SingleOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            listBooks.Remove(listBooks.SingleOrDefault(b => b.Id == id));

            return Ok();
        }

        private int NextBookId => listBooks.Count() == 0 ? 1 : listBooks.Max(b => b.Id) + 1;

        [HttpGet("GetNextBookId")]
        public int GetNextBookId()
        {
            return NextBookId;
        }

        [HttpPost]
        public IActionResult Post(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            book.Id = NextBookId;
            listBooks.Add(book);

            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }

        [HttpPost("AddBook")]
        public IActionResult PostBody([FromBody] Book book) => Post(book);

        [HttpPut]
        public IActionResult Put(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var storedBook = listBooks.SingleOrDefault(b => b.Id == book.Id);

            if (storedBook == null)
            {
                return NotFound();
            }

            storedBook.BookName = book.BookName;
            storedBook.Author = book.Author;
            storedBook.CountOfPages = book.CountOfPages;

            return Ok(storedBook);
        }

        [HttpPut("UpdateBook")]
        public IActionResult PutBody([FromBody] Book book) => Put(book);
    }
}
