using System;

namespace MyFullstackApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public User(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }

        public Post(int id, string title, string content, int userId)
        {
            Id = id;
            Title = title;
            Content = content;
            UserId = userId;
        }
    }
}