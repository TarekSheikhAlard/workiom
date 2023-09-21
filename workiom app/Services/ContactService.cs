using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using workiom_app.Models;

namespace workiom_app.Services
{
    public class ContactService
    {
        private readonly IMongoCollection<Contact> _myCollection;

        public ContactService(IMongoClient client, IConfiguration config)
        {
            var database = client.GetDatabase(config.GetSection("MongoDB:Database").Value);
            _myCollection = database.GetCollection<Contact>("contact");
        }

        public void InsertOne(Contact contact)
        {
            _myCollection.InsertOne(contact);
        }
        public List<Contact> Get() => _myCollection.Find(model => true).ToList();

        public Contact Get(string id)=> _myCollection.Find(contact => contact.id.Equals(id)).First();
        

        public void Delete(string id)
        {
            _myCollection.DeleteOne(contact => contact.id.Equals(id));
        }

        public void Update(string id, Contact contactIn)
        {
            _myCollection.ReplaceOne(contact => contact.id == id, contactIn);
        }


    }
}
