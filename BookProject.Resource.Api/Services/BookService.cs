﻿using BookProject.Resource.Api.Entities;
using BookProject.Resource.Api.Models.Book;
using BookProject.Resource.Api.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookProject.Resource.Api.Services
{
    public class BookService : IBookService
    {
        private readonly ProjectDbContext _context;

        public BookService(ProjectDbContext context)
        {
            _context = context;
        }
        //For Admin
        public void AddBookToItems(Book item)
        {
            _context.Books.Add(item);
            _context.SaveChanges();
        }

        public Book DeleteBookFromItems(int id)
        {
            var book = _context.Books.FirstOrDefault(x => x.Id == id);
            if (book == null) return null;
            _context.Books.Remove(book);
            _context.SaveChanges();
            return book;
        }

        public void UpdateBook(Book item)
        {

            if (_context.Books.Where(b => b.Id == item.Id) == null) return;
            _context.Books.Update(item);
            _context.SaveChanges();
        }


        // For Users
        public List<CartItem> GetItemsInCart(int userId)
        {
            List<CartItem> cartItems = new List<CartItem>();
            List<UserCart> items = _context.UserCart.Where(i => i.UserId == userId).ToList();
            foreach(var item in items)
            {
                Book book = _context.Books.FirstOrDefault(x => x.Id == item.BookId);
                if(book == null) continue;
                CartItem cartItem = new CartItem()
                {
                    Id = book.Id,
                    Url = book.Url,
                    Title = book.Title,
                    Author = book.Author,
                    Price = book.Price,
                    Count = item.Count
                };
                cartItems.Add(cartItem);
            }
            return cartItems;
        }
        public UserCart AddBookToCart(int bookId, int userId)
        {
            Book bookItem = _context.Books.Where(b => b.Id == bookId).FirstOrDefault();
            if (bookItem == null)
                return null;

            UserCart item = _context.UserCart.Where(c => c.BookId == bookId && c.UserId==userId).FirstOrDefault();

            if (item != null)
            {
                item.Count += 1;
                _context.UserCart.Update(item);
            }
            else
            {
                item = new UserCart()
                {
                    UserId = userId,
                    BookId = bookId,
                    Count = 1
                };
                _context.UserCart.Add(item);
            }

            _context.SaveChanges();
            return item;
        }

        public UserCart DeleteBookFromCart(int id, int userId)
        {
            var itemInCart = _context.UserCart.Where(o => o.BookId == id && o.UserId == userId).FirstOrDefault();
            if (itemInCart == null) return null;
            if(itemInCart.Count > 1)
            {
                itemInCart.Count--;
                _context.UserCart.Update(itemInCart);
            }
            else
                _context.UserCart.Remove(itemInCart);
            
            _context.SaveChanges();
            return itemInCart;
        }
        public bool ClearRowInCart(int id, int userId)
        {
            var itemInCart = _context.UserCart.Where(o => o.BookId == id && o.UserId == userId).FirstOrDefault();
            if (itemInCart == null) return false;
            _context.UserCart.Remove(itemInCart);
            _context.SaveChanges();
            return true;
        }

        public void ClearCart(int userId)
        {
            List<UserCart> itemInCart = _context.UserCart.Where(c => c.UserId == userId).ToList();

            foreach (UserCart item in itemInCart)
            {
                _context.UserCart.Remove(item);
            }
            _context.SaveChanges();
        }

        public Book GetById(int id)
        {
            return _context.Books.SingleOrDefault(x => x.Id == id);
        }

        public List<Book> GetAll()
        {
            return _context.Books.ToList();
        }

        
    }
}
