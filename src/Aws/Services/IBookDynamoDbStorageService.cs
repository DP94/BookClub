﻿using Core.Models;

namespace Aws.Services;

public interface IBookDynamoDbStorageService
{
    Task<List<Book>> GetBooks();
    Task<Book> GetBookById(string id);

    Task<Book> CreateBook(Book book);
}