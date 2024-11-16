using HtmlAgilityPack;

namespace bible_parser;

public record Book(List<Chapter> Chapters);
public record Chapter(List<Verse> Verses);
public record Verse(string Book, int Chapter, int Number, string Text);

public class Program
{
    public static void Main()
    {
        // Phase 1 - WebScraper - Create text files representing the Books (45min)
        // List<string> bookNames = WebScraper.ParseBooksList("books.txt");
        // WebScraper.ParseWebBible(bookNames);

        // Phase 2 - FileScraper - Parse text files for more efficient processing (500ms)
        List<Book> books = FileScraper.ParseDirectory("books");
    }
}
